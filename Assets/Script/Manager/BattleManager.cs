using NUnit.Framework;
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

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get
        {
            return _instance;
        }
    }


    private BattleState battleState;
    private BattleResult battleResult;

    //enemy
    private List<IBuildingData> listBuilding;

    //Player
    private List<IUnitData> listUnit;



    private void Start()
    {
        _instance = this;
        battleState = BattleState.INIT;
    }


    public BattleState GetBattleState()
    {
        return battleState;
    }

    public BattleResult GetBattleResult()
    {
        return battleResult;
    }

    public void InitData(List<IBuildingData>  buildingDatas)
    {
        listBuilding = buildingDatas;
    }    

    public void InitBattle()
    {
        // Khoi tao cac thong tin can thiet cho tran chien
        battleState = BattleState.PREPARE;
        BattleController.Instance.ShowPrepare();
        //load danh sach building và khoi tao map
        foreach (var building in listBuilding)
        {
            // khoi tao map
        }   
    }

    public void PrepareBattle()
    {

    }

    public void StartBattle()
    {
        battleState = BattleState.START;
        // logic start

        battleState = BattleState.BATTLE;
    }

    public void BattleUpdate()
    {
        // tha unit
        // cap nhat trang thai cua cac don vi
        // kiem tra dieu kien ket thuc tran chien

        foreach (var unit in listUnit)
        {
            // check HP
            // check status
            // count
        }

        foreach (var building in listBuilding)
        {
            // check HP
            // check status
            // count
        }

        //check timer

        if (true)
        {
            battleResult = BattleResult.WIN;
        }
        else
        {
            battleResult = BattleResult.LOSE;
        }

        battleState = BattleState.END;

    }

    public void EndBattle()
    {
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

}
