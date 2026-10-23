using UnityEngine;
using UnityEngine.EventSystems;

public class DropCollectionTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject collectBox;

    bool grabbed = false;
    bool hovered = false;

    void OnEnable()
    {
        UIManager.OnInventoryCallback += ToggleBoxState;
    }

    void OnDestroy()
    {
        UIManager.OnInventoryCallback -= ToggleBoxState;
    }

    void Update()
    {
        if (hovered) collectBox.transform.position = Input.mousePosition;
    }

    void ToggleBoxState(bool state)
    {
        Cursor.visible = !state;
        hovered = state;
        collectBox.SetActive(state);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!grabbed) ToggleBoxState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!grabbed) ToggleBoxState(false);
    }
}
