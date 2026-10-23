using System.Collections;
using UnityEngine;

public class BaseMechanics : MonoBehaviour
{
    [SerializeField] private GameObject cuttingPlane;
    Rigidbody cuttingPlaneRB;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform cameraHolder;

    [SerializeField] private PlayerLocal playerLocal;

    Animator animator;
    Inventory inventory;

    void Start()
    {
        cuttingPlaneRB = cuttingPlane.GetComponent<Rigidbody>();

        animator = playerLocal.playerAnimator;
        inventory = PlayerGlobal.Singletone.inventory;
    }

    void Update()
    {
        if (Input.GetKeyDown(Controls.Singletone.useKeys[0]) && inventory.UsableInventory[0].itemInfo.type != Base.ItemTypes.None)
        {
            if (inventory.RemoveUsableItem(0))
            {
                print("поймали'с");
                ProcessUsableItemMechanic(inventory.UsableInventory[0]);
                inventory.CheckOnUsableCount(0);
            }
        }
        if (Input.GetKeyDown(Controls.Singletone.useKeys[1]) && inventory.UsableInventory[1].itemInfo.type != Base.ItemTypes.None)
        {
            if (inventory.RemoveUsableItem(1))
            {
                print("поймали'с");
                ProcessUsableItemMechanic(inventory.UsableInventory[1]);
                inventory.CheckOnUsableCount(1);
            }
        }
        if (Input.GetKeyDown(Controls.Singletone.useKeys[2]) && inventory.UsableInventory[2].itemInfo.type != Base.ItemTypes.None)
        {
            if (inventory.RemoveUsableItem(2))
            {
                print("поймали'с");
                ProcessUsableItemMechanic(inventory.UsableInventory[2]);
                inventory.CheckOnUsableCount(2);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (inventory.RemoveTool())
            {
                animator.SetTrigger("attack");
                inventory.CheckOnToolCount();
            }
            //добавить нормальную анимацию
        }
    }

    public void ProcessToolMechanic()
    {
        switch (inventory.Tool.itemInfo.id)
        {
            case 5:
                cuttingPlaneRB.isKinematic = true;
                cuttingPlane.transform.position = origin.position;
                cuttingPlane.transform.rotation = origin.rotation;
                cuttingPlaneRB.isKinematic = false;
                cuttingPlaneRB.AddForce(origin.forward * 0.001f);
                break;
        }
    }

    void ProcessUsableItemMechanic(Item item)
    {
        switch (item.itemInfo.id)
        {
            case 3:
                StartCoroutine(BoosterEffect());
                break;
        }
    }

    IEnumerator BoosterEffect()
    {
        var prev = playerLocal.playerMovement.moveSpeed;
        playerLocal.playerMovement.moveSpeed = 15f;
        print("бесюкаемся");
        yield return new WaitForSeconds(5);
        playerLocal.playerMovement.moveSpeed = prev;
        print("не бесюкаемся(");
        yield break;
    }
}
