using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;

public class Hardin_DarkEmperor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("闇のオーブ", "Darksphere",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine1()
            {
                yield return ContinuousController.instance.StartCoroutine(CostCoroutine());

                PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
                powerUpByEnemy.SetUpPowerUpByEnemyWeapon("", (enemyUnit, Power) => Power + 30, UnitCondition, enemyUnitCondition, PowerUpByEnemy.Mode.Defending, card);
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add((_timing) => powerUpByEnemy);

                bool UnitCondition(Unit unit)
                {
                    if(unit != null)
                    {
                        if (unit == card.UnitContainingThisCharacter())
                        {
                            return true;
                        }
                    }

                    return false;
                }

                bool enemyUnitCondition(Unit enemyUnit)
                {
                    if(enemyUnit != null)
                    {
                        if (enemyUnit.Character != null)
                        {
                            if(enemyUnit != enemyUnit.Character.Owner.Lord)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("血染めの神槍", "Bloodstained Gradivus", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine2()
            {
                yield return ContinuousController.instance.StartCoroutine(CostCoroutine());

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
                    AfterSelectUnitCoroutine: (targetUnits) => AfterSelectUnitCoroutine(targetUnits),
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect: activateClass1);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator AfterSelectUnitCoroutine(List<Unit> targetUnits)
                {
                    List<IDestroyUnit> destroyUnits = new List<IDestroyUnit>();

                    foreach (Unit unit in targetUnits)
                    {
                        Hashtable _hashtable = new Hashtable();
                        _hashtable.Add("cardEffect", activateClass1);
                        _hashtable.Add("Unit", new Unit(unit.Characters));
                        IDestroyUnit destroyUnit = new IDestroyUnit(unit, 1, BreakOrbMode.Hand, _hashtable);
                        destroyUnits.Add(destroyUnit);
                        yield return ContinuousController.instance.StartCoroutine(destroyUnit.Destroy());
                    }

                    if(destroyUnits.Count((destrouUnit) => destrouUnit.Destroyed) > 0)
                    {
                        RangeUpClass rangeUpClass = new RangeUpClass();
                        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                        card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                        PowerModifyClass powerUpClass = new PowerModifyClass();
                        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                        card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
                    }
                }
            }

            #region コスト支払い
            bool CanPayReverseCost()
            {
                if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse) >= 2)
                {
                    return true;
                }

                return false;
            }

            bool CanPayDestroyCost()
            {
                if (card.Owner.FieldUnit.Count((unit) => unit != card.UnitContainingThisCharacter() && !unit.CanNotDestroyedByCost) > 0)
                {
                    return true;
                }

                return false;
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(CanPayReverseCost()|| CanPayDestroyCost())
                {
                    return true;
                }

                return false;
            }

            IEnumerator CostCoroutine()
            {
                endSelect = false;
                isDestroyCost = false;

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && !unit.CanNotDestroyedByCost,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy,
                    cardEffect:null);

                //リバースコストしか払えない場合
                if (CanPayReverseCost() && !CanPayDestroyCost())
                {
                    yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<PayCostClass>().PayReverseCost(2, (cardSource) => true, card));
                }

                //撃破コストしか払えない場合
                else if (!CanPayReverseCost() && CanPayDestroyCost())
                {
                    yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
                }

                //コストを選択できる場合
                else if (CanPayReverseCost() && CanPayDestroyCost())
                {
                    if (card.Owner.isYou)
                    {
                        GManager.instance.commandText.OpenCommandText("Do you destroy your unit?");

                        List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Yes",() => photonView.RPC("SetIsDestroyCost",RpcTarget.All,true),0),
                                new Command_SelectCommand("No",() => photonView.RPC("SetIsDestroyCost",RpcTarget.All,false),1),
                            };

                        GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                    }

                    yield return new WaitWhile(() => !endSelect);
                    endSelect = false;

                    GManager.instance.commandText.CloseCommandText();
                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                    if (isDestroyCost)
                    {
                        yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
                    }

                    else
                    {
                        yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<PayCostClass>().PayReverseCost(2, (cardSource) => true, card));
                    }
                }
            }
            #endregion
        }

        return cardEffects;
    }

    bool endSelect { get; set; } = false;
    bool isDestroyCost { get; set; } = false;

    [PunRPC]
    public void SetIsDestroyCost(bool isDestroyCost)
    {
        this.isDestroyCost = isDestroyCost;
        endSelect = true;
    }
}
