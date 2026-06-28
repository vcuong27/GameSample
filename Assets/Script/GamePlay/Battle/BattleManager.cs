using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    NONE,
    INIT, // khoi tao cac thong tin can thiet cho tran chien
    PREPARE, // cho phep nguoi choi chuan bi, lua chon don vi, sap xep tran hinh
    START, // bat dau tran chien
    BATTLE, // tran chien dang dien ra, cap nhat trang thai cua cac don vi, kiem tra dieu kien ket thuc tran chien
    END // ket thuc tran chien, tinh toan ket qua, cap nhat thong tin nguoi choi, hien thi ket qua
}

public enum BattleResult
{
    WIN,
    LOSE,
    DRAW
}

public class BattleResultData
{
    public BattleResult Result;
    public int RemainingUnits;
    public int DestroyedBuildings;
    public float TimeTaken;
    public int ExpErned;

    public BattleResultData(BattleResult result, int remainingUnits, int destroyedBuildings, float timeTaken, int expErned)
    {
        Result = result;
        RemainingUnits = remainingUnits;
        DestroyedBuildings = destroyedBuildings;
        TimeTaken = timeTaken;
        ExpErned = expErned;
    }
}

public class BattleManager : Singleton<BattleManager>
{

    //enemy
    private List<PlayerBuildingData> listBuilding;
    private List<IBuilding> listBuildingObj;

    //Player
    private List<PlayerUnitData> listPlayerUnit;
    private List<IUnit> listPlayerUnitObj;

    private List<PlayerItemData> listPlayerItem;

    //battle temp data
    private BattleState battleState;
    private BattleResult battleResult;
    private int numberBuilding;
    private int numberPlayerUnit;
    private float remainTime;
    private BattleResultData battleData;


    private void Start()
    {
        battleState = BattleState.INIT;
        //InitData(PlayerProfile.Instance.GetPlayerBuildingDatas(), PlayerProfile.Instance.GetPlayerUnitDatas(), PlayerProfile.Instance.GetPlayerItemData()); 
    }


    public BattleState GetBattleState()
    {
        return battleState;
    }

    public BattleResult GetBattleResult()
    {
        return battleResult;
    }

    public void InitData(List<PlayerBuildingData> buildingDatas, List<PlayerUnitData> playerUnitDatas, List<PlayerItemData> playerItemDatas)
    {
        battleData = new BattleResultData(BattleResult.DRAW, 0, 0, 0, 0);

        listBuilding = buildingDatas;
        listPlayerUnit = playerUnitDatas;
        listPlayerItem = playerItemDatas;
    }

    public void InitBattle()
    {
        // Khoi tao cac thong tin can thiet cho tran chien
        battleState = BattleState.PREPARE;
        BattleController.Instance.InitBattleUI();
        BattleController.Instance.ShowPrepare();

        //load danh sach building và khoi tao map
        foreach (var building in listBuilding)
        {
            BuidingDataGame buildingData = DataManager.Instance.GetbuidingDataGames(building.BuildingType);
            bool result = BattleController.Instance.gridManager.PlaceBuilding(building.Position, buildingData.size, building.BuildingType);
            if (result)
            {
                //GameObject buildingObj = Instantiate(DataManager.Instance.GetbuidingDataGames(buildingData.buildingType).buildingPrefab, BattleController.Instance.object3D.transform);
                //buildingObj.transform.position = GridManager.Instance.CellToWorldCenter(building.Position, buildingData.size);
                //buildingObj.GetComponent<IBuilding>()?.InitBatle();
                //listBuildingObj.Add(buildingObj.GetComponent<IBuilding>());
                //numberBuilding ++;
                //Debug.Log("Building placed successfully.");
            }
            else
            {
                Debug.Log("Failed to place the building.");
            }
        }


    }

    public void PrepareBattle()
    {

    }

    public void StartBattle()
    {
        battleState = BattleState.START;
        battleResult = BattleResult.DRAW;
        numberPlayerUnit = 0;
        remainTime = 5 * 60;

        // logic start
        battleState = BattleState.BATTLE;
    }

    public void BattleUpdate()
    {
        // tha unit
        SpawnUnit();

        int count = 0;
        foreach (var building in listBuildingObj)
        {
            if (building.IsDestroyed() == false)
            {
                count++;
            }
        }
        numberBuilding = count;

        if (numberBuilding == 0)
        {
            battleResult = BattleResult.WIN;
            battleState = BattleState.END;
        }

        count = 0;
        foreach (var unit in listPlayerUnit)
        {
            count += unit.Number;
        }

        foreach (var unit in listPlayerUnit)
        {
            // check HP
            // check status
            // count++
        }

        numberPlayerUnit = count;

        if (numberPlayerUnit == 0)
        {
            battleResult = BattleResult.LOSE;
            battleState = BattleState.END;
        }

        if (remainTime <= 0)
        {
            battleResult = BattleResult.DRAW;
            battleState = BattleState.END;
        }

        // cập nhập tài nguyên, chỉ số
        // cập nhập vào battle result

        // sử dụng item
        // update thông của item khi được sử dụng


    }

    public void EndBattle()
    {
        battleData.Result = battleResult;
        battleData.ExpErned = 100;
        battleData.TimeTaken = 12 * 20;

        BattleController.Instance.ShowResult(battleResult);
        battleState = BattleState.NONE;
    }



    public void Update()
    {

        switch (battleState)
        {
            case BattleState.NONE:
                break;
            case BattleState.INIT:
                InitBattle();
                break;
            case BattleState.PREPARE:
                PrepareBattle();
                break;
            case BattleState.START:
                StartBattle();
                break;
            case BattleState.BATTLE:
                BattleUpdate();
                break;
            case BattleState.END:
                EndBattle();
                break;
        }

    }


    public List<PlayerUnitData> GetListPlayerUnit()
    {
        return listPlayerUnit;
    }

    private void SpawnUnit()
    {
        // kiểm tra người chơi chọn unit nào
        // khi kích chuột vào cùng thả quân thì sẽ sinh ra unit đó ở vị trí đó
        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetMouseHitPoint(out Vector3 hitPoint))
            {
                // Spawn unit at hitPoint
                Debug.Log("Spawn unit at: " + hitPoint);
                // Instantiate(unitPrefab, hitPoint, Quaternion.identity);
                // listPlayerUnit.Add
            }
        }
    }

    private bool TryGetMouseHitPoint(out Vector3 hitPoint)
    {
        hitPoint = default;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, BattleController.Instance.groundLayer, QueryTriggerInteraction.Ignore))
        {
            hitPoint = hit.point;
            return true;
        }

        return false;
    }

    internal BattleResultData GetBattleResultData()
    {
        return battleData;
    }
}
