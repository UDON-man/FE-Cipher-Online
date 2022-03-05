using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Coffee.UIEffects;
public class Draggable_Card : Draggable
{
    public CEntity_Base cEntity_Base { get; set; }
    public Transform DefaultParent;
    public Image CardImage;
    public UnityAction<List<DropArea>> OnDragAction; 
    public UnityAction<List<DropArea>> OnDropAction;

    public bool IsDragging { get; set; }

    public void OffDraggable_Card()
    {
        this.gameObject.SetActive(false);
    }

    public void SetUpDraggable_Card(CEntity_Base _cEntity_Base,Vector3 StartPosition)
    {
        this.transform.localScale = new Vector3(1, 1, 1);

        this.cEntity_Base = _cEntity_Base;

        //CardImage.color = new Color(1, 1, 1, 0.8f);

        CardImage.sprite = _cEntity_Base.CardImage;

        this.gameObject.SetActive(true);

        this.transform.SetParent(DefaultParent);

        this.transform.position = StartPosition;

        IsDragging = true;

        OnBeginDrag(null);
    }


    public override void OnDrag(BaseEventData eventData)
    {
        if (GetRaycastArea((PointerEventData)eventData) != null)
        {
            OnDragAction?.Invoke(GetRaycastArea((PointerEventData)eventData));
        }

        base.OnDrag(eventData);
    }

    public override void OnEndDrag(BaseEventData eventData)
    {
        if(!IsDragging)
        {
            return;
        }

        IsDragging = false;

        this.transform.SetParent(DefaultParent);

        OnDropAction?.Invoke(GetRaycastArea((PointerEventData)eventData));

        base.OnEndDrag(eventData);
    }

    private void Update()
    {
        if (IsDragging)
        {
            OnDrag(null);

            if(Input.GetMouseButtonUp(0))
            {
                if (this == null)
                {
                    return;
                }

                OnEndDrag(null);
            }
        }
    }
}
