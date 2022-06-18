using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _smoothSpeed = 0.125f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, 0f);

        private void LateUpdate()
        {
            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}

