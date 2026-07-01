using Lean.Gui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarrackBuildingUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TimeText;
    [SerializeField]
    private Image ProgressImg;
    [SerializeField]
    private Image ItemImg;
    [SerializeField]
    private LeanButton ConfirmButton;
    [SerializeField]
    private LeanButton CancelButton;

    private BarrackBuilding barrackBuilding;

    public void Initialize(BarrackBuilding building)
    {
        barrackBuilding = building;
        ProgressImg.gameObject.SetActive(false);
        TimeText.text = "0s";
    }

    public void UpdateConstructTime(float percent, TimeSpan timeRemaining)
    {
        ProgressImg.fillAmount = percent;
        TimeText.text = $"{timeRemaining.ToString()}";
    }

    public void HideUI()
    {
        ProgressImg.gameObject.SetActive(false);
        ConfirmButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
    }

    public void Confirm()
    {
        Debug.Log("Confirm");
        if (PlayerProfile.Instance.GetPlayerCoins() < DataManager.Instance.GetBuildingDataByID(BuildingType.BARRACKS).Price)
        {
            Debug.Log("Not enough coins build.");
            barrackBuilding.gameObject.SetActive(false);
            Destroy(barrackBuilding.gameObject);
            return;
        }
        barrackBuilding.Confirm();
        HideUI();
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        HideUI();
        barrackBuilding.gameObject.SetActive(false);
        Destroy(barrackBuilding.gameObject);
        barrackBuilding.Cancel();
    }

}
