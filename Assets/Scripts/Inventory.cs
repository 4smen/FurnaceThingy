using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;
using UnityEditorInternal;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Item tool;
    public Item Tool { get { return tool; } }

    [SerializeField] private List<Item> usableInventory = new();
    public List<Item> UsableInventory { get { return usableInventory; } }

    [SerializeField] private List<Item> mainInventory = new List<Item>();
    public List<Item> MainInventory { get { return mainInventory; } }


    [SerializeField] private int invCount;
    [SerializeField] private int limit = 300;

    public static event Action OnItemChangedCallback;

    public static event Action OnItemAddedCallback;
    public static event Action OnItemRemovedCallback;

    public static event Action OnHotbarItemChangedCallback;

    public void ClearTool()
    {
        tool.NullifyInfo();
        OnItemChangedCallback?.Invoke();
        OnItemRemovedCallback?.Invoke();
    }

    void ClearUsableInventory()
    {
        foreach (var usableInfo in usableInventory)
        {
            usableInfo.NullifyInfo();
        }
        OnItemChangedCallback?.Invoke();
        OnItemRemovedCallback?.Invoke();
    }


    public bool AddItem(Item newInf) //вторичная инва объекта(кол-во и кастомки)
    {
        var newBase = newInf.itemInfo; //первичная инфа объекта(основа)

        switch (newBase.type)
        {
            case Base.ItemTypes.Material:
                if (invCount + newInf.count > limit)
                {
                    Warnings.Singletone.RaiseWarning("NotEnoughSpace");
                    return false;
                }

                var existingMaterial = mainInventory.Find(vr => vr.itemInfo != null && vr.itemInfo.id == newBase.id);
                if (existingMaterial != null) //если в инвентаре уже есть похожий материал
                {
                    existingMaterial.count += newInf.count;
                    invCount += newInf.count;

                    OnItemChangedCallback?.Invoke();
                }
                else //добавляем новый
                {
                    //изучение нового материала
                    //if (newBase.discovered == false) newBase.discovered = true;

                    Item newItem = new Item();
                    newItem.CopyInfo(newInf);

                    mainInventory.Add(newItem);

                    invCount += newInf.count;

                    OnItemAddedCallback?.Invoke();
                }

                break;
            case Base.ItemTypes.Tool:
                if (tool.itemInfo == null)
                {
                    tool.CopyInfo(newInf);

                    OnHotbarItemChangedCallback?.Invoke();
                    break;
                }

                if (invCount + newInf.count > limit)
                {
                    Warnings.Singletone.RaiseWarning("NotEnoughSpace");
                    return false;
                }

                var existingTool = mainInventory.Find(vr => vr.itemInfo != null && vr.itemInfo.id == newBase.id);

                if (existingTool != null && existingTool.customVars != null && existingTool.customVars.Exists(vr => vr != null && vr.name == "stackable"))
                {
                    existingTool.count += newInf.count;
                    invCount += newInf.count;

                    OnItemChangedCallback?.Invoke();
                }
                else
                {
                    Item newItem = new Item();
                    newItem.CopyInfo(newInf);

                    mainInventory.Add(newItem);

                    invCount += newInf.count;

                    OnItemAddedCallback?.Invoke();
                }

                break;
            case Base.ItemTypes.Usable:
                var existingUsable = usableInventory.Find(vr => vr.itemInfo != null && vr.itemInfo.id == newBase.id);
                if (existingUsable != null && existingUsable.customVars != null && existingUsable.customVars.Exists(vr => vr != null && vr.name == "stackable")) //когда стакабл и есть в хотбаре
                {
                    existingUsable.count += newInf.count;

                    OnHotbarItemChangedCallback?.Invoke();
                    break;
                }

                foreach (var usable in usableInventory) //когда не стакабл и новый
                {
                    if (usable.itemInfo == null)
                    {
                        usable.CopyInfo(newInf);

                        print("нови предмет");
                        OnHotbarItemChangedCallback?.Invoke();
                        return true;
                    }
                }

                var existingUsableInInventory = mainInventory.Find(vr => vr.itemInfo != null && vr.itemInfo.id == newBase.id);
                if (existingUsableInInventory != null && existingUsableInInventory.customVars != null && existingUsableInInventory.customVars.Exists(vr => vr != null && vr.name == "stackable"))
                {
                    existingUsableInInventory.count += newInf.count;
                    invCount += newInf.count;

                    OnItemChangedCallback?.Invoke();
                }
                else
                {
                    Item newItem = new Item();
                    newItem.CopyInfo(newInf);

                    mainInventory.Add(newItem);

                    invCount += newInf.count;

                    OnItemAddedCallback?.Invoke();
                }
                break;
        }
        print("нови предмет");
        return true;
    }


    public void RemoveInventoryItem(int id, int count)
    {
        var item = mainInventory.Find(vr => vr.itemInfo != null && vr.itemInfo.id == id);
        if (item.count - count <= 0)
        {
            invCount -= item.count;
            mainInventory.Remove(item);
            OnItemRemovedCallback?.Invoke();
        }
        else
        {
            item.count -= count;
            invCount -= count;
        }

        OnItemChangedCallback?.Invoke();
    }

    public bool RemoveTool()
    {
        if (tool.itemInfo == null) return false;

        int countOfUses = 1;

        var countOfUsesVar = tool.customVars.Find(vr => vr != null && vr.name == "useCount");
        if (countOfUsesVar != null)
        {
            countOfUses = countOfUsesVar.value;
        }

        var durabilityVar = tool.customVars.Find(vr => vr != null && vr.name == "durability");
        if (durabilityVar != null)
        {
            print("кажеца есть прочность");

            if (durabilityVar.value - countOfUses < 0)
            {
                Warnings.Singletone.RaiseWarning("NotEnoughDurailityOnTool");
                return false;
            }
            else
            {
                durabilityVar.value -= countOfUses;
            }
        }
        else if (!tool.customVars.Exists(vr => vr != null && vr.name == "unbreakable"))
        {
            print("кажеца всё как обычно");
            if (tool.count - countOfUses < 0)
            {
                Warnings.Singletone.RaiseWarning("NotEnoughTools");
                return false;
            }
            else
            {
                tool.count -= countOfUses;
            }
        }
        else return true;

        return true;
    }

    public bool RemoveUsableItem(int index)
    {
        if (index != Math.Clamp(index, 0, usableInventory.Count) || usableInventory[index].itemInfo == null) return false;

        int countOfUses = 1;

        if (usableInventory[index].customVars.Exists(vr => vr != null && vr.name == "useCount"))
        {
            countOfUses = usableInventory[index].customVars.Find(vr => vr != null && vr.name == "useCount").value;
        }

        if (usableInventory[index].customVars.Exists(vr => vr != null && vr.name == "usesLeft"))
        {
            print("кажеца есть кол-во юзов");
            var usesLeftVar = usableInventory[index].customVars.Find(vr => vr != null && vr.name == "usesLeft");

            if (usesLeftVar.value - countOfUses < 0)
            {
                Warnings.Singletone.RaiseWarning("NotEnoughUsesOnUsableItem");
                return false;
            }
            else
            {
                usesLeftVar.value -= countOfUses;
            }
        }
        else
        {
            print("кажеца всё как обычно");
            if (usableInventory[index].count - countOfUses < 0)
            {
                Warnings.Singletone.RaiseWarning("NotEnoughUsableItem");
                return false;
            }
            else
            {
                usableInventory[index].count -= countOfUses;
            }
        }

        return true;
    }


    public void CheckOnToolCount()
    {
        var durabilityVar = tool.customVars.Find(vr => vr != null && vr.name == "durability");
        if (durabilityVar != null)
        {
            if (durabilityVar.value == 0)
            {
                tool.NullifyInfo();
                OnItemRemovedCallback?.Invoke();
            }
        }
        else
        {
            if (tool.count == 0)
            {
                tool.NullifyInfo();
                OnItemRemovedCallback?.Invoke();
            }
        }

        OnHotbarItemChangedCallback?.Invoke();
    }

    public void CheckOnUsableCount(int index)
    {
        if (index != Math.Clamp(index, 0, usableInventory.Count) || usableInventory[index] == null) return;

        var usesLeftVar = usableInventory[index].customVars.Find(vr => vr != null && vr.name == "usesLeft");
        if (usesLeftVar != null)
        {
            if (usesLeftVar.value == 0)
            {
                usableInventory[index].NullifyInfo();
                OnItemRemovedCallback?.Invoke();
            }
        }
        else
        {
            if (usableInventory[index].count == 0)
            {
                usableInventory[index].NullifyInfo();
                OnItemRemovedCallback?.Invoke();
            }
        }

        OnHotbarItemChangedCallback?.Invoke();
    }
}
