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

    private BarrackBuilding barrackBuilding;

    public void Initialize(BarrackBuilding building)
    {
        barrackBuilding = building;
        TimeText.text = "0s";
        ProgressImg.fillAmount = 0f;
    }

    public void UpdateUI(float percent, float timeRemaining)
    {
        ProgressImg.fillAmount = percent;
        TimeText.text = $"{timeRemaining:F1}s";
    }

    public void Confirm()
    {
        Debug.Log("Confirm");
        barrackBuilding.Confirm();
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        barrackBuilding.Cancel();
    }

}
