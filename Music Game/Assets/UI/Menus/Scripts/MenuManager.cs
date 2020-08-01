using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] List<Menu> menuPrefabsList = new List<Menu>();

    Stack<Menu> menuStack = new Stack<Menu>();

    static MenuManager instance;
    public static MenuManager Instance => instance;
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMenus();          
        }        
    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }        
    }

    public void CloseCurrentMenu()
    {
        if (menuStack.Count == 0) return;

        Menu currentMenu = menuStack.Pop();
        currentMenu.gameObject.SetActive(false);

        if (menuStack.Count > 0)
        {
            menuStack.Peek().gameObject.SetActive(true);
        }
    }
    public void ClearMenuHistory()
    {
        while(menuStack.Count > 0)
        {
            Menu menu = menuStack.Pop();
            menu.gameObject.SetActive(false);
        }
    }
    
    public void InitializeMenus()
    {
        Transform menuParent = new GameObject("Menus").transform;
        DontDestroyOnLoad(menuParent.gameObject);

        bool mainMenuPresent = false;
        foreach (Menu menuPrefab in menuPrefabsList)
        {
            if(menuPrefab)
            {
                Menu menuInstance = Instantiate(menuPrefab, menuParent);
                if (menuPrefab is MainMenu)
                {
                    mainMenuPresent = true;

                    if(GameManager.GetCurrentSceneName() == "MenuScene")
                    {
                        OpenMenu(menuInstance);
                    }
                    else
                    {
                        menuInstance.gameObject.SetActive(false);
                    }
                }
                else
                {
                    menuInstance.gameObject.SetActive(false);
                }
            }            
        }
        if(!mainMenuPresent) { Debug.LogWarning("MenuManager.InitializeMenus(): Must have a main menu to play game"); }
    }
    public void OpenMenu(Menu menuInstance)
    {
        if (!menuInstance) return;

        if (menuStack.Count > 0) 
        { 
            menuStack.Peek().gameObject.SetActive(false); 
        }
        // the the menu we want to open is already in the stack
        if (menuStack.Contains(menuInstance))
        {
            while(menuStack.Peek() != menuInstance)
            {
                menuStack.Pop();
            }
            menuStack.Peek().gameObject.SetActive(true);
        }
        else
        {
            menuInstance.gameObject.SetActive(true);
            menuStack.Push(menuInstance);
        }
    }
}
