using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Textbox : MonoBehaviour
{
    [SerializeField] private Image textbox;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Image continueArrow;

    public static Textbox Instance { get { return _instance; } }
    private static Textbox _instance;

    public float MessageSpeed { set { _messageSpeed = value;  } }
    private float _messageSpeed = 0.1f;

    private void Awake() {
        //Setup of Monobehaviour Singleton
        if (_instance != this && _instance != null) {
            Destroy(_instance.gameObject);
        }
        _instance = this;
    }

    public void Message(string message) {
        textbox.gameObject.SetActive(true);

        StartCoroutine(MessageParseAndDisplay(message, 0, _messageSpeed));
    }

    public void Message(string key, string index) {

    }

    private IEnumerator MessageParseAndDisplay(string message, int index, float speed) {
        //Begin tag parsing
        if (message[index] == '[') {
            //Take and read tag
            char tag = message[index + 1];
            int newIndex = index;
            switch (tag) {
                case 'P':   //Pause for seconds
                    //Debug.Log("[Textbox]: Pause for seconds: " + pause);
                    yield return new WaitForSeconds(ParseTag(message, ref newIndex));
                    StartCoroutine(MessageParseAndDisplay(message, newIndex + 1, speed));
                    break;
                case 'S':   //New text speed
                    float newSpeed = ParseTag(message, ref newIndex);
                    newSpeed = newSpeed >= 0 ? newSpeed : _messageSpeed; //If the new speed is less than 0, set it to the default speed
                    StartCoroutine(MessageParseAndDisplay(message, newIndex + 1, newSpeed));
                    break;
                default:
                    Debug.Log("[Textbox]: Tag was not a valid character");
                    break;
            }
        } else {
            string currentMessage = textField.text;
            currentMessage = currentMessage + message[index];
            textField.text = currentMessage;

            yield return new WaitForSeconds(speed);

            //If the newIndex is greater than/equal to the length of the message, finalize the message
            if (message.Length > index + 1) {
                //Recurse
                StartCoroutine(MessageParseAndDisplay(message, index + 1, speed));
            } else {
                //Enable continue arrow
                continueArrow.enabled = true;
            }
        }
    }

    private float ParseTag(string message, ref int index) {
        //Move to tag
        index++;
        //Move to colon, check if colon exists, move on
        index++;
        if (message[index] != ':') {
            Debug.Log("[Textbox]: Incorrect Syntax, add ':' after tag");
        }
        index++;

        char character = message[index];    //Initialize character index
        string valueString = "";    //Float before conversion

        while (character != ']') {
            valueString += character;
            character = message[++index];
        }

        //If the value is default, return -1
        if (valueString == "default") {
            return -1f;
        }

        return float.Parse(valueString);   //Convert parsed string to float
    }
}
