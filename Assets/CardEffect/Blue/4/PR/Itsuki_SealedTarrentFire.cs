using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;
public class Itsuki_SealedTarrentFire : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("絶命剣", "Fatal Sword",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.Owner.HandCards.Count((cardSource) => cardSource.Weapons.Contains(Weapon.Sharp)) > 0)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                CardSource discardedCard = null;

                SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                selectHandEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource) && cardSource.Weapons.Contains(Weapon.Sharp),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                    AfterSelectCardCoroutine: null,
                    mode: SelectHandEffect.Mode.Custom,
                    cardEffect: activateClass);

                yield return StartCoroutine(selectHandEffect.Activate(null));

                IEnumerator SelectCardCoroutine(CardSource cardSource)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("isCost", true);
                    yield return StartCoroutine(cardSource.cardOperation.DiscardFromHand(hashtable));

                    discardedCard = cardSource;
                }

                if(discardedCard != null)
                {
                    SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                    selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != unit.Character.Owner.Lord && unit.Character.PlayCost <= 2,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy,
                    cardEffect:null);

                    yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                    if(discardedCard.Owner.TrashCards.Contains(discardedCard))
                    {
                        if (discardedCard.cEntity_EffectController.GetCardEffects(EffectTiming.None).Count((cardEffect) => cardEffect.isCF) > 0)
                        {
                            if(discardedCard.CanPlayAsNewUnit())
                            {
                                ActivateClass activateClass1 = new ActivateClass();
                                activateClass1.SetUpICardEffect("","", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, true,card);
                                activateClass1.SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                                if (activateClass1.CanUse(null))
                                {
                                    yield return ContinuousController.instance.StartCoroutine(activateClass1.Activate_Optional_Cost_Execute(null, "Do you deploy the discarded card?"));
                                }

                                IEnumerator ActivateCoroutine1()
                                {
                                    if (discardedCard.Owner.isYou)
                                    {
                                        GManager.instance.commandText.OpenCommandText("Which do you deploy on Front or Back?");

                                        List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Itsuki",RpcTarget.All,true),0),
                                new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Itsuki",RpcTarget.All,false),1),
                            };

                                        GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                                    }

                                    yield return new WaitWhile(() => !endSelect);
                                    endSelect = false;

                                    GManager.instance.commandText.CloseCommandText();
                                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                                    Hashtable hashtable = new Hashtable();
                                    hashtable.Add("cardEffect", activateClass);
                                    yield return StartCoroutine(new IPlayUnit(discardedCard, null, isFront, true, hashtable, false).PlayUnit());
                                }
                            }
                        }
                    }
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("滅竜門", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }

    bool isFront = false;
    bool endSelect = false;
    [PunRPC]
    public void SetIsFront_Itsuki(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
}