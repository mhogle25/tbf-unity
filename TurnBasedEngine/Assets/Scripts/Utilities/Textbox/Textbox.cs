using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Textbox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Image textboxArrow;

    public static Textbox Instance { get { return _instance; } }
    private static Textbox _instance;

    public float MessageSpeed { set { _messageSpeed = value;  } }
    private float _messageSpeed = 0.1f;

    private Image textboxImage;

    private void Awake() { 
        textboxImage = GetComponent<Image>();

        //Setup of Monobehaviour Singleton
        if (_instance != this && _instance != null) {
            Destroy(_instance.gameObject);
        }
        _instance = this;
    }

    public void Message(string message) {
        StartCoroutine(MessageParseAndDisplay(message, 0));
    }

    public void Message(string key, string index) {

    }

    private IEnumerator MessageParseAndDisplay(string message, int index) {
        string currentMessage = textField.text;
        currentMessage = currentMessage + message[index];
        textField.text = currentMessage;
        yield return new WaitForSeconds(_messageSpeed);
        if (message.Length > index + 1) {
            StartCoroutine(MessageParseAndDisplay(message, index + 1));
        }
    }
}
