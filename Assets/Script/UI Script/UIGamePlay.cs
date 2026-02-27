using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : MonoBehaviour
{
    [SerializeField]
    private Image PlayerImg;
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private TextMeshProUGUI playerGold;

    private void Start()
    {
        Initilize();
    }


    public void Initilize()
    {
        playerName.text = PlayerProfile.Instance.GetPlayerName();
        playerGold.text = PlayerProfile.Instance.GetCoins().ToString();
    }   
    
    public void OpenPopup()
    {
        GameController.Instance.OpenShop();
    }

    public void OpenPopup1()
    {
    }

    public void OpenPopup2()
    {
    }

}
