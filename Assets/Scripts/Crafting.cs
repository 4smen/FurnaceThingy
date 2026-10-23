using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class Crafting : MonoBehaviour
{
    [SerializeField] private PlayerGlobal playerGlobal;

    Inventory inventory;

    public static event Action ItemCraftedCallback;

    void Start()
    {
        inventory = playerGlobal.inventory;
    }

    public void ChooseRecipe()
    {

    }

    public bool CanCraft(RecipeSO targetedRecipe)
    {
        bool success = true;
        foreach (var ig in targetedRecipe.ingredients)
        {
            var inventoryIg = inventory.MainInventory.Find(vr => vr.itemInfo.id == ig.id);
            if (ig.count > inventoryIg.count)
            {
                //добавить чтобы текст количества ингредиентов менялся на красный
                success = false;
            }
        }

        return success;
    }

    public void Craft(RecipeSO targetedRecipe)
    {
        if (!CanCraft(targetedRecipe))
        {
            Warnings.Singletone.RaiseWarning("NotEnoughIngredients");
            return;
        }

        foreach (var ig in targetedRecipe.ingredients)
        {
            var inventoryIg = inventory.MainInventory.Find(vr => vr.itemInfo.id == ig.id);
            inventory.RemoveInventoryItem(ig.id, ig.count);
        }

        switch (targetedRecipe.resultType)
        {
            case Base.ItemTypes.Material:

                break;
            case Base.ItemTypes.Usable:

                break;
        }

        ItemCraftedCallback.Invoke();
    }
}
