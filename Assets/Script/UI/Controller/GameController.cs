using Lean.Gui;
using UnityEngine;

public class GameController : IMenuStack
{
    private static GameController _instance;
    public static GameController Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] private UIGamePlay uIGamePlay;
    [SerializeField] private UIShop uIShop;
    [SerializeField] private UIBarackPanel uIBarackPanel;
    [SerializeField] private FindClanUI uIFindClanUI;
    [SerializeField] private ClanPanelUI uIClanPanelUI;


    IBuilding openedBuildingMenu;

    public void OpenShop()
    {
        ShowBlockPopup("Shop", "Wating ...");
        OpenMenu(uIShop.gameObject.GetComponent<LeanWindow>());
        uIShop.InitUI();
        CloseBlockPopup();
    }

    public void CloseShop()
    {

        CloseMenu();
        ShowNoticePopup("Shop Closed", "You have closed the shop.");
        ShowNoticePopup("Game Play", "Wating ...");
    }

    public void OpenGamePlay()
    {
        ShowConfirmPopup("Start Battle", "Are you sure you want to start the battle?", () =>
        {
            GameSceneManager.Instance.LoadScene(GameSceneManager.SCENE_BATTLE);
        },
        () =>
        {
            CloseMenu();
        });
    }

    public void OpenBuildMenu(IBuilding building)
    {
        openedBuildingMenu = building;
        BuildingType type = openedBuildingMenu.buildingType;
        switch (type)
        {
            case BuildingType.NONE:
                break;
            case BuildingType.MAINTOWER:
                break;
            case BuildingType.BARRACKS:
                uIBarackPanel.gameObject.SetActive(true);
                uIBarackPanel.Initilize((BarrackBuilding)building, this);
                break;
            case BuildingType.FARM_GOLD:
                break;
            default:
                break;
        }
    }

    public void CloseBuildMenu()
    {
        CloseMenu();

        if (openedBuildingMenu == null)
        {
            return;
        }

        BuildingType type = openedBuildingMenu.buildingType;
        switch (type)
        {
            case BuildingType.NONE:
                break;
            case BuildingType.MAINTOWER:
                break;
            case BuildingType.BARRACKS:
                uIBarackPanel.gameObject.SetActive(false);
                break;
            case BuildingType.FARM_GOLD:
                break;
            default:
                break;
        }
    }

    public void OpenClanMenu()
    {
        if (PlayerProfile.Instance.getClanID() <= 0)
        {
            OpenMenu(uIFindClanUI.gameObject.GetComponent<LeanWindow>());
            uIFindClanUI.initialize(this);
        }
        else
        {
            OpenMenu(uIClanPanelUI.gameObject.GetComponent<LeanWindow>());
            uIClanPanelUI.initialize(this);
        }
    }

    public void ShowClanWarPanel()
    {
        Debug.Log("ShowClanWarPanel");
    }
}
