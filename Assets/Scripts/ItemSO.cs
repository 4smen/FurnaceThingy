using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("Main Info")]
    public int id;
    public Base.ItemTypes type;
    public string itemName;
    public string description;
    public Sprite icon;

    [Header("Default Values")]
    public int defaultCount = 1;

    [Header("Default Custom Variables")]
    public List<ItemCustomVariable> defaultCustomVars = new();
}


[Serializable]
public class ItemCustomVariable
{
    public string name;
    public int value;
}
