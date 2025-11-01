using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

namespace Sui
{
    public class Sui_OnPressMethodCall : MonoBehaviour
    {
        [SerializeField] SplineAnimate _splineAnimate;

        public UnityEvent<Action> OnPressEvent;

        private void Awake()
        {
            _splineAnimate = GetComponent<SplineAnimate>();
        }

        private void OnMouseDown()
        {
            if(Sui_LevelManager.Instance._isProcessing) return;

            OnPressEvent?.Invoke(() =>
            {
                
            });
        }
    }

}
