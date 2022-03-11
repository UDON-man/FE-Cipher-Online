using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Hezar_TemptationWomanThief : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("秘密の鍵開け", "Locktouch",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(new IShowLibraryCard(new List<CardSource>() { cardSource }, hashtable, false).ShowLibraryCard());

                    if (cardSource.sex.Count((sex) => sex == Sex.female) > 0)
                    {
                        yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
                    }

                    else
                    {
                        yield return new WaitForSeconds(0.5f);
                    }

                    yield return new WaitForSeconds(2);
                    GManager.instance.GetComponent<Effects>().OffShowCard();
                }
            }
        }


        return cardEffects;
    }
}
