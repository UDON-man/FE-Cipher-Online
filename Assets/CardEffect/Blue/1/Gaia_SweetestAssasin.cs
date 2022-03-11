using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaia_SweetestAssasin : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            bool check = false;

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("暗殺", "Assassination",new List<Cost>() { new TapCost() }, null, -1, false,card);
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

                    if (cardSource.PlayCost >= 3)
                    {
                        ActivateClass activateClass1 = new ActivateClass();
                        activateClass1.SetUpICardEffect("","", new List<Cost>() { new ReverseCost(2, (_cardSource) => true) }, null, -1, true,card);
                        activateClass1.SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                        IEnumerator ActivateCoroutine1()
                        {
                            SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                            selectUnitEffect.SetUp(
                                SelectPlayer: card.Owner,
                                CanTargetCondition: (unit) => unit.Character.Owner != card.Owner,
                                CanTargetCondition_ByPreSelecetedList: null,
                                CanEndSelectCondition: null,
                                MaxCount: 1,
                                CanNoSelect: false,
                                CanEndNotMax: false,
                                SelectUnitCoroutine: null,
                                AfterSelectUnitCoroutine: null,
                                mode: SelectUnitEffect.Mode.Destroy,
                                cardEffect: activateClass1);

                            yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
                        }

                        if (activateClass1.CanUse(null))
                        {
                            check = true;
                            yield return ContinuousController.instance.StartCoroutine(activateClass1.Activate_Optional_Cost_Execute(null, "Do you pay cost?"));
                        }
                    }

                    if(!check)
                    {
                        yield return new WaitForSeconds(1.5f);
                    }

                    yield return new WaitForSeconds(0.5f);
                    GManager.instance.GetComponent<Effects>().OffShowCard();
                }
            }
        }

        else if(timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("報酬はスイーツ", "Pay me in candy.", new List<Cost>() , new List<System.Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner,1).Draw());
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(hashtable != null)
                {
                    if(hashtable.ContainsKey("cardEffect"))
                    {
                        if(hashtable["cardEffect"] is ICardEffect)
                        {
                            ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                            if (cardEffect.card() != null)
                            {
                                if(cardEffect.card() ==card)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }
        }

        return cardEffects;
    }
}
