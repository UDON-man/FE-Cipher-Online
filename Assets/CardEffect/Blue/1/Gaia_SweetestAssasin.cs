using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaia_SweetestAssasin : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            bool check = false;

            activateClass[0].SetUpICardEffect("暗殺", new List<Cost>() { new TapCost() }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Assassination";
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    //ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Library Card", false));

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[0]);
                    yield return ContinuousController.instance.StartCoroutine(new IShowLibraryCard(new List<CardSource>() { cardSource }, hashtable, false).ShowLibraryCard());

                    if (cardSource.PlayCost >= 3)
                    {
                        SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                        selectUnitEffect.SetUpICardEffect("", new List<Cost>() { new ReverseCost(2, (_cardSource) => true) }, null, -1, true);

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
                mode: SelectUnitEffect.Mode.Destroy);

                        if (selectUnitEffect.CanUse(null))
                        {
                            check = true;
                            yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate_Optional_Cost_Execute(null, "Do you pay cost?"));
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

        else if(timing == EffectTiming.OnDestroyedOther)
        {
            activateClass[1].SetUpICardEffect("報酬はスイーツ", new List<Cost>() , new List<System.Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[1].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Pay me in candy.";
            }

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
                                if(cardEffect.card() == this.card)
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
