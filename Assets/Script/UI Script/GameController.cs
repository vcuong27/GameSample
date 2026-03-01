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
        OpenMenu(uIShop.gameObject);
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
            case BuildingType.None:
                break;
            case BuildingType.MainTower:
                break;
            case BuildingType.Barracks:
                OpenMenu( uIBarackPanel.gameObject);
                uIBarackPanel.Inlitilize((BarrackBuilding)building);
                break;
            case BuildingType.Farm:
                break;
            case BuildingType.Workshop:
                break;
            case BuildingType.Tree:
                break;
            case BuildingType.Rock:
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
            case BuildingType.None:
                break;
            case BuildingType.MainTower:
                break;
            case BuildingType.Barracks:
                break;
            case BuildingType.Farm:
                break;
            case BuildingType.Workshop:
                break;
            case BuildingType.Tree:
                break;
            case BuildingType.Rock:
                break;
            default:
                break;
        }
    }

}
