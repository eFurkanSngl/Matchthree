using System;
using UnityEngine;

namespace Utils
{
    public abstract class EventListenerMono:MonoBehaviour
    {
        private void Start()
        {
            RegisterEvents();
        }

        protected void OnEnable()
        {
            RegisterEvents();
        }
        // Enable olduğunda Kayıt oluyor

        protected void OnDisable()
        {
            UnRegisterEvents();
        }
        // İzin yoksa Kayıttan cıkıyor

        protected abstract void UnRegisterEvents();

        protected  abstract void RegisterEvents();

        
    }
}