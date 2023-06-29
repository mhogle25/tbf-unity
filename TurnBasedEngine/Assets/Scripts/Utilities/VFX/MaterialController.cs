using UnityEngine;
using System;

namespace BF2D.Utilities
{
    class MaterialController : MonoBehaviour
    {
        [SerializeField] private string shaderOffsetKey = "_GradientAdjustment";
        [SerializeField] private float shaderTransitionSpeed = 1f;
        [SerializeField] private float transitionThreshold = 0.01f;

        [SerializeField] private Material material = null;

        private Action state = null;
        private float approaching;

        private float ShaderOffset
        {
            get => this.material.GetFloat(this.shaderOffsetKey);
            set => this.material.SetFloat(this.shaderOffsetKey, value);
        }

        private void Update()
        {
            this.state?.Invoke();
        }

        public void NewPaletteOffsetClocked(float value)
        {
            this.approaching = value;
            this.state = StatePaletteOffset;
        }

        private void StatePaletteOffset()
        {
            if (this.approaching - this.ShaderOffset < this.transitionThreshold)
            {
                this.approaching = default;
                this.state = null;
            }

            this.ShaderOffset = Mathf.Lerp(
            this.ShaderOffset,
            this.approaching,
            Time.deltaTime * this.shaderTransitionSpeed
            );
        }

    }
}