using System;
using UnityEngine;

public class ConfirmPopup : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI titleText;
    [SerializeField] private TMPro.TextMeshProUGUI messageText;

    private Action onConfirm;
    private Action onCancel;

    internal void Init(string title, string message, Action onConfirm, Action onCancel)
    {
        titleText.text = title;
        messageText.text = message;
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
    }

    public void OnConfirm()
    {
        onConfirm?.Invoke();
    }

    public void OnCancel()
    {
        onCancel?.Invoke();
    }
}
