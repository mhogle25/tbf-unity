using UnityEngine;
using System;
using System.Collections.Generic;


namespace BF2D.UI
{
    public abstract class UIComponent : MonoBehaviour
    {
        public bool Interactable { get { return this.interactable; } set { this.interactable = value; } }
        [SerializeField] private bool interactable = true;
    }
}
