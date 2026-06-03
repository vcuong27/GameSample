using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class FindClanUI : MonoBehaviour
{

    public GameController controller;

    private void OnEnable()
    {
        ClanManager.OnCLanCreated += HandleClanCreated;
        ClanManager.OnCLanInfoReceived += HandleClanInfoReceived;
    }



    private void OnDisable()
    {
        ClanManager.OnCLanCreated -= HandleClanCreated;
        ClanManager.OnCLanInfoReceived -= HandleClanInfoReceived;
    }

    private void HandleClanCreated()
    {
        controller.CloseBlockPopup();
        controller.ShowBlockPopup("Clan Created", "waiting clan information!");
    }

    private void HandleClanInfoReceived()
    {
        controller.CloseBlockPopup();
        Debug.Log("Clan Info Received: Clan information has been received successfully!");
        // You can also update the UI with the received clan information here
    }



    public void CreateClan()
    {
        controller.ShowConfirmPopup("Create Clan", "Are you sure you want to create a new clan?",
            () =>{
                controller.ShowBlockPopup("Clan Created", "Waiting for create clan!");
                ClanManager.Instance.CreateClan("ClanName_01");
                controller.CloseMenu();
            },
            () =>{
                controller.CloseMenu();
            });
    }

    public void OnClickFindClan()
    {
        int clanID = 123; // Replace with the actual clan ID you want to find
        ClanManager.Instance.GetClanInfo(clanID);
    }

    public void OnClanInfoReceived(SC_ClanInfo clanInfo)
    {
        // Handle the received clan information here
        Debug.LogFormat("Clan ID: {0}, Name: {1}, Score: {2}", clanInfo.clanID, clanInfo.name, clanInfo.score);
    }


}
