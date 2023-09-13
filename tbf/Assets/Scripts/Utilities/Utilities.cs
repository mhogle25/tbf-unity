using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

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
                AudioClip audioClip = await Audio.LoadClip(p);
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

    public static class Probability
    {
        public static bool Roll(int percentage)
        {
            return Roll(percentage, 0);
        }

        public static bool Roll(int percentage, int modifier)
        {
            return UnityEngine.Random.Range(0 + modifier, 100) < percentage;
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
                Debug.LogError($"[Utilities:TextFile:LoadFile] The file at path '{path}' does not exist");
                return string.Empty;
            }
            return content;
        }

        public static string LoadFileQuiet(string path)
        {
            if (File.Exists(path))
                return LoadFile(path);

            return string.Empty;
        }

        public static int LoadFiles(Dictionary<string, string> collection, string path)
        {
            string[] paths;
            try
            {
                paths = Directory.GetFiles(path);
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadFiles] Path '{path}' was invalid");
                return -1;
            }

            foreach (string p in paths)
            {
                LoadFile(collection, p);
            }

            return paths.Length;
        }

        private static void LoadFile(Dictionary<string, string> collection, string path)
        {
            string id;
            string content;

            try
            {
                id = Path.GetFileNameWithoutExtension(path);
                content = File.ReadAllText(path);
            } 
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadFile] The file at path '{path}' does not exist");
                return;
            }

            if (collection.ContainsKey(id))
            {
                Debug.LogError($"[Utilities:TextFile:LoadFile] Tried to add a file with a duplicate ID to the collection (ID: {id})");
                return;
            }

            collection[id] = content;
        }

        public static int LoadFiles(Dictionary<string, List<string>> collection, string path)
        {
            string[] paths;
            try
            {
                paths = Directory.GetFiles(path);
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadFiles] Path '{path}' was invalid");
                return -1;
            }

            foreach (string p in paths)
            {
                LoadFile(collection, p);
            }

            return paths.Length;
        }

        private static void LoadFile(Dictionary<string, List<string>> collection, string path)
        {
            List<string> dialog = new();

            string id;
            try
            {
                id = Path.GetFileNameWithoutExtension(path);

                using StreamReader stream = new(path);
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    dialog.Add(line);
                }
            }
            catch
            {
                Debug.LogError($"[Utilities:TextFile:LoadFile] The file at path '{path}' does not exist");
                return;
            }

            if (collection.ContainsKey(id))
            {
                Debug.LogError($"[Utilities:TextFile:LoadFile] Tried to add a file with a duplicate ID to the collection (ID: {id})");
                return;
            }

            collection[id] = dialog;
        }
    }

    public static class JSON
    {
        public static T DeserializeJson<T>(string json)
        {
            T t;
            try
            {
                t = JsonConvert.DeserializeObject<T>(json, new Newtonsoft.Json.Converters.StringEnumConverter());
            }
            catch (Exception x)
            {
                Debug.LogError($"[Utilities:TextFile:DeserializeString] Tried to deserialize JSON but it was not valid.");
                Debug.LogError(x.Message);
                return default;
            }
            return t;
        }

        public static string SerializeObject<T>(T obj)
        {
            string t;
            try
            {
                t = JsonConvert.SerializeObject(obj, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());
            }
            catch (Exception x)
            {
                Debug.LogError($"[Utilities:TextFile:SerializeObject] Tried to serialize JSON but it was not valid.");
                Debug.LogError(x.Message);
                return default;
            }
            return t;
        }
    }

    public static class Text
    {
        private static readonly Regex replaceBrackets = new(@"\[[^)]*\]");
        private static readonly Regex replaceExtraSpace = new(@"\s{2,}");

        public static string Wash(this string value) => Text.replaceBrackets.Replace(Text.replaceExtraSpace.Replace(value, " "), "");

        public static string Colorize(this string original, Color32 color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{original}</color>";
        }

        public static string NonZeroToSignedString(this int value)
        {
            if (value > 0)
                return $"+{value}";


            if (value < 0)
                return $"-{Math.Abs(value)}";

            return null;
        }
    }

    public static class Collections
    {

        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
