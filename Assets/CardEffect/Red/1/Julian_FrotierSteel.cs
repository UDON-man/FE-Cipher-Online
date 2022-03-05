using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Julian_FrotierSteel : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("鍵開け", new List<Cost>() { new TapCost() }, null, -1, false);
            activateClass[1].SetUpActivateClass((_hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Locktouch";
            }

            IEnumerator ActivateCoroutine2()
            {
                bool check = false;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    //ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Library Card", false));

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[1]);
                    yield return ContinuousController.instance.StartCoroutine(new IShowLibraryCard(new List<CardSource>() { cardSource }, hashtable, false).ShowLibraryCard());

                    if (cardSource.PlayCost >= 3)
                    {
                        if (activateClass[2] != null)
                        {
                            activateClass[2].SetUpICardEffect("", new List<Cost>() { new ReverseCost(1, (_cardSource) => true) }, null, -1, true);
                            activateClass[2].SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                            IEnumerator ActivateCoroutine1()
                            {
                                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                            }

                            if (activateClass[2].CanUse(null))
                            {
                                check = true;
                                yield return ContinuousController.instance.StartCoroutine(activateClass[2].Activate_Optional_Cost_Execute(null, "Do you pay cost?"));
                            }
                        }
                    }

                    if (!check)
                    {
                        yield return new WaitForSeconds(1.5f);
                    }

                    yield return new WaitForSeconds(0.5f);
                    GManager.instance.GetComponent<Effects>().OffShowCard();
                }
            }


            activateClass[0].SetUpICardEffect("財宝奪取", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Thief of Treasure";
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    yield return ContinuousController.instance.StartCoroutine(cardSource.cardOperation.DiscardFromLibrary());
                }
            }
        }

        return cardEffects;
    }
}
