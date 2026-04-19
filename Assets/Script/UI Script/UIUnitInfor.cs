using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitInfor : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI textUnitName;
    [SerializeField]
    private Image imageUnitIcon;


    private PlayerUnitData unitData;
    public void Init(PlayerUnitData unit)
    {
        unitData = unit;
        //DataManager.Instance.GetUnitData(unitData.Type, out UnitData data);
        textUnitName.text = "Name";
        //imageUnitIcon.sprite = unitData.UnitIcon;
    }
    
}
