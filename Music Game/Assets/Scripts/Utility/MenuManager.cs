using System.Collections.Generic;
using UnityEngine;

namespace KD.MusicGame.UI
{
    public class MenuManager : MonoBehaviour
    {
        static MenuManager _instance;

        [SerializeField] Menu[] menuPrefabs = null;

        List<Menu> menus = new List<Menu>();
        Menu _currentMenu;
        private void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(_instance);
        }
        private void Start()
        {
            InitializeMenus();
        }
        void InitializeMenus()
        {
            _currentMenu = null;
            menus.Clear();

            foreach (var menu in menuPrefabs)
            {
                if (menu != null)
                {
                    Menu newMenu = Instantiate(menu, transform);
                    newMenu.gameObject.SetActive(false);
                    menus.Add(newMenu);

                    if (menu is MainMenu && Utility.GameManager.GetCurrentSceneName() == Utility.GameManager.StartScene)
                    {
                        MainMenu.Instance.Open();
                    }
                }
            }
        }
        public static void CloseAllMenus()
        {
            if (_instance == null) return;
            foreach (var menu in _instance.menus)
            {
                if (menu != null) menu.gameObject.SetActive(false);
            }
            _instance._currentMenu = null;
        }
        public static void OpenMenu(Menu menu)
        {
            if (menu == null || _instance == null || menu == _instance._currentMenu) return;

            Menu menuToOpen = _instance.menus.Find(x => x == menu);
            if (menuToOpen != null)
            {
                //Debug.Log(menuToOpen.name);
                _instance.CloseCurrentMenu();
                menuToOpen.gameObject.SetActive(true);
                _instance._currentMenu = menuToOpen;
                Utility.SceneTransitions.sceneLoadingComplete = true;
            }
        }
        public static void CloseMenu(Menu menu)
        {
            if (menu == null || _instance == null) return;

            if (menu == _instance._currentMenu) _instance.CloseCurrentMenu();
        }
        void CloseCurrentMenu()
        {
            if (_currentMenu != null)
            {
                _currentMenu.gameObject.SetActive(false);
                _currentMenu = null;
            }
        }

    }
}

