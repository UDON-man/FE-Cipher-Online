using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class CommandButton : MonoBehaviour, IPointerClickHandler
{
    public UnityAction OnClickAction;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickAction?.Invoke();
    }
}
