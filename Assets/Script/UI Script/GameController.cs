using Lean.Gui;
using UnityEngine;
using UnityEngine.UIElements;

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


    IBuilding openedBuildingMenu;

    public void OpenShop()
    {
        OpenMenu(uIShop.gameObject.GetComponent<LeanWindow>());
        uIShop.InitUI();
    }

    public void CloseShop()
    {
        CloseMenu();
    }

    public void OpenBuildMenu(IBuilding building )
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
                //OpenMenu( uIBarackPanel.gameObject.getc);
                uIBarackPanel.Inlitilize((BarrackBuilding)building);
                break;
            case BuildingType.FARM_GOLD:
                break;
            case BuildingType.WORKSHOP:
                break;
            case BuildingType.TREE:
                break;
            case BuildingType.ROCK:
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
                break;
            case BuildingType.FARM_GOLD:
                break;
            case BuildingType.WORKSHOP:
                break;
            case BuildingType.TREE:
                break;
            case BuildingType.ROCK:
                break;
            default:
                break;
        }
    }

}
