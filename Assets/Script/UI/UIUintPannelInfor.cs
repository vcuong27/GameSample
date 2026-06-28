using System.Collections.Generic;
using UnityEngine;

public class UIUintPannelInfor : MonoBehaviour
{
    [SerializeField]
    private GameObject contentUnitInfor;
    [SerializeField]
    private GameObject prefabUnitInfor;

    public void Init()
    {
        List<PlayerUnitData> listUnit = BattleManager.Instance.GetListPlayerUnit();
        foreach (PlayerUnitData unit in listUnit)
        {
            UIUnitInfor uIUnitInfor = Instantiate(prefabUnitInfor, contentUnitInfor.transform).GetComponent<UIUnitInfor>();
            uIUnitInfor.Init(unit);
        }
    }

}
