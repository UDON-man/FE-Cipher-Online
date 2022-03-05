using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

public class Wayu_WaitingForTheEnemy : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
        canNotBeEvadedClass.SetUpICardEffect("太刀筋絶好調!",new List<Cost>(),new List<Func<Hashtable, bool>>(),-1,false);
        canNotBeEvadedClass.SetLvS(card.UnitContainingThisCharacter(),4);
        canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == this.card.UnitContainingThisCharacter(), (DefendingUnit) => true);
        cardEffects.Add(canNotBeEvadedClass);

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[1].SetUpICardEffect("修行修行!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Back to sword practice!";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        bool canFromTrash = card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("ワユ")) > 0;
                        bool canFromHand = card.Owner.HandCards.Count((cardSource) => cardSource.UnitNames.Contains("ワユ")) > 0;

                        if (canFromTrash || canFromHand)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                isFromTrash = false;

                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => cardSource.UnitNames.Contains("ワユ"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => true,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: AfterSelectCardCoroutine,
                    Message: "Select a card to stack down from tarsh.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true);

                SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                selectHandEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource) && cardSource.UnitNames.Contains("ワユ"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: AfterSelectCardCoroutine,
                    mode: SelectHandEffect.Mode.Custom);


                //yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator AfterSelectCardCoroutine(List<CardSource> cardSources)
                {
                    foreach(CardSource cardSource in cardSources)
                    {
                        if (cardSource.Owner.HandCards.Contains(cardSource))
                        {
                            yield return StartCoroutine(GManager.instance.GetComponent<Effects>().DeleteHandCardEffectCoroutine(cardSource));
                        }
                    }
                    
                    yield return ContinuousController.instance.StartCoroutine(new IGrow(card.UnitContainingThisCharacter(), cardSources).Grow());
                }

                bool canFromTrash = card.Owner.TrashCards.Count((cardSource) => cardSource.UnitNames.Contains("ワユ")) > 0;
                bool canFromHand = card.Owner.HandCards.Count((cardSource) => cardSource.UnitNames.Contains("ワユ")) > 0;

                if(!canFromTrash && !canFromHand)
                {
                    yield break;
                }

                else if(canFromTrash && !canFromHand)
                {
                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                }

                else if (!canFromTrash && canFromHand)
                {
                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }

                else if(canFromTrash && canFromHand)
                {
                    if(card.Owner.isYou)
                    {
                        GManager.instance.commandText.OpenCommandText("Which do you select a card from?");

                        List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("From trash",() => photonView.RPC("SetisFromTrash_Wayu",RpcTarget.All,true),0),
                                new Command_SelectCommand("From hand",() => photonView.RPC("SetisFromTrash_Wayu",RpcTarget.All,false),1),
                            };

                        GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                    }

                    yield return new WaitWhile(() => !endSelect);
                    endSelect = false;

                    if(isFromTrash)
                    {
                        yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                    }

                    else
                    {
                        yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                    }
                }
            }
        }

        return cardEffects;
    }

    bool isFromTrash = false;
    bool endSelect = false;

    [PunRPC]
    public void SetisFromTrash_Wayu(bool isFromTrash)
    {
        this.isFromTrash = isFromTrash;
        endSelect = true;
        
    }

}



