using UnityEngine;
using UnityEngine.EventSystems;

public class TileDeletionArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool _pointerInDeletionArea;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointerInDeletionArea = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointerInDeletionArea = false;
    }

    public bool GetPointerIsInDeletionArea()
    {
        return _pointerInDeletionArea;
    }
}
