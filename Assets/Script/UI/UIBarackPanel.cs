using UnityEngine;

public class UIBarackPanel : MonoBehaviour
{

    private IMenuStack menu_stack;

    public void Initilize(BarrackBuilding building, IMenuStack menu)
    {
        menu_stack = menu;
    }

    public void OpenTraining()
    {
        MenuData menuData = DataManager.Instance.GetMenuData(MenuType.Training);
        GameObject trainingMenu = Instantiate(menuData.MenuPrefab, menu_stack.GetUIRoot().transform);
        menu_stack.OpenMenu(trainingMenu.GetComponent<Lean.Gui.LeanWindow>());
    }

}
