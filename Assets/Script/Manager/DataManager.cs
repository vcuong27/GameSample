using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class DataManager : MonoBehaviour
{

    private Buildings buildingsData;
    List<IBuildingData> listBuildData;

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
        listBuildData = new List<IBuildingData>();
        listBuildData.Add(buildingsData.farmData);
        listBuildData.Add(buildingsData.barrackData);
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

    public IBuildingData GetBuildingDataByID(BuildingType type)
    {
        return buildingsData.farmData;
    }

    public List<IBuildingData> GetBuildingDatas()
    {
        return listBuildData;
    }

}
