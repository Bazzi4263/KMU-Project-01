using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerEnterHandler, IEndDragHandler, IBeginDragHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEnterHandler = null;
    public Action<PointerEventData> EndDragHandler = null;
    public Action<PointerEventData> BeginDragHandler = null;
    public Action<PointerEventData> ExitHandler = null;

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(OnEnterHandler != null)
            OnEnterHandler.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(EndDragHandler != null)
            EndDragHandler.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(BeginDragHandler != null)
            BeginDragHandler.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(ExitHandler != null)
            ExitHandler.Invoke(eventData);
    }
}
