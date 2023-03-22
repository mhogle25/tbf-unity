using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Enums;

namespace BF2D.Utilities
{
    public class ExternalFileManager : FileManager
    {
        [SerializeField] private string path = string.Empty;
        [SerializeField] private string fileExtension = string.Empty;
        [SerializeField] private GameDirectory directory = GameDirectory.Persistent;

        public override Dictionary<string, List<string>> LoadFilesLined()
        {
            Dictionary<string, List<string>> data = new();
            BF2D.Utilities.TextFile.LoadTextFiles(data, GetPathPrefix() + '/' + this.path);
            return data;
        }

        public override Dictionary<string, string> LoadFiles()
        {
            Dictionary<string, string> data = new();
            BF2D.Utilities.TextFile.LoadTextFiles(data, GetPathPrefix() + '/' + this.path);
            return data;
        }

        public override string LoadFile(string id)
        {
            return BF2D.Utilities.TextFile.LoadFile(GetPathPrefix() + '/' + this.path + '/' + $"{id}.{this.fileExtension}");
        }   

        public void WriteToFile(string content, string id)
        {
            if (this.directory == GameDirectory.Streaming)
            {
                Terminal.IO.LogWarning("[ExternalFileManager:WriteToFile] Cannot write to a streaming assets file, they are readonly.");
                return;
            }

            StreamWriter writer = new(Application.persistentDataPath + '/' + this.path + '/' + $"{id}.{this.fileExtension}", false);
            writer.WriteLine(content);
            writer.Close();
        }

        private string GetPathPrefix()
        {
            return this.directory switch
            {
                GameDirectory.Persistent => Application.persistentDataPath,
                GameDirectory.Streaming => Application.streamingAssetsPath,
                _ => string.Empty
            };
        }
    }
}