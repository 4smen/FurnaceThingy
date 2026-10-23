using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Тайминги")]
    [SerializeField] private float grabThreshold = 0.2f; // Короткое зажатие (0.2 сек)
    [SerializeField] private float holdThreshold = 0.6f; // Длинное зажатие (0.6 сек)

    [Header("Защита от сдвига")]
    // Если мышь сдвинулась дальше этого расстояния (в пикселях) до 0.6 сек, 
    // слайдер не включится. Заменяет нестабильный OnPointerExit.
    [SerializeField] private float dragTolerance = 25f;

    // Состояния мыши
    private bool isPointerDown = false;
    private float pointerDownTime;
    private Vector2 startMousePosition; // Позиция мыши в момент клика

    // Флаги фаз
    private bool isHoldingTriggered = false;     // Включился ли перенос за мышкой?
    private bool isLongHoldingTriggered = false; // Включился ли ползунок?
    private bool canTriggerLongHold = true;      // Разрешено ли активировать длинное зажатие?

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        if (isPointerDown)
        {
            float holdDuration = Time.time - pointerDownTime;

            // Динамическая проверка сдвига мыши вместо OnPointerExit
            if (canTriggerLongHold && !isLongHoldingTriggered)
            {
                float mouseMovedDistance = Vector2.Distance(Input.mousePosition, startMousePosition);
                if (mouseMovedDistance > dragTolerance)
                {
                    canTriggerLongHold = false;
                    Debug.Log("🔒 Мышь сдвинулась слишком далеко. Длинное зажатие заблокировано.");
                }
            }

            // 1. КОРОТКОЕ ЗАЖАТИЕ (0.2 сек)
            if (!isHoldingTriggered && holdDuration >= grabThreshold)
            {
                isHoldingTriggered = true;
                OnShortHoldStart();
            }

            // 2. ДЛИННОЕ ЗАЖАТИЕ (0.6 сек)
            if (isHoldingTriggered && !isLongHoldingTriggered && canTriggerLongHold && holdDuration >= holdThreshold)
            {
                isLongHoldingTriggered = true;
                OnLongHoldStart();
            }
        }

        // Если короткое зажатие пошло — картинка летит за мышкой
        if (isHoldingTriggered)
        {
            FollowCursor();
        }

        // Если длинное зажатие активно — обновляем слайдер от мыши
        if (isLongHoldingTriggered)
        {
            OnLongHoldUpdate();
        }
    }

    // --- КЛИКИ И ОТПУСКАНИЕ ---

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPointerDown = true;
        pointerDownTime = Time.time;
        startMousePosition = Input.mousePosition; // Запоминаем стартовую точку мыши

        // Запоминаем родной слот инвентаря ДО любых манипуляций
        originalParent = transform.parent;

        // Сбрасываем флаги для нового нажатия
        isHoldingTriggered = false;
        isLongHoldingTriggered = false;
        canTriggerLongHold = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!isPointerDown) return;

        isPointerDown = false;

        // Возвращаем физику лучей UI и полную видимость
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (isLongHoldingTriggered)
        {
            OnLongHoldEnd(eventData);
        }
        else if (isHoldingTriggered)
        {
            OnShortHoldEnd(eventData);
        }
        else
        {
            // На случай, если игрок отпустил мышь раньше 0.2 секунд
            ReturnToOriginalSlot();
        }
    }

    // --- ЛОГИКА ПОВЕДЕНИЯ ---

    private void OnShortHoldStart()
    {
        Debug.Log("⏱️ Короткое зажатие! Предмет взят.");

        originalParent = transform.parent;

        // ИСПРАВЛЕНИЕ МАСКИ: Находим самый корень UI (обычно это Canvas)
        // Чтобы объект гарантированно отрисовывался ПОВЕРХ инвентаря и его масок,
        // мы перемещаем его на самый верхний Canvas и делаем его ПОСЛЕДНИМ в списке дочерних объектов.
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null)
        {
            transform.SetParent(rootCanvas.transform);
            transform.SetAsLastSibling(); // Это выводит объект на самый передний план в Unity UI
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    private void OnLongHoldStart()
    {
        Debug.Log("🔥 Длинное зажатие АКТИВИРОВАНО! (Показываем ползунок)");
        // Здесь будет: sliderUI.Activate(...);
    }

    private void OnLongHoldUpdate()
    {
        // Здесь будет: sliderUI.UpdateValueByMouse();
    }

    private void OnShortHoldEnd(PointerEventData eventData)
    {
        Debug.Log("🛑 Отпустили после обычного переноса. Возврат на место.");
        ReturnToOriginalSlot();
    }

    private void OnLongHoldEnd(PointerEventData eventData)
    {
        Debug.Log("❌ Отпустили после слайдера. Возврат на место.");

        // 1. Закрываем UI ползунка: sliderUI.Deactivate();

        // 2. Возвращаем оригинальную иконку на место
        ReturnToOriginalSlot();

        // 3. ЗДЕСЬ в будущем мы будем спавнить "фантомную" пачку предметов 
        // с выбранным количеством, которая останется на курсоре для переноса в новый слот.
    }

    // Универсальный метод возврата картинки в родную ячейку сетки
    private void ReturnToOriginalSlot()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            //rectTransform.anchoredPosition = Vector2.zero; // Центрируем обратно в Grid Layout
        }
    }

    private void FollowCursor()
    {
        if (canvas == null) return;

        // ИСПРАВЛЕНИЕ БЕШЕНОГО ПЕРЕМЕЩЕНИЯ:
        // Проверяем режим работы вашего Canvas. 
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Для режима Overlay (самый частый) координаты мыши напрямую подходят под anchoredPosition,
            // но их нужно разделить на scaleFactor, чтобы учесть масштабирование экрана (например, 4K или FullHD).
            rectTransform.anchoredPosition = (Vector2)Input.mousePosition / canvas.scaleFactor;
        }
        else
        {
            // Для режимов Screen Space - Camera или World Space используем стандартный математический перевод
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out localMousePos
            );
            rectTransform.anchoredPosition = localMousePos;
        }
    }

}
