using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemHandler : MonoBehaviour 
{
    public Item item;

    //void OnTriggerEnter(Collider other)
    //{
    //    if (!other.gameObject.CompareTag("Player")) return;
    //    var interactManager = other.gameObject.GetComponent<Interact>();
    //    if (this != interactManager.InteractTarget) return;

    //    interactManager.InteractHandler(gameObject);
    //}
}

[Serializable]
public class Item
{
    [SerializeReference] public ItemSO itemInfo;

    [Header("Exact Info")]
    public int count = 1;

    [Header("Custom Variables")]
    public List<ItemCustomVariable> customVars = new();

    public void CopyInfo(Item source)
    {
        if (source?.itemInfo == null)
        {
            NullifyInfo();
            return;
        }
        itemInfo = source.itemInfo;
        count = source.count;

        customVars.Clear();
        foreach (var customVar in source.customVars)
        {
            customVars.Add(new ItemCustomVariable
            {
                name = customVar.name,
                value = customVar.value
            });
        }
    }

    public void NullifyInfo()
    {
        itemInfo = null;
        count = 0;
        customVars.Clear();
    }
}
