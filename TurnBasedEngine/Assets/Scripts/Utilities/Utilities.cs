using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BF2D.Utilities
{
    public static class Audio
    {
        public static async void LoadClips(Dictionary<string, AudioClip> collection, string path)
        {
            string[] paths = Directory.GetFiles(path);

            foreach (string p in paths)
            {
                string filename = Path.GetFileNameWithoutExtension(p);
                AudioClip audioClip = await BF2D.Utilities.Audio.LoadClip(p);
                collection[filename] = audioClip;
            }
        }

        public static async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                uwr.SendWebRequest();

                // wrap tasks in try/catch, otherwise it'll fail silently
                try
                {
                    while (!uwr.isDone) await Task.Delay(5);

                    if (uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}, {e.StackTrace}");
                }
            }

            return clip;
        }

        public static void PlayAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
                return;

            if (audioSource.clip != null)
                audioSource.Play();
        }
    }

    public static class TextFile
    {

        public static string LoadFile(string path)
        {
            string content;
            try
            {
                content = File.ReadAllText(path);
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadFile] The files specified by path '{path}' do not exist");
                return string.Empty;
            }
            return content;
        }

        public static T DeserializeString<T>(string content)
        {
            T t;
            try
            {
                t = JsonConvert.DeserializeObject<T>(content, new Newtonsoft.Json.Converters.StringEnumConverter());
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:DeserializeString] Tried to deserialize JSON but it was not valid. Content: {content}");
                return default;
            }
            return t;
        }

        public static int LoadTextFiles(Dictionary<string, string> collection, string path)
        {
            string[] paths;
            try
            {
                paths = Directory.GetFiles(path);
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadTextFiles] The specified path '{path}' was invalid");
                return -1;
            }

            foreach (string p in paths)
            {
                LoadTextFile(collection, p);
            }

            return paths.Length;
        }

        public static void LoadTextFile(Dictionary<string, string> collection, string path)
        {
            try
            {
                string filename = Path.GetFileNameWithoutExtension(path);
                string content = File.ReadAllText(path);
                collection[filename] = content;
            } 
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadTextFile] The file at path '{path}' does not exist");
            }
        }

        public static int LoadTextFiles(Dictionary<string, List<string>> collection, string path)
        {
            string[] paths;
            try
            {
                paths = Directory.GetFiles(path);
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadTextFile] Path '{path}' does not exist");
                return -1;
            }

            foreach (string p in paths)
            {
                LoadTextFile(collection, p);
            }

            return paths.Length;
        }

        public static void LoadTextFile(Dictionary<string, List<string>> collection, string path)
        {
            try
            {
                string filename = Path.GetFileNameWithoutExtension(path);
                List<string> dialog = new();

                using (StreamReader stream = new(path))
                {
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        dialog.Add(line);
                    }
                }

                collection[filename] = dialog;
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadTextFile] The file at path '{path}' does not exist");
            }
        }
    }
}
