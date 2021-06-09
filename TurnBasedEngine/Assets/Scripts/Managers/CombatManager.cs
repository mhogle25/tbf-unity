using UnityEngine;

public class CombatManager : MonoBehaviour
{
    //Singleton Reference
    public static CombatManager Instance { get { return _instance; } }
    private static CombatManager _instance;

    private void Awake() {
        //Setup of Monobehaviour Singleton
        if (_instance != this && _instance != null) {
            Destroy(_instance.gameObject);
        }
        _instance = this;
    }
}
