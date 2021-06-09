using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    //Singleton Reference
    public static GlobalGameManager Instance { get { return _instance; } }
    private static GlobalGameManager _instance;
    //All characters
    public List<PlayerStats> Characters { get { return _characters; } }
    private List<PlayerStats> _characters;
    //The characters in your party
    public PlayerStats Char1 { get { return _char1; } }
    private PlayerStats _char1;
    public PlayerStats Char2 { get { return _char2; } }
    private PlayerStats _char2;
    public PlayerStats Char3 { get { return _char3; } }
    private PlayerStats _char3;
    public PlayerStats Char4 { get { return _char4; } }
    private PlayerStats _char4;

    private void Awake() {
        //Set this object not to destroy on loading new scenes
        DontDestroyOnLoad(gameObject);

        //Setup of Monobehaviour Singleton
        if (_instance != this && _instance != null) {
            Destroy(_instance.gameObject);
        }
        _instance = this;
    }
}
