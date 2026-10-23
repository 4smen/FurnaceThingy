using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCSO", menuName = "Scriptable Objects/NPCSO")]
public class NPCSO : ScriptableObject
{
    [Header("Main Info")]
    public string npcName;
    public int maxHP;

    [Header("Bestiary")]
    public string npcDescription;
    public Sprite npcIcon;

    [Header("Combat")]
    public int damage;
    public int attackSpeed;
    public int attackRange;
    public float attackCooldown;


    [Header("Default NPC Custom Variables")]
    public List<NPCCustomVariable> defaultCustomVars = new();
}


[Serializable]
public class NPCCustomVariable
{
    public string name;
    public int value;
}
