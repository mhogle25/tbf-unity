using UnityEngine;

namespace BF2D
{
    public class SCPaths
    {
        public static void Run(params string[] arguments)
        {
            ShCtx.One.Log($"Streaming Assets Path: {Application.streamingAssetsPath}");
            ShCtx.One.Log($"Persistent Data Path: {Application.persistentDataPath}");
        }
    }
}