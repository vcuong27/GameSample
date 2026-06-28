using UnityEngine;

public class UIMainBattle : MonoBehaviour
{
    [SerializeField]
    private UIDefendPlayerInfo DefendPlayerInfo;

    [SerializeField]
    private UIBatterInfor BatterInfo;

    [SerializeField]
    private UIUintPannelInfor UintPannelInfor;

    public void Init()
    {
        DefendPlayerInfo.Init();
        UintPannelInfor.Init();
        BatterInfo.Init();
    }

    public void UPdateBattleInfor()
    {
        //BatterInfo.Update();
    }

    public void ShowPrepareScreen()
    {

    }

    internal void EndBattle()
    {
    }

    internal void ShowResult(BattleResult result)
    {
        BattleResultData battleResultData = BattleManager.Instance.GetBattleResultData();
        PlayerProfile.Instance.IncreasePlayerExperience(battleResultData.ExpErned);
        //OnlineManager.Instance.SentBattleResult(result, battleResultData);
    }

    internal void StartBattle()
    {
    }

}
