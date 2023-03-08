using UnityEngine;

namespace BF2D
{
    public class TcPaths
    {
        public static void Run(string[] arguments)
        {
            Terminal.IO.LogQuiet($"Streaming Assets Path: {Application.streamingAssetsPath}");
            Terminal.IO.LogQuiet($"Persistent Data Path: {Application.persistentDataPath}");
        }
    }
}