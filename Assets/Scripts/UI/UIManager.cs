using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject craftingMenu;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject hotbar;

    [SerializeField] private bool inventory = false;
    [SerializeField] private bool paused = false;

    [SerializeField] private InventoryUIManager inventoryUIManager;

    PlayerMovement playerMovement;
    PlayerCam playerCam;
    BaseMechanics baseMechanics;

    public static event Action<bool> OnInventoryCallback;
    public static event Action<bool> OnPauseCallback;

    void Start()
    {
        playerMovement = PlayerLocal.Singletone.playerMovement;
        playerCam = PlayerLocal.Singletone.playerCam;
        baseMechanics = PlayerLocal.Singletone.playerBaseMechanics;
    }


    public void ToggleCraftingMenu(bool state)
    {
        craftingMenu.SetActive(state);

        LobotomizePlayer(state);
    }

    public void ToggleInventoryMenu(bool state)
    {
        inventoryMenu.SetActive(state);
        hotbar.SetActive(!state);

        LobotomizePlayer(state);

        if (!inventory) OnInventoryCallback?.Invoke(state);
    }

    public void TogglePauseMenu(bool state)
    {
        pauseMenu.SetActive(state);

        LobotomizePlayer(state);

        if (paused) OnPauseCallback?.Invoke(state);
    }

    public void LobotomizePlayer(bool state)
    {
        playerMovement.enabled = !state;
        playerCam.enabled = !state;
        baseMechanics.enabled = !state;

        Cursor.visible = state;
        if (state) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if (Input.GetKeyDown(Controls.Singletone.inventoryKey)) 
        {
            if (paused) return;

            inventory = !inventory;
            ToggleInventoryMenu(inventory);

            if (inventory) inventoryUIManager.UpdateInventoryUI();
        }
        if (Input.GetKeyDown(Controls.Singletone.pauseKey))
        {
            if (inventory)
            {
                inventory = false;
                ToggleInventoryMenu(inventory);
            }

            paused = !paused;
            TogglePauseMenu(paused);
        }
    }
}
