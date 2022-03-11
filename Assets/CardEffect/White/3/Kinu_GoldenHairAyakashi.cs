using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class Kinu_GoldenHairAyakashi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("次は次はー?", "Who's That, Who's That?", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit != null)
                            {
                                if (Unit.Character != null)
                                {
                                    if (Unit != card.UnitContainingThisCharacter())
                                    {
                                        return true;
                                    }
                                }

                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnStartTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("妖宴の戯れ", "Masquerade", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                bool check = false;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                if (card.Owner.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.LibraryCards[0];
                    card.Owner.LibraryCards.Remove(cardSource);
                    CardObjectController.AddTrashCard(cardSource);
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                    ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Deck Top Card", false));

                    if (cardSource.CanPlayAsNewUnit())
                    {
                        ActivateClass activateClass1 = new ActivateClass();
                        activateClass1.SetUpICardEffect("", "",new List<Cost>() { new ReverseCost(1, (_cardSource) => true) }, null, -1, true,card);
                        activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());

                        IEnumerator ActivateCoroutine1()
                        {
                            #region そのユニットの効果は無効化される
                            InvalidationClass invalidationClass = new InvalidationClass();
                            invalidationClass.SetUpInvalidationClass(InvalidateCondition);
                            card.Owner.UntilEachTurnEndEffects.Add((_timing) => invalidationClass);

                            bool InvalidateCondition(ICardEffect _cardEffect)
                            {
                                if(cardSource.cEntity_EffectController.GetAllCardEffects().Contains(_cardEffect))
                                {
                                    return true;
                                }

                                if(_cardEffect.card() != null)
                                {
                                    if (_cardEffect.card() == cardSource)
                                    {
                                        return true;
                                    }
                                }

                                return false;
                            }
                            #endregion

                            #region 前衛・後衛を指定
                            if (cardSource.Owner.isYou)
                            {
                                GManager.instance.commandText.OpenCommandText("Which do you deploy on Front or Back?");

                                List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Kinu",RpcTarget.All,true),0),
                                new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Kinu",RpcTarget.All,false),1),
                            };

                                GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                            }

                            yield return new WaitWhile(() => !endSelect);
                            endSelect = false;

                            GManager.instance.commandText.CloseCommandText();
                            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                            #endregion

                            Hashtable hashtable = new Hashtable();
                            hashtable.Add("cardEffect", activateClass1);
                            card.Owner.TrashCards.Remove(cardSource);
                            yield return StartCoroutine(new IPlayUnit(cardSource, null, isFront, true, hashtable, false).PlayUnit());

                            #region ターン終了時にそのユニットは破壊される
                            ActivateClass activateClass2 = new ActivateClass();
                            activateClass2.SetUpICardEffect("出撃させたユニットを\n撃破する", "Destroy the\ndeployed unit.", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
                            activateClass2.SetUpActivateClass((hashtable1) => ActivateCoroutine2());
                            card.Owner.UntilEachTurnEndEffects.Add(TurnEndAction);

                            bool CanUseCondition(Hashtable hashtable1)
                            {
                                if (cardSource.UnitContainingThisCharacter() != null)
                                {
                                    return true;
                                }

                                return false;
                            }

                            IEnumerator ActivateCoroutine2()
                            {
                                Hashtable hashtable1 = new Hashtable();
                                hashtable1.Add("cardEffect", activateClass2);
                                hashtable.Add("Unit", new Unit(cardSource.UnitContainingThisCharacter().Characters));
                                yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(cardSource.UnitContainingThisCharacter(), 1, BreakOrbMode.Hand, hashtable1).Destroy());
                            }

                            ICardEffect TurnEndAction(EffectTiming _timing)
                            {
                                if(_timing == EffectTiming.OnEndTurn)
                                {
                                    return activateClass2;
                                }

                                return null;
                            }
                            #endregion

                            #region そのユニットはレベルアップできない
                            CanNotLevelUpClass canNotLevelUpClass = new CanNotLevelUpClass();
                            canNotLevelUpClass.SetUpCanNotLevelUpCondition((unit) => unit == cardSource.UnitContainingThisCharacter());
                            cardSource.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => canNotLevelUpClass);
                            #endregion
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
        }

        return cardEffects;
    }

    bool isFront = false;
    bool endSelect = false;
    [PunRPC]
    public void SetIsFront_Kinu(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
}