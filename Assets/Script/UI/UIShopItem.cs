using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image buildingImage;

    IBuildingData data;

    public void InitUI(IBuildingData buildingData)
    {
        data = buildingData;
        buildingNameText.text = buildingData.Name;
        costText.text = buildingData.Price.ToString();
        buildingImage.sprite = DataManager.Instance.GetbuidingDataGames(buildingData.BuildingType).buildSprite;
    }

    public void BuyBuilding()
    {
        Debug.Log($"Buy Building {data.BuildingType} ");
        //GameManager.Instance.BuyBuilding(data.BuildingType);
    }    
}
