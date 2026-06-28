using Lean.Gui;
using System.Collections.Generic;
using UnityEngine;

public class IMenuStack : MonoBehaviour
{
    Stack<LeanWindow> menuStack = new Stack<LeanWindow>();

    [SerializeField] private ConfirmPopup confirmPopupPref;
    [SerializeField] private NoticePopup noticePopupPref;
    [SerializeField] private BlockPopup blockPopupPref;

    [SerializeField] private GameObject uIRoot;


    public void OpenMenu(LeanWindow menu)
    {
        menuStack.Push(menu);
        menu.Set(true);
        menu.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        if (menuStack.Count > 0)
        {
            LeanWindow topMenu = menuStack.Pop();
            topMenu.Set(false);
            topMenu.gameObject.SetActive(false);
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

    public GameObject GetUIRoot()
    {
        return uIRoot;
    }

    public void ShowConfirmPopup(string title, string message, System.Action onConfirm, System.Action onCancel)
    {
        ConfirmPopup popup = Instantiate(confirmPopupPref, uIRoot.transform);
        popup.Init(title, message, onConfirm, onCancel);
        OpenMenu(popup.GetComponent<LeanWindow>());
    }

    public void ShowNoticePopup(string title, string message)
    {
        NoticePopup popup = Instantiate(noticePopupPref, uIRoot.transform);
        //popup.Init(title, message);
        OpenMenu(popup.GetComponent<LeanWindow>());
    }


    private BlockPopup blockPopup = null;

    public void ShowBlockPopup(string title, string message)
    {
        if (blockPopup == null)
        {
            blockPopup = Instantiate(blockPopupPref, uIRoot.transform);
            //popup.Init(title, message);
            //OpenMenu(popup.GetComponent<LeanWindow>());
        }
    }

    public void CloseBlockPopup()
    {
        if (blockPopup != null)
        {
            Destroy(blockPopup.gameObject);
            blockPopup = null;
        }
    }


}
