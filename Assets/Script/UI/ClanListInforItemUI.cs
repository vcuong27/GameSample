using TMPro;
using UnityEngine;

public class ClanListInforItemUI : MonoBehaviour
{
    public TextMeshProUGUI txtClanName;

    private int clanID;
    private GameController controller;

    public void initialize(GameController controller, ClanListInfo info)
    {
        this.controller = controller;
        txtClanName.text = info.name;
        clanID = info.clanID;
    }

    public void attack()
    {
        controller.ShowConfirmPopup("Attack Clan", $"Are you sure you want to attack {txtClanName.text}?", () =>
        {
            ClanManager.Instance.AttackClanID(clanID);
            controller.CloseMenu();
            controller.ShowBlockPopup("Clan Panel", "Starting clan war...");
        },
        () =>
        {
            controller.CloseMenu();
        }
        );
    }
}
