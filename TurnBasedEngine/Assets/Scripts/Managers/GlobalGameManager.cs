using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
