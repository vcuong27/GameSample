using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMenuStack : MonoBehaviour
{
    Stack<LeanWindow> menuStack = new Stack<LeanWindow>();

    [SerializeField] private GameObject uIRoot;

    public void OpenMenu(LeanWindow menu)
    {
        menuStack.Push(menu);
        menu.Set(true);
    }

    public void CloseMenu()
    {
        if (menuStack.Count > 0)
        {
            LeanWindow topMenu = menuStack.Pop();
            topMenu.Set(false);
        }
    }

    public void ClearMenus()
    {
        while (menuStack.Count > 0)
        {
            LeanWindow menu = menuStack.Pop();
            menu.Set(false);
        }
    }

}
