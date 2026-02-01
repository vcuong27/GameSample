using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image buildingImage;

    BuildingData data;

    public void InitUI(BuildingData buildingData)
    {
        data = buildingData;
        buildingNameText.text = buildingData.buildingName;
        costText.text = buildingData.cost.ToString();
        buildingImage.sprite = DataManager.Instance.GetbuidingDataGames(buildingData.buildingType).buildSprite;
    }

    public void BuyBuilding()
    {
        Debug.Log($"Buy Building {data.id} ");
        GameManager.Instance.BuyBuilding(data.id);
    }    
}
