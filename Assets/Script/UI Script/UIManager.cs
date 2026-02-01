using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;     
    public static UIManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] private UIGamePlay uIGamePlay;
    [SerializeField] private UIShop uIShop;

    public void OpenShop()
    {
        uIShop.gameObject.SetActive(true);
        uIShop.InitUI();
    }

    public void CloseShop()
    {
        uIShop.Close();
    }   

}
