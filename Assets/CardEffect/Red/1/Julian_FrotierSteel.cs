using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Julian_FrotierSteel : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("鍵開け", "Locktouch", new List<Cost>() { new TapCost() }, null, -1, false,card);
            activateClass.SetUpActivateClass((_hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine2()
            {
                bool check = false;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(new IShowLibraryCard(new List<CardSource>() { cardSource }, hashtable, false).ShowLibraryCard());

                    if (cardSource.PlayCost >= 3)
                    {
                        ActivateClass activateClass1 = new ActivateClass();
                        activateClass1.SetUpICardEffect("","", new List<Cost>() { new ReverseCost(1, (_cardSource) => true) }, null, -1, true,card);
                        activateClass1.SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                        IEnumerator ActivateCoroutine1()
                        {
                            yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                        }

                        if (activateClass1.CanUse(null))
                        {
                            check = true;
                            yield return ContinuousController.instance.StartCoroutine(activateClass1.Activate_Optional_Cost_Execute(null, "Do you pay cost?"));
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

            ActivateClass activateClass2 = new ActivateClass();
            activateClass2.SetUpICardEffect("財宝奪取", "Thief of Treasure", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass2.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass2);

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
