using System;
using UnityEngine;

namespace BF2D.Utilities
{
    public struct FileWriter
    {
        public FileWriter(ExternalFileManager fileManager, string id, string content, Action callback)
        {
            this.fileManager = fileManager;
            this.id = id;
            this.content = content;
            this.callback = callback;
        }

        public bool FileExists => this.fileManager.FileExists(this.id);
        public bool FileExistsStreaming => this.fileManager.FileExists(this.id, Enums.GameDirectory.Streaming);
        public string ID => this.id;

        private readonly ExternalFileManager fileManager;
        private readonly string id;
        private readonly string content;
        private readonly Action callback;

        /// <summary>
        /// Writes to the file unless a file with a matching ID exists in the streaming assets folder.
        /// </summary>
        public void Overwrite()
        {
            if (!this.FileExistsStreaming)
            {
                try
                {
                    this.fileManager.WriteToFile(this.id, this.content);
                }
                catch (Exception x)
                {
                    Debug.LogError(x.Message);
                    return;
                }

                this.callback?.Invoke();
            }
        }

        /// <summary>
        /// Checks if the file exists before writing. If it does, the write fails and a warning message is displayed.
        /// </summary>
        public void Write()
        {
            if (this.FileExists)
            {
                Debug.LogWarning($"[FileWriter:Write] Failed, file at id '{this.id}' already exists.");
                return;
            }

            Overwrite();
        }
    }
}