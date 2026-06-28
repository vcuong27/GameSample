using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : MonoBehaviour
{
    [SerializeField] private Image PlayerImg;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerGold;
    [SerializeField] private TextMeshProUGUI playerGems;

    private void Start()
    {
        Initilize();
    }

    private void OnEnable()
    {
        PlayerProfile.OnProfileUpdated += PlayerProleUpdated;
    }

    private void OnDisable()
    {
        PlayerProfile.OnProfileUpdated -= PlayerProleUpdated;
    }

    private void PlayerProleUpdated()
    {
        Initilize();
    }

    public void Initilize()
    {
        playerName.text = PlayerProfile.Instance.GetPlayerName();
        playerLevel.text = "Level: " + PlayerProfile.Instance.GetPlayerLevel().ToString();
        playerGold.text = PlayerProfile.Instance.GetPlayerCoins().ToString();
        playerGems.text = PlayerProfile.Instance.GetPlayerGems().ToString();
    }

    public void JoyStickSet(Vector2 vector)
    {
        //GameController.Instance.MovePlayer(x, y);

    }

    public void OpenPopup()
    {
        GameController.Instance.OpenShop();
    }

    public void OpenClanPopup()
    {
        GameController.Instance.OpenClanMenu();
    }

    public void OpenPopup2()
    {
    }

}
