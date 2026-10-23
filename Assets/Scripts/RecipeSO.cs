using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]

public class RecipeSO : ScriptableObject
{
    [Header("Main Info")]
    public bool unlocked = false;
    public Base.CraftingLevels required_crafting_level;
    public int result_id;
    public Base.ItemTypes resultType;
    public RecipeIngredient[] ingredients;
}

[Serializable]
public struct RecipeIngredient
{
    public int id;
    public int count;
}
