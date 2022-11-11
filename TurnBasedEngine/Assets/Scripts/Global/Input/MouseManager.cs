using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class MouseManager : MonoBehaviour
    {
        [SerializeField] SpriteRenderer cursor;

        private void Awake()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            cursor.transform.position = Input.mousePosition;
        }
    }
}
