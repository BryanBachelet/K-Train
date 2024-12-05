using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDispatcher : MonoBehaviour
{
    public MarchandUiView marchandUiView;
    public GameObject fixeGameplayUI;
    public UI_Inventory uiInventory;
    public GuerhoubaGames.UI.DamageRecapUI damageRecap;
    public GuerhoubaGames.UI.DragManager dragManager;
    public GuerhoubaGames.UI.CristalUI cristalUI;
    public GuerhoubaGames.UI.AnvilUIView anvilUIView;
    public PauseMenu pauseMenu;
     public GameObject bandenoir;
    public void ActiveUIElement()
    {
        uiInventory.InitComponent();
        dragManager.StartDragManager();
    }


    public bool IsAnythingIsOpen()
    {
     

        bool anvilResult = false;
        if(anvilUIView.anvilBehavior != null)
        {
            anvilResult = anvilUIView.anvilBehavior.isOpen;
        }
        return marchandUiView.isOpen || anvilResult;
    }


    public bool CloseInventory()
    {
        if (uiInventory.isOpen)
        {
            uiInventory.DeactivateInventoryInterface();
            return true;
        }
        return false;
    }
}
