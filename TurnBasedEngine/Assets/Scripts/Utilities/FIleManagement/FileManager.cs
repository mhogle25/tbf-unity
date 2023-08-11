using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Utilities
{
    public abstract class FileManager : MonoBehaviour
    {
        public abstract Dictionary<string, List<string>> LoadFilesLined();
        public abstract Dictionary<string, string> LoadFiles();
        public abstract string LoadFile(string id);


        public bool FileExists(string id)
        {
            return !string.IsNullOrEmpty(LoadFile(id));
        }
    }
}