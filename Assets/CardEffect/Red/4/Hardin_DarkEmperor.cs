using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;

public class Hardin_DarkEmperor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("闇のオーブ", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Darksphere";
            }

            IEnumerator ActivateCoroutine1()
            {
                yield return ContinuousController.instance.StartCoroutine(CostCoroutine());

                PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
                powerUpByEnemy.SetUpPowerUpByEnemyWeapon("", (enemyUnit, Power) => Power + 30, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit != enemyUnit.Character.Owner.Lord, PowerUpByEnemy.Mode.Defending);
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add(powerUpByEnemy);
            }

            activateClass[1].SetUpICardEffect("血染めの神槍", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Bloodstained Gradivus";
            }

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
                    mode: SelectUnitEffect.Mode.Custom);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator AfterSelectUnitCoroutine(List<Unit> targetUnits)
                {
                    List<IDestroyUnit> destroyUnits = new List<IDestroyUnit>();

                    Hashtable _hashtable = new Hashtable();
                    _hashtable.Add("cardEffect", selectUnitEffect);

                    foreach(Unit unit in targetUnits)
                    {
                        IDestroyUnit destroyUnit = new IDestroyUnit(unit, 1, BreakOrbMode.Hand, _hashtable);
                        destroyUnits.Add(destroyUnit);
                        yield return ContinuousController.instance.StartCoroutine(destroyUnit.Destroy());
                    }

                    if(destroyUnits.Count((destrouUnit) => destrouUnit.Destroyed) > 0)
                    {
                        RangeUpClass rangeUpClass = new RangeUpClass();
                        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                        card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(rangeUpClass);

                        PowerUpClass powerUpClass = new PowerUpClass();
                        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
                        card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);
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
                if (card.Owner.FieldUnit.Count((unit) => unit != card.UnitContainingThisCharacter() && !unit.CanNotDestroyedBySkill(GetComponent<SelectUnitEffect>())) > 0)
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
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && !unit.CanNotDestroyedBySkill(GetComponent<SelectUnitEffect>()),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy);

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
