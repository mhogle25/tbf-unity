using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

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
        public static void LoadTextFiles(Dictionary<string, string> collection, string path)
        {
            string[] paths = Directory.GetFiles(path);

            foreach (string p in paths)
            {
                LoadTextFile(collection, p);
            }
        }

        public static void LoadTextFile(Dictionary<string, string> collection, string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string content = File.ReadAllText(path);
            collection[filename] = content;
        }

        public static void LoadTextFiles(Dictionary<string, List<string>> collection, string path)
        {
            string[] paths = Directory.GetFiles(path);

            foreach (string p in paths)
            {
                LoadTextFile(collection, p);
            }
        }

        public static void LoadTextFile(Dictionary<string, List<string>> collection, string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            List<string> dialog = new List<string>();

            using (StreamReader stream = new StreamReader(path))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    dialog.Add(line);
                }
            }

            collection[filename] = dialog;
        }
    }
}
