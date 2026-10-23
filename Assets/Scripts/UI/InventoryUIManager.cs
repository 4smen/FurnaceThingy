using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private ItemUIElement toolSlot;
    [SerializeField] private List<ItemUIElement> usableSlots;

    [SerializeField] private ItemUIElement inventoryToolSlot;
    [SerializeField] private List<ItemUIElement> inventoryUsableSlots;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject itemSlotPrefab;

    [SerializeField] private DropCollectionTrigger dropTrigger;

    Drop drop;
    Inventory inventory;

    void OnEnable()
    {
        Inventory.OnHotbarItemChangedCallback += UpdateHotbarUI;

        Inventory.OnItemChangedCallback += UpdateInventoryUI;
        Inventory.OnItemAddedCallback += UpdateInventoryUI;
        Inventory.OnItemRemovedCallback += UpdateInventoryUI;
    }

    void OnDestroy()
    {
        Inventory.OnHotbarItemChangedCallback -= UpdateHotbarUI;

        Inventory.OnItemChangedCallback -= UpdateInventoryUI;
        Inventory.OnItemAddedCallback -= UpdateInventoryUI;
        Inventory.OnItemRemovedCallback -= UpdateInventoryUI;
    }

    void Start()
    {
        inventory = PlayerGlobal.Singletone.inventory;
        drop = PlayerLocal.Singletone.playerDrop;

        UpdateHotbarUI();
    }

    public void UpdateHotbarUI()
    {
        var tool = inventory.Tool;
        var usableInventory = inventory.UsableInventory;

        for (int i = 0; i < usableSlots.Count && i < usableInventory.Count; i++)
        {
            if (usableInventory[i].itemInfo != null)
            {
                usableSlots[i].image.enabled = true;
                usableSlots[i].image.sprite = usableInventory[i].itemInfo.icon;

                if (usableInventory[i].customVars.Exists(vr => vr != null && vr.name == "usesLeft"))
                {
                    var index = usableInventory[i].customVars.FindIndex(vr => vr != null && vr.name == "usesLeft");
                    usableSlots[i].usesLeftBar.enabled = true;
                    usableSlots[i].usesLeftBar.fillAmount = usableInventory[i].customVars[index].value / (float)usableInventory[i].itemInfo.defaultCustomVars[index].value;

                    usableSlots[i].count.enabled = false;
                }
                else
                {
                    usableSlots[i].count.enabled = true;
                    usableSlots[i].count.text = usableInventory[i].count.ToString();
                }
            }
            else
            {
                usableSlots[i].image.enabled = false;
                usableSlots[i].usesLeftBar.enabled = false;
                usableSlots[i].count.enabled = false;
            }
        }

        if (tool.itemInfo != null)
        {
            toolSlot.image.enabled = true;
            toolSlot.image.sprite = tool.itemInfo.icon;

            if (tool.customVars.Exists(vr => vr != null && vr.name == "durability"))
            {
                var index = tool.customVars.FindIndex(vr => vr != null && vr.name == "durability");
                toolSlot.usesLeftBar.enabled = true;
                toolSlot.usesLeftBar.fillAmount = tool.customVars[index].value / (float)tool.itemInfo.defaultCustomVars[index].value;

                toolSlot.count.enabled = false;
            }
            else
            {
                toolSlot.count.enabled = true;
                toolSlot.count.text = tool.count.ToString();
            }
        }
        else
        {
            toolSlot.image.enabled = false;
            toolSlot.usesLeftBar.enabled = false;
            toolSlot.count.enabled = false;
        }
    }

    public void UpdateInventoryUI()
    {
        for (int i = gridContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(gridContainer.GetChild(i).gameObject);
        }

        foreach (var item in inventory.MainInventory)
        {
            GameObject slot = Instantiate(itemSlotPrefab, gridContainer);
            slot.GetComponentInChildren<Image>().sprite = item.itemInfo.icon;
            slot.GetComponentInChildren<TextMeshProUGUI>().text = item.count.ToString();

            slot.name = inventory.MainInventory.IndexOf(item).ToString();
        }

        var tool = inventory.Tool;
        var usableInventory = inventory.UsableInventory;

        for (int i = 0; i < inventoryUsableSlots.Count && i < usableInventory.Count; i++)
        {
            if (usableInventory[i].itemInfo != null)
            {
                inventoryUsableSlots[i].image.enabled = true;
                inventoryUsableSlots[i].image.sprite = usableInventory[i].itemInfo.icon;

                if (usableInventory[i].customVars.Exists(vr => vr != null && vr.name == "usesLeft"))
                {
                    var index = usableInventory[i].customVars.FindIndex(vr => vr != null && vr.name == "usesLeft");
                    inventoryUsableSlots[i].usesLeftBar.enabled = true;
                    inventoryUsableSlots[i].usesLeftBar.fillAmount = usableInventory[i].customVars[index].value / (float)usableInventory[i].itemInfo.defaultCustomVars[index].value;

                    inventoryUsableSlots[i].count.enabled = false;
                }
                else
                {
                    inventoryUsableSlots[i].count.enabled = true;
                    inventoryUsableSlots[i].count.text = usableInventory[i].count.ToString();
                }
            }
            else
            {
                inventoryUsableSlots[i].image.enabled = false;
                inventoryUsableSlots[i].usesLeftBar.enabled = false;
                inventoryUsableSlots[i].count.enabled = false;
            }
        }

        if (tool.itemInfo != null)
        {
            inventoryToolSlot.image.enabled = true;
            inventoryToolSlot.image.sprite = tool.itemInfo.icon;

            if (tool.customVars.Exists(vr => vr != null && vr.name == "durability"))
            {
                var index = tool.customVars.FindIndex(vr => vr != null && vr.name == "durability");
                inventoryToolSlot.usesLeftBar.enabled = true;
                inventoryToolSlot.usesLeftBar.fillAmount = tool.customVars[index].value / (float)tool.itemInfo.defaultCustomVars[index].value;

                inventoryToolSlot.count.enabled = false;
            }
            else
            {
                inventoryToolSlot.count.enabled = true;
                inventoryToolSlot.count.text = tool.count.ToString();
            }
        }
        else
        {
            inventoryToolSlot.image.enabled = false;
            inventoryToolSlot.usesLeftBar.enabled = false;
            inventoryToolSlot.count.enabled = false;
        }
    }
}

[Serializable]
internal class ItemUIElement
{
    public Image image;
    public Image usesLeftBar;
    public TextMeshProUGUI count;
}
