using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] private Transform dropPoint;

    [SerializeField] private PlayerLocal playerLocal;

    Animator animator;
    Inventory inventory;

    void Start()
    {
        animator = playerLocal.playerAnimator;
        inventory = PlayerGlobal.Singletone.inventory;
    }

    public void DropTool()
    {
        var id = inventory.Tool.itemInfo.id;
        Instantiate(Base.Singletone.prefabs[id], dropPoint.position, Quaternion.identity).GetComponent<Item>().CopyInfo(inventory.Tool);

        inventory.ClearTool();
    }
}
