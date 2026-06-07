using UnityEngine;

public class ClanPanelUI : MonoBehaviour
{

    public GameController controller;

    public MyClanDetailUI clanDetailUI;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void initialize(GameController controller)
    {
        this.controller = controller;
        clanDetailUI.initialize();
    }

    public void BackToMainMenu()
    {
        controller.CloseMenu();
    }
}
