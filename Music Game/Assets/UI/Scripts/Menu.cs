using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    //public abstract void OnBackPressed();
}
public abstract class Menu<T> : Menu where T : Menu<T>
{
    protected static T _instance;

    MenuManagerUpdated _manager;
    protected virtual void Awake()
    {
        if (_instance == null) _instance = (T)this;
        else Destroy(gameObject);

        _manager = null;
    }
    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
    public void InitializeMenu(MenuManagerUpdated manager)
    {
        _manager = manager;
    }

    public static void Open()
    {
        MenuManagerUpdated.OpenMenu(_instance);
    }

}
