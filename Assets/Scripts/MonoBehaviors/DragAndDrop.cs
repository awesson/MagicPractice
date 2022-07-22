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

    private bool m_Dragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsValidDragTarget)
        {
            return;
        }

        m_Dragging = true;
        m_OnDragStarted?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsValidDragTarget)
        {
            // Make sure to send the end event if we sent the start event,
            // even if it's not valid to drag anymore
            if (m_Dragging)
            {
                OnEndDrag(eventData);
            }
            return;
        }

        if (eventData.dragging)
        {
            // Make sure to send the start event if we were disabled at the time of the real start
            if (!m_Dragging)
            {
                OnBeginDrag(eventData);
            }

            transform.position += (Vector3)eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsValidDragTarget)
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
        }

        // Make sure to send the end event if we sent a start event,
        // even if it's not valid to drag anymore
        if (m_Dragging)
        {
            m_OnDragEnded?.Invoke(this);
            m_Dragging = false;
        }
    }
}
