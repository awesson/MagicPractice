using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool IsValidDragTarget { get; set; } = true;
    public bool IsValidDropTarget { get; set; } = true;

    [SerializeField]
    private UnityEvent<DragAndDrop> m_OnDragStarted = default;
    [SerializeField]
    private UnityEvent<DragAndDrop> m_OnDragEnded = default;
    [SerializeField]
    private UnityEvent<DragAndDrop, DragAndDrop> m_OnDroppedOnto = default;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            transform.position += (Vector3)eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var overList = eventData.hovered;
        for (int i = 0; i < overList.Count; ++i)
        {
            var posibleDropTarget = overList[i].GetComponent<DragAndDrop>();
            if (posibleDropTarget != null
                && posibleDropTarget != this
                && posibleDropTarget.IsValidDropTarget)
            {
                // TODO: Handle if we are over more than one drop target?
                posibleDropTarget.m_OnDroppedOnto?.Invoke(this, posibleDropTarget);
                break;
            }
        }

        m_OnDragEnded?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_OnDragStarted?.Invoke(this);
    }
}
