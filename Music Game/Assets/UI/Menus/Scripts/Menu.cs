using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuGeneric<T> : Menu where T: MenuGeneric<T>
{
    //static T instance;
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);          
        }
        else
        {
            Instance = (T)this;
        }
    }
    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public static void Open()
    {
        if(MenuManager.Instance && Instance)
        {
            MenuManager.Instance.OpenMenu(Instance);
        }
    }   

    public override void OnBackPressed()
    {
        if (MenuManager.Instance)
        {
            MenuManager.Instance.CloseCurrentMenu();
        }
    }
}

[RequireComponent(typeof(Canvas))]
public abstract class Menu : MonoBehaviour
{
    public abstract void OnBackPressed();
    
}
