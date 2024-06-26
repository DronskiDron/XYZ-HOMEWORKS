﻿using Cinemachine;
using UnityEngine;

namespace General.Components.Cutscenes
{
    public class CameraStateController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CinemachineVirtualCamera _camera;

        private static readonly int ShowTargetKey = Animator.StringToHash("Show-Target");


        public void SetPosition(Vector3 targetPosition)
        {
            targetPosition.z = _camera.transform.position.z;
            _camera.transform.position = targetPosition;
        }


        public void SetState(bool state)
        {
            _animator.SetBool(ShowTargetKey, state);
        }
    }
}
