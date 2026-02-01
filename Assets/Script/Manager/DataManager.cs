using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    private Buildings buildingsData;

    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public void InitOnlineData()
    {

    }    

    public void Initlize()
    {
        buildingsData = Resources.Load<Buildings>("Buildings");
    }

    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        Initlize();
        DontDestroyOnLoad(gameObject);
    }

    public Buildings GetBuildingData()
    {
        return buildingsData;
    }

    public BuidingDataGame GetbuidingDataGames(BuildingType type)
    {
        return buildingsData.buidingDataGames.First(x => x.buildingType == type);
    }    

    public BuildingData GetBuildingDataByID(int id)
    {
        return buildingsData.buildingDatas.First(x => x.id == id);
    }

    public BuildingData[] GetBuildingDatas()
    {
        return buildingsData.buildingDatas;
    }

}
