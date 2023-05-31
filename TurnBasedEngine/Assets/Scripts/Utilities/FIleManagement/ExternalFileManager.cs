using System.IO;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Enums;
using System;
using System.Runtime.Serialization;

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
                TextFile.LoadFiles(data, Path.GetFullPath(Path.Combine(Application.persistentDataPath, this.path)));

            if (this.directory == GameDirectory.Streaming || this.directory == GameDirectory.All)
                TextFile.LoadFiles(data, Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, this.path)));

            return data;
        }

        public override Dictionary<string, string> LoadFiles()
        {
            Dictionary<string, string> data = new();

            if (this.directory == GameDirectory.Persistent || this.directory == GameDirectory.All)
                TextFile.LoadFiles(data, Path.GetFullPath(Path.Combine(Application.persistentDataPath, this.path)));

            if (this.directory == GameDirectory.Streaming || this.directory == GameDirectory.All)
                TextFile.LoadFiles(data, Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, this.path)));

            return data;
        }

        public override string LoadFile(string id)
        {
            string persistentContent = LoadFilePersistent(id);
            string streamingContent = LoadFileStreaming(id);

            if (!string.IsNullOrEmpty(persistentContent) && !string.IsNullOrEmpty(streamingContent))
            {
                Debug.Log($"[ExternalFileManager:LoadFile] There is a duplicate file ID for path {this.path} (ID: {id}). Check persistent and streaming data paths and remove one of the two files.");
                return string.Empty;
            }

            if (persistentContent is null && streamingContent is null)
            {
                Debug.LogError($"[ExternalFileManager:LoadFile] The file at id '{id}' does not exist");
                return string.Empty;
            }

            return string.IsNullOrEmpty(persistentContent) ? streamingContent : persistentContent;
        }

        public bool FileExists(string id, GameDirectory directory)
        {
            return directory switch
            {
                GameDirectory.All => FileExists(id),
                GameDirectory.Persistent => !string.IsNullOrEmpty(LoadFilePersistent(id)),
                GameDirectory.Streaming => !string.IsNullOrEmpty(LoadFileStreaming(id)),
                _ => false
            };
        }

        private string LoadFilePersistent(string id)
        {
            if (this.directory == GameDirectory.Persistent || this.directory == GameDirectory.All)
                return TextFile.LoadFileQuiet(Path.GetFullPath(Path.Combine(Application.persistentDataPath, this.path, $"{id}.{this.fileExtension}")));

            return string.Empty;
        }

        private string LoadFileStreaming(string id)
        {
            if (this.directory == GameDirectory.Streaming || this.directory == GameDirectory.All)
                return TextFile.LoadFileQuiet(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, this.path, $"{id}.{this.fileExtension}")));

            return string.Empty;
        }

        public void WriteToFile(string id, string content)
        {
            if (this.directory == GameDirectory.Streaming)
            {
                Debug.LogWarning("[ExternalFileManager:WriteToFile] Cannot write to a streaming assets file, they are readonly.");
                return;
            }

            StreamWriter writer = new(Path.GetFullPath(Path.Combine(Application.persistentDataPath, this.path, $"{id}.{this.fileExtension}")), false);
            writer.WriteLine(content);
            writer.Close();
        }

        public void DeleteFile(string id)
        {
            if (this.directory == GameDirectory.Streaming)
            {
                Debug.LogWarning("[ExternalFileManager:DeleteFile] Cannot delete a streaming assets file.");
                return;
            }

            try
            {
                string path = Path.GetFullPath(Path.Combine(Application.persistentDataPath, this.path, $"{id}.{this.fileExtension}"));

                // Check if file exists with its full path    
                if (File.Exists(path))
                {   
                    File.Delete(path);
                    Debug.Log($"[ExternalFileManager:DeleteFile] File at id '{id}' deleted.");
                    return;
                }

                Debug.LogError($"[ExternalFileManager:DeleteFile] File at id '{id}' not found");
            }
            catch (IOException x)
            {
                Debug.LogError(x.Message);
            }
        }
    }
}