using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.MusicGame.UI
{
    public abstract class Menu : MonoBehaviour
    {
        //public abstract void OnBackPressed();
    }
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null) Instance = (T)this;
            else Destroy(gameObject);

        }
        protected virtual void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }


        public virtual void Open()
        {
            MenuManager.OpenMenu(Instance);
        }
        public virtual void Close()
        {
            MenuManager.CloseMenu(Instance);
        }

    }
}

