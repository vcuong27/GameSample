using System.Collections.Generic;
using UnityEngine;

public class ClanPanelUI : MonoBehaviour
{

    private GameController controller;

    public MyClanDetailUI clanDetailUI;
    public ClanListInforItemUI clanListInforItemPrefab;
    public Transform clanListContent;

    private void OnEnable()
    {
        ClanManager.OnCLanListReceived += OnListClanReceived;
        ClanManager.OnClanWarStarted += OnClanWarStarted;
        ClanManager.OnClanWarInfoReceived += OnClanWarInfoReceived;
    }


    private void OnDisable()
    {
        ClanManager.OnCLanListReceived -= OnListClanReceived;
        ClanManager.OnClanWarStarted -= OnClanWarStarted;
        ClanManager.OnClanWarInfoReceived -= OnClanWarInfoReceived;
    }


    public void initialize(GameController controller)
    {
        this.controller = controller;
        clanDetailUI.initialize();
        ClanManager.Instance.GetListClan(0);
        controller.ShowBlockPopup("Clan Panel", "Loading clan information...");
    }

    public void BackToMainMenu()
    {
        controller.CloseMenu();
    }


    private void OnListClanReceived()
    {
        controller.CloseBlockPopup();

        List<ClanListInfo> listClan = ClanManager.Instance.GetClanListInfo();
        foreach (Transform child in clanListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (ClanListInfo info in listClan)
        {
            ClanListInforItemUI item = Instantiate(clanListInforItemPrefab, clanListContent);
            item.initialize(controller, info);
        }
    }

    private void OnClanWarStarted()
    {
        ClanManager.Instance.GetClanWarInfo();
    }
    private void OnClanWarInfoReceived()
    {
        controller.CloseBlockPopup();
        controller.ShowClanWarPanel();
    }

}
