using System;
using UnityEngine;

namespace BF2D.Utilities
{
    public class FileWriter
    {
        public FileWriter(ExternalFileManager fileManager, string id, string content)
        {
            this.fileManager = fileManager;
            this.id = id;
            this.content = content;
        }

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

        private readonly ExternalFileManager fileManager = null;
        private readonly string id = string.Empty;
        private readonly string content = string.Empty;
        private readonly Action callback = null;

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