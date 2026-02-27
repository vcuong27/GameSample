using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMenuStack : MonoBehaviour
{
    Stack<GameObject> menuStack = new Stack<GameObject>();

    [SerializeField] private GameObject uIRoot;

    public void OpenMenu(GameObject menu)
    {
        menuStack.Push(menu);
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        if (menuStack.Count > 0)
        {
            GameObject topMenu = menuStack.Pop();
            topMenu.SetActive(false);
        }
    }

    public void ClearMenus()
    {
        while (menuStack.Count > 0)
        {
            GameObject menu = menuStack.Pop();
            menu.SetActive(false);
        }
    }

}
