using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Forca_SuddenleySpawnInfoShop : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("高価な報告書", new List<Cost>() { new ReverseCost(1, (_cardSource) => true) }, null, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Expensive Report";
            }

            IEnumerator ActivateCoroutine()
            {
                bool check = false;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[0]);
                    yield return ContinuousController.instance.StartCoroutine(new IShowLibraryCard(new List<CardSource>() { cardSource }, hashtable, false).ShowLibraryCard());

                    if (cardSource.PlayCost >= 3)
                    {
                        if (activateClass[1] != null)
                        {
                            activateClass[1].SetUpICardEffect("", new List<Cost>() { new TapCost() }, null, -1, true);
                            activateClass[1].SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                            IEnumerator ActivateCoroutine1()
                            {
                                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                            }

                            if (activateClass[1].CanUse(null))
                            {
                                check = true;
                                yield return ContinuousController.instance.StartCoroutine(activateClass[1].Activate_Optional_Cost_Execute(null, "Do you pay cost?"));
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
        }

        return cardEffects;
    }
}
