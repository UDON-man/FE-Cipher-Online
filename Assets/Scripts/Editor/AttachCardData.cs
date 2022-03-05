using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon;
using Photon.Pun;
using System.Linq;

public class AttachCardData : MonoBehaviour
{
	[MenuItem("Window/Attach/AttachCardData")]
	static void Attach_CardData()
	{
		List<CEntity_Base> List = GetAsset.LoadAll<CEntity_Base>("Assets/CardEntity/");

		foreach (GameObject obj in Selection.gameObjects)
		{
			if (obj.GetComponent<ContinuousController>() != null)
			{
				ContinuousController CCtrl = obj.GetComponent<ContinuousController>();

				CCtrl.CardList = new List<CEntity_Base>();

				foreach(CEntity_Base cEntity_Base in List)
				{
					bool HasEffect = false;

					if(!string.IsNullOrEmpty(cEntity_Base.ClassName))
					{
						Type t = null;

						t = Type.GetType(cEntity_Base.ClassName);

						if (t != null)
						{
							HasEffect = true;
						}
					}

					CCtrl.CardList.Add(cEntity_Base);
					continue;
					if (HasEffect)
					{
						
					}

					else
					{
						Debug.Log($"{cEntity_Base.CardName}の効果は未実装");
					}
				}

				CCtrl.CardList = DeckData.SortedList(CCtrl.CardList);
				return;
			}
		}
	}

}