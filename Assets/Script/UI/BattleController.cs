using System;
using UnityEngine;

public class BattleController : IMenuStack
{
    private static BattleController _instance;
    public static BattleController Instance => _instance;

    [SerializeField]
    private UIMainBattle uIMainBattle;
    [SerializeField]
    public GridManager gridManager;
    [SerializeField]
    public LayerMask groundLayer;
    [SerializeField]
    public GameObject object3D;


    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        _instance = this;
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public void InitBattleUI()
    {
        uIMainBattle.Init();
    }

    public void ShowPrepare()
    {
        uIMainBattle.ShowPrepareScreen();
    }   
    
    public void StartBattle()
    {
        BattleManager.Instance.StartBattle();
        uIMainBattle.StartBattle();
    }

    public void EndBattle()
    {
        BattleManager.Instance.EndBattle();
        uIMainBattle.EndBattle();
    }

    public void ShowResult(BattleResult result)
    {
        uIMainBattle.ShowResult(result);
    }    

}
