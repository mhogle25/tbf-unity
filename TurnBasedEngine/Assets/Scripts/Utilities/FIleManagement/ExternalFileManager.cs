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

            if (this.directory == GameDirectory.Persistent || this.directory == GameDirectory.All)
                TextFile.LoadFiles(data, Application.persistentDataPath + '/' + this.path);

            if (this.directory == GameDirectory.Streaming || this.directory == GameDirectory.All)
                TextFile.LoadFiles(data, Application.streamingAssetsPath + '/' + this.path);

            return data;
        }

        public override Dictionary<string, string> LoadFiles()
        {
            Dictionary<string, string> data = new();

            if (this.directory == GameDirectory.Persistent || this.directory == GameDirectory.All)
                TextFile.LoadFiles(data, Application.persistentDataPath + '/' + this.path);

            if (this.directory == GameDirectory.Streaming || this.directory == GameDirectory.All)
                TextFile.LoadFiles(data, Application.streamingAssetsPath + '/' + this.path);

            return data;
        }

        public override string LoadFile(string id)
        {
            string persistentContent = null;
            string streamingContent = null;

            if (this.directory == GameDirectory.Persistent || this.directory == GameDirectory.All)
                persistentContent = TextFile.LoadFile(Application.persistentDataPath + '/' + this.path + '/' + $"{id}.{this.fileExtension}");

            if (this.directory == GameDirectory.Streaming || this.directory == GameDirectory.All)
                streamingContent = TextFile.LoadFile(Application.streamingAssetsPath + '/' + this.path + '/' + $"{id}.{this.fileExtension}");

            if (!string.IsNullOrEmpty(persistentContent) && !string.IsNullOrEmpty(streamingContent))
            {
                Debug.Log($"[ExternalFileManager:LoadFile] There is a duplicate file ID for path {this.path} (ID: {id})");
                return string.Empty;
            }

            return string.IsNullOrEmpty(persistentContent) ? streamingContent : persistentContent;
        }   

        public void WriteToFile(string content, string id)
        {
            if (this.directory == GameDirectory.Streaming)
            {
                Debug.LogWarning("[ExternalFileManager:WriteToFile] Cannot write to a streaming assets file, they are readonly.");
                return;
            }

            StreamWriter writer = new(Application.persistentDataPath + '/' + this.path + '/' + $"{id}.{this.fileExtension}", false);
            writer.WriteLine(content);
            writer.Close();
        }
    }
}