using UnityEngine;

namespace BF2D
{
    public class TcPaths
    {
        public static void Run(string[] arguments)
        {
            Terminal.IO.Log($"Streaming Assets Path: {Application.streamingAssetsPath}");
            Terminal.IO.Log($"Persistent Data Path: {Application.persistentDataPath}");
        }
    }
}