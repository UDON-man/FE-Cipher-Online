using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;
public class Kamira_BeautifulObsidianPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("爆発", "Eruption",new List<Cost>() { new ReverseCost(3, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.OrbCards.Count((cardSource) => cardSource.IsReverse) > 0)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.Owner == card.Owner && cardSource.IsReverse,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a orb card to turn face up.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.SetFace,
                        root: SelectCardEffect.Root.Orb,
                        CustomRootCardList: null,
                        CanLookReverseCard: false,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    yield return GManager.instance.photonWaitController.StartWait("Select_Kamira");

                    if (card.Owner.isYou)
                    {
                        GManager.instance.commandText.OpenCommandText("Which do you select, Front or Back?");

                        List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Kamira",RpcTarget.All,true),0),
                                new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Kamira",RpcTarget.All,false),1),
                            };

                        GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                    }

                    yield return new WaitWhile(() => !endSelect);
                    endSelect = false;

                    GManager.instance.commandText.CloseCommandText();
                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                    List<Unit> DestroyUnit = new List<Unit>();

                    List<Unit> targetUnits()
                    {
                        List<Unit> _targetUnits = new List<Unit>();

                        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
                        {
                            if (isFront)
                            {
                                foreach(Unit unit in player.GetFrontUnits())
                                {
                                    if(unit != unit.Character.Owner.Lord)
                                    {
                                        _targetUnits.Add(unit);
                                    }
                                }
                            }

                            else
                            {
                                foreach (Unit unit in player.GetBackUnits())
                                {
                                    if (unit != unit.Character.Owner.Lord)
                                    {
                                        _targetUnits.Add(unit);
                                    }
                                }
                            }
                        }
                            
                        return _targetUnits;
                    }

                    foreach(Unit unit in targetUnits())
                    {
                        DestroyUnit.Add(unit);
                    }

                    foreach (Unit unit in DestroyUnit)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("cardEffect", activateClass);
                        hashtable.Add("Unit", new Unit(unit.Characters));
                        yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable).Destroy());
                    }
                }
            }
        }

        else if (timing == EffectTiming.OnEndTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("漆黒の霧", "Obsidian Mist", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.OrbCards.Count((cardSource) => !cardSource.IsReverse) > 0)
                {
                    foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
                    {
                        foreach (Unit unit in player.FieldUnit)
                        {
                            if (unit != player.Lord && unit.Character.PlayCost == 1)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                List<Unit> DestroyUnit = new List<Unit>();

                foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players_ForTurnPlayer)
                {
                    foreach(Unit unit in player.FieldUnit)
                    {
                        if(unit != player.Lord && unit.Character.PlayCost == 1)
                        {
                            DestroyUnit.Add(unit);
                        }
                    }
                }

                foreach(Unit unit in DestroyUnit)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    hashtable.Add("Unit", new Unit(unit.Characters));
                    yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable).Destroy());
                }
            }
        }

        return cardEffects;
    }

    bool isFront = false;
    bool endSelect = false;
    [PunRPC]
    public void SetIsFront_Kamira(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
}
