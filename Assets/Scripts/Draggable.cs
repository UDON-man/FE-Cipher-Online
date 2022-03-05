using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour
{
    private Transform root;
    [HideInInspector] public Transform area;
    [HideInInspector] public Transform self;
    public CanvasGroup canvasGroup = null;

    public int oldChildIndex { get; set; }
    public Transform oldParent { get; set; }

   

    

    public void Awake()
    {
        this.self = this.transform;
        this.area = this.self.parent;
        this.root = this.area.parent;
    }

    public virtual void OnPointerEnter()
    {

    }

    public virtual void OnPointerExit()
    {

    }

    public virtual void OnBeginDrag(BaseEventData eventData)
    {
        /*
        // ドラッグできるよういったん子の中の上位に移動する
        oldChildIndex = this.transform.GetSiblingIndex();
        oldParent = this.transform.parent;

        if (oldParent != null)
        {
            if (oldParent.GetComponent<GridLayoutGroup>() != null)
            {
                oldParent.GetComponent<GridLayoutGroup>().enabled = false;
            }
        }

        this.transform.SetSiblingIndex(this.transform.parent.childCount - 1);

        //this.canvasGroup.blocksRaycasts = false;
        //this.self.SetParent(this.root);
        */
    }

    public virtual void OnDrag(BaseEventData eventData)
    {
        /*
        if(eventData != null)
        {
            this.self.localPosition = GetLocalPosition(((PointerEventData)eventData).position, this.transform);
        }

        else
        {
            this.self.localPosition = GetLocalPosition(Input.mousePosition, this.transform);
        }
        */

        this.self.localPosition = GetLocalPosition(Input.mousePosition, this.transform);

    }

    public static Vector3 GetLocalPosition(Vector3 position, Transform transform)
    {
        Camera MainCamera = null;

        if(GManager.instance != null)
        {
            MainCamera = GManager.instance.camara;
        }

        else if(Opening.instance != null)
        {
            MainCamera = Opening.instance.MainCamera;
        }

        if(MainCamera != null)
        {
            // 画面上の座標 (Screen Point) を RectTransform 上のローカル座標に変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                position,
                //Camera.main,
                MainCamera,
                out var result);
            return new Vector3(result.x, result.y, 0);
        }

        return Vector3.zero;
        
    }

    public virtual void OnEndDrag(BaseEventData eventData)
    {
        
        if(this != null)
        {
            if(this.canvasGroup != null)
            {
                //修正後
                // UI 機能を一時的無効化
                this.canvasGroup.blocksRaycasts = false;

                // UI 機能を復元
                this.canvasGroup.blocksRaycasts = true;

                if (GetRaycastArea((PointerEventData)eventData) != null)
                {
                    if (GetRaycastArea((PointerEventData)eventData).Count == 0)
                    {
                        ReturnDefaultPosition();
                    }
                }

                if (oldParent != null)
                {
                    if (oldParent.GetComponent<GridLayoutGroup>() != null)
                    {
                        oldParent.GetComponent<GridLayoutGroup>().enabled = true;
                    }
                }
            }
        }
       
    }

    public void ReturnDefaultPosition()
    {
        if(oldParent != null)
        {
            this.transform.SetParent(oldParent);
        }
        
        this.transform.SetSiblingIndex(oldChildIndex);
    }

    /// <summary>
    /// イベント発生地点の DropArea を取得する
    /// </summary>
    /// <param name="eventData">イベントデータ</param>
    /// <returns>DropArea</returns>
    public static List<DropArea> GetRaycastArea(PointerEventData eventData)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);

        List<RaycastResult> results = new List<RaycastResult>();
        // マウスポインタの位置にレイ飛ばし、ヒットしたものを保存
        pointer.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointer, results);

        List<DropArea> DropAreas = new List<DropArea>();

        // ヒットしたUIの名前
        foreach (RaycastResult target in results)
        {
            if(target.gameObject.GetComponent<DropArea>() != null)
            {
                DropAreas.Add(target.gameObject.GetComponent<DropArea>());
            }
        }

        return DropAreas;

    }
}