using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D.Utilities
{
    public class InternalFileManager : FileManager
    {
        [SerializeField] private List<TextAsset> textFiles = new();

        private Dictionary<string, string> cachedData = null;

        public override Dictionary<string, List<string>> LoadFilesLined()
        {
            Dictionary<string, List<string>> data = new();
            foreach (TextAsset file in this.textFiles)
            {
                List<string> content = new();
                foreach (string line in file.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    content.Add(line);
                }
                data[file.name] = content;
            }
            return data;
        }

        public override Dictionary<string, string> LoadFiles()
        {
            Dictionary<string, string> data = new();
            foreach (TextAsset file in this.textFiles)
            {
                data[file.name] = file.text;
            }
            return data;
        }

        public override string LoadFile(string id)
        {
            if (this.cachedData is null)
                this.cachedData = LoadFiles();

            if (!this.cachedData.ContainsKey(id))
            {
                Debug.LogError($"[InternalFileManager: LoadFile] The text asset {id} didnt exist.");
                return string.Empty;
            }

            return this.cachedData[id];
        }
    }
}