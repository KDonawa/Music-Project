using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuTemplpate<T> : Menu where T: MenuTemplpate<T>
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

    public static void Open()
    {
        if(MenuManager.Instance && Instance)
        {
            MenuManager.Instance.OpenMenu(Instance);
        }
    }
    protected virtual void OnDestroy()
    {
        if(Instance == this) Instance = null;
    }
}

[RequireComponent(typeof(Canvas))]
public abstract class Menu : MonoBehaviour
{
    public virtual void OnBackPressed()
    {
        if (MenuManager.Instance)
        {
            MenuManager.Instance.CloseCurrentMenu();
        }
    }  
}
