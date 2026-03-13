using Lean.Gui;
using System;
using TMPro;
using Unity.VisualScripting;
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

    private void OnEnable()
    {
        PlayerProfile.OnProfileUpdated += PlayerProleUpdated;
    }

    private void OnDisable()
    {
        PlayerProfile.OnProfileUpdated -= PlayerProleUpdated;
    }



    private void PlayerProleUpdated()
    {
        Initilize();
    }

    public void Initilize()
    {
        playerName.text = PlayerProfile.Instance.GetPlayerName();
        playerGold.text = PlayerProfile.Instance.GetCoins().ToString();


    }  
    
    public void JoyStickSet(Vector2 vector)
    {
        //GameController.Instance.MovePlayer(x, y);

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
