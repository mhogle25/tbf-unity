using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace BF2D
{
    public class ShOut : MonoBehaviour
    {
        [SerializeField] private LayoutGroup content = null;
        [SerializeField] private TextMeshProUGUI standardMessagePrefab = null;
        [SerializeField] private TextMeshProUGUI warningMessagePrefab = null;
        [SerializeField] private TextMeshProUGUI errorMessagePrefab = null;
        [SerializeField] private int messageLimit = 10;

        private readonly Queue<TextMeshProUGUI> history = new();

        public void Log(string message)
        {
            InstantiateMessage(this.standardMessagePrefab, message);
        }

        public void LogWarning(string warning)
        {
            InstantiateMessage(this.warningMessagePrefab, warning);
        }

        public void LogError(string error)
        {
            InstantiateMessage(this.errorMessagePrefab, error);
        }

        public void Clear()
        {
            while(this.history.Count > 0)
            {
                Destroy(this.history.Dequeue().gameObject);
            }
        }

        private void InstantiateMessage(TextMeshProUGUI prefab, string text)
        {
            TextMeshProUGUI instance = Instantiate(prefab);
            instance.transform.SetParent(content.transform);
            instance.transform.localScale = Vector3.one;
            instance.transform.localPosition = new Vector3(instance.transform.localPosition.x, instance.transform.localPosition.y, 0);
            instance.text = text;

            this.history.Enqueue(instance);
            if (this.history.Count > this.messageLimit)
            {
                Destroy(this.history.Dequeue().gameObject);
            }
        }
    }
}