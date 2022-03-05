using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{
	//このDropAreaが対象のオブジェクトの子になっているか
   public bool IsChild(GameObject Parent)
   {
		List<GameObject> list = GetAllChildren.GetAll(Parent);

		list.Add(Parent);

		foreach (GameObject child in list)
        {
            if(child == this.gameObject)
            {
                return true;
            }
        }

        return false;
   }

	[Header("カードドロップパネル")]
	public GameObject DropPanel;

	[Header("オンポインターパネル")]
	public GameObject OnPointerPanel;

	private void Start()
	{
		if(DropPanel != null)
		{
			DropPanel.SetActive(false);
		}
		
		if(OnPointerPanel != null)
		{
			OnPointerPanel.SetActive(false);
		}
	}

	public void OnDropPanel()
	{
		if (DropPanel != null)
		{
			DropPanel.SetActive(true);
		}

		if (OnPointerPanel != null)
		{
			OnPointerPanel.SetActive(false);
		}
	}

	public void OffDropPanel()
	{
		if (DropPanel != null)
		{
			DropPanel.SetActive(false);
		}
	}

	public void OnPointerEnter()
	{
		if (OnPointerPanel != null)
		{
			OnPointerPanel.SetActive(true);
		}
	}

	public void OnPointerExit()
	{
		if (OnPointerPanel != null)
		{
			OnPointerPanel.SetActive(false);
		}
	}
}

public static class GetAllChildren
{
	public static List<GameObject> GetAll(this GameObject obj)
	{
		List<GameObject> allChildren = new List<GameObject>();
		GetChildren(obj, ref allChildren);
		return allChildren;
	}

	//子要素を取得してリストに追加
	public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
	{
		Transform children = obj.GetComponentInChildren<Transform>();
		//子要素がいなければ終了
		if (children.childCount == 0)
		{
			return;
		}
		foreach (Transform ob in children)
		{
			allChildren.Add(ob.gameObject);
			GetChildren(ob.gameObject, ref allChildren);
		}
	}
}
