using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon;
using Photon.Pun;
using System;


public class SelectUnitEffect : MonoBehaviourPunCallbacks
{
    public void SetUp
        (Player SelectPlayer,
        Func<Unit, bool> CanTargetCondition, 
        Func<List<Unit>, Unit, bool> CanTargetCondition_ByPreSelecetedList, 
        Func<List<Unit>, bool> CanEndSelectCondition,
        int MaxCount, 
        bool CanNoSelect, 
        bool CanEndNotMax, 
        Func<Unit, IEnumerator> SelectUnitCoroutine, 
        Func<List<Unit>, IEnumerator> AfterSelectUnitCoroutine,
        Mode mode,
        ICardEffect cardEffect)
    {
        this.SelectPlayer = SelectPlayer;
        this.CanTargetCondition = CanTargetCondition;
        this.CanTargetCondition_ByPreSelecetedList = CanTargetCondition_ByPreSelecetedList;
        this.CanEndSelectCondition = CanEndSelectCondition;
        this.MaxCount = MaxCount;
        this.CanNoSelect = CanNoSelect;
        this.CanEndNotMax = CanEndNotMax;
        this.SelectUnitCoroutine = SelectUnitCoroutine;
        this.AfterSelectUnitCoroutine = AfterSelectUnitCoroutine;
        this.mode = mode;
        this.cardEffect = cardEffect;
    }

    //選択するプレイヤー
    Player SelectPlayer { get; set; }
    //選択することのできるユニットの条件
    Func<Unit, bool> CanTargetCondition { get; set; }
    //現在の選択リストの状態でそのユニットを選択することができるか
    Func<List<Unit>,Unit, bool> CanTargetCondition_ByPreSelecetedList { get; set; }
    //選択終了することのできる条件(選択終了時点のリストを参照)
    Func<List<Unit>,bool> CanEndSelectCondition { get; set; }
    //選択する最大枚数
    int MaxCount { get; set; }
    //選択しないことを選べるか
    bool CanNoSelect { get; set; }
    //最大数未満でも選択を終えられるか
    bool CanEndNotMax { get; set; }
    //(Mode.Custom時限定)選択してする処理
    Func<Unit,IEnumerator> SelectUnitCoroutine;
    //選択した後にする処理
    Func<List<Unit>, IEnumerator> AfterSelectUnitCoroutine;
    //選択してする処理の分類
    Mode mode;
    //スキル
    ICardEffect cardEffect;

    public List<IDestroyUnit> destroyUnits = new List<IDestroyUnit>();
    public enum Mode
    {
        Move,
        Tap,
        UnTap,
        Destroy,
        Custom
    }

    //選択されたユニットリスト
    public List<Unit> targetUnits { get; set; } = new List<Unit>();
    //選択しないフラグ
    bool NoSelect;

    bool endSelect;

    #region 選択が可能であるか
    public bool active()
    {
        if(CanNoSelect || CanEndNotMax)
        {
            return true;
        }

        int count = 0;

        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            foreach (Unit unit in player.FieldUnit)
            {
                if (CanTargetCondition(unit))
                {
                    count++;
                }
            }
        }

        if(count >= MaxCount)
        {
            return true;
        }

        return false;
    }
    #endregion

    public virtual IEnumerator Activate(Hashtable hash)
    {
        targetUnits = new List<Unit>();
        destroyUnits = new List<IDestroyUnit>();

        if (GManager.instance.IsAI && !SelectPlayer.isYou)
        {
            yield break;
        }

        if (active())
        {
            NoSelect = false;

            yield return GManager.instance.photonWaitController.StartWait("SelectUnitEffect");

            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                GManager.instance.turnStateMachine.OffFieldCardTarget(player);
                GManager.instance.turnStateMachine.OffHandCardTarget(player);
            }

            if (SelectPlayer.isYou)
            {
                GManager.instance.turnStateMachine.IsSelecting = true;

                #region メッセージ表示
                switch (mode)
                {
                    case Mode.Move:
                        if(MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a unit to move.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select units to move.");
                        }
                       
                        break;

                    case Mode.Tap:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a unit to tap.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select units to tap.");
                        }
                        
                        break;

                    case Mode.UnTap:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a unit to untap.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select units to untap.");
                        }

                        break;


                    case Mode.Destroy:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a unit to be destroyed.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select units to be destroyed.");
                        }

                        break;

                    case Mode.Custom:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a unit.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select units.");
                        }

                        break;
                }
                #endregion

                List<Unit> PreSelectedUnits = new List<Unit>();

                foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                {
                    foreach (Unit unit in player.FieldUnit)
                    {
                        if (CanTargetCondition(unit))
                        {
                            unit.ShowingFieldUnitCard.AddClickTarget(OnClickFieldUnitCard);
                        }
                    }
                }

                CheckEndSelect();

                #region フィールドユニットカードクリック時の処理
                void OnClickFieldUnitCard(FieldUnitCard fieldUnitCard)
                {
                    if (PreSelectedUnits.Contains(fieldUnitCard.thisUnit))
                    {
                        PreSelectedUnits.Remove(fieldUnitCard.thisUnit);
                    }

                    else
                    {
                        if(CanTargetCondition_ByPreSelecetedList != null)
                        {
                            if(!CanTargetCondition_ByPreSelecetedList(PreSelectedUnits,fieldUnitCard.thisUnit))
                            {
                                return;
                            }
                        }

                        if (PreSelectedUnits.Count < MaxCount)
                        {
                            PreSelectedUnits.Add(fieldUnitCard.thisUnit);
                        }

                        else
                        {
                            if (PreSelectedUnits.Count > 0)
                            {
                                PreSelectedUnits.RemoveAt(PreSelectedUnits.Count - 1);
                                PreSelectedUnits.Add(fieldUnitCard.thisUnit);
                            }
                        }
                    }

                    CheckEndSelect();
                }
                #endregion

                #region 選択終了可能かどうか判定しアウトライン表示
                void CheckEndSelect()
                {
                    #region 終了出来るか判定
                    bool CanEndSelect()
                    {
                        //枚数の条件を満たさない場合
                        if (!(PreSelectedUnits.Count == MaxCount || (PreSelectedUnits.Count <= MaxCount && CanEndNotMax)))
                        {
                            return false;
                        }

                        //指定された条件を満たさない場合
                        if(CanEndSelectCondition != null)
                        {
                            if(!CanEndSelectCondition(PreSelectedUnits))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                    #endregion

                    #region 終了できるかによってUI表示
                    if (CanEndSelect())
                    {
                        GManager.instance.selectCommandPanel.SetUpCommandButton(new List<Command_SelectCommand>() { new Command_SelectCommand("End Selection", SetTargetUnits_RPC, 0) });

                        void SetTargetUnits_RPC()
                        {
                            foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
                            {
                                foreach (Unit unit in player.FieldUnit)
                                {
                                    unit.ShowingFieldUnitCard.RemoveSelectEffect();
                                    unit.ShowingFieldUnitCard.OffClickTarget();
                                }
                            }
                            
                            List<bool> isTurnPlayer = new List<bool>();
                            List<int> UnitIndex = new List<int>();

                            foreach(Unit unit in PreSelectedUnits)
                            {
                                isTurnPlayer.Add(unit.Character.Owner == GManager.instance.turnStateMachine.gameContext.TurnPlayer);
                                UnitIndex.Add(unit.Character.Owner.FieldUnit.IndexOf(unit));
                            }

                            photonView.RPC("SetTargetUnits", RpcTarget.All, isTurnPlayer.ToArray(),UnitIndex.ToArray() );

                            GManager.instance.BackButton.CloseSelectCommandButton();
                        }
                    }

                    else
                    {
                        GManager.instance.selectCommandPanel.CloseSelectCommandPanel();
                    }
                    #endregion

                    #region 選択リストによってアウトライン表示
                    foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                    {
                        foreach (Unit unit in player.FieldUnit)
                        {
                            unit.ShowingFieldUnitCard.RemoveSelectEffect();

                            if(CanTargetCondition(unit))
                            {
                                if (PreSelectedUnits.Contains(unit))
                                {
                                    unit.ShowingFieldUnitCard.OnSelectEffect(1.1f);
                                    unit.ShowingFieldUnitCard.SetOrangeOutline();
                                }

                                else
                                {
                                    unit.ShowingFieldUnitCard.OnSelectEffect(1.1f);
                                    unit.ShowingFieldUnitCard.SetBlueOutline();

                                    if (CanTargetCondition_ByPreSelecetedList != null)
                                    {
                                        if(!CanTargetCondition_ByPreSelecetedList(PreSelectedUnits,unit))
                                        {
                                            unit.ShowingFieldUnitCard.RemoveSelectEffect();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region 戻るボタン表示
                if (CanNoSelect)
                {
                    GManager.instance.BackButton.OpenSelectCommandButton("Not select", () => { photonView.RPC("SetNoSelectUnit", RpcTarget.All); }, 0);
                }
                #endregion
            }

            else
            {
                #region メッセージ表示
                if (MaxCount == 1)
                {
                    GManager.instance.commandText.OpenCommandText("The opponent is selecting a unit.");
                }

                else
                {
                    GManager.instance.commandText.OpenCommandText("The opponent is selecting units.");
                }
                #endregion
            }

            //選択終了まで待機
            yield return new WaitWhile(() => !endSelect);
            endSelect = false;

            #region リセット
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                GManager.instance.turnStateMachine.OffFieldCardTarget(player);
                GManager.instance.turnStateMachine.OffHandCardTarget(player);

                foreach (Unit unit in player.FieldUnit)
                {
                    unit.ShowingFieldUnitCard.RemoveSelectEffect();
                }
            }

            GManager.instance.commandText.CloseCommandText();
            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

            #endregion

            Hashtable hashtable = new Hashtable();
            hashtable.Add("cardEffect", cardEffect);

            if (!NoSelect)
            {
                foreach(Unit targetUnit in targetUnits)
                {
                    FieldUnitCard fieldUnitCard = targetUnit.ShowingFieldUnitCard;

                    fieldUnitCard.OnSelectEffect(1.1f);

                    #region ターゲット矢印表示
                    if (cardEffect != null)
                    {
                        if(cardEffect.card() != null)
                        {
                            if (cardEffect.card().UnitContainingThisCharacter() == null)
                            {
                                if (cardEffect.card().Owner.SupportCards.Contains(cardEffect.card()))
                                {
                                    if (cardEffect.card().Owner.isYou)
                                    {
                                        yield return GManager.instance.OnTargetArrow(
                                        new Vector3(840, -120, 0),
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        null,
                                        null);
                                    }

                                    else
                                    {
                                        yield return GManager.instance.OnTargetArrow(
                                        new Vector3(840, 327, 0),
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        null,
                                        null);
                                    }
                                }

                                else if (cardEffect.card().Owner.TrashCards.Contains(cardEffect.card()) || cardEffect.card().Owner.InfinityCards.Contains(cardEffect.card()))
                                {
                                    if (cardEffect.card().Owner.isYou)
                                    {
                                        yield return GManager.instance.OnTargetArrow(
                                        new Vector3(-720, -150, 0),
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        null,
                                        null);
                                    }

                                    else
                                    {
                                        yield return GManager.instance.OnTargetArrow(
                                        new Vector3(-720, 150, 0),
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        null,
                                        null);
                                    }
                                }

                                else if (cardEffect.card().Owner.BondCards.Contains(cardEffect.card()))
                                {
                                    if (cardEffect.card().Owner.isYou)
                                    {
                                        yield return GManager.instance.OnTargetArrow(
                                        new Vector3(-690, -320, 0),
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        null,
                                        null);
                                    }

                                    else
                                    {
                                        yield return GManager.instance.OnTargetArrow(
                                        new Vector3(-690, 320, 0),
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        null,
                                        null);
                                    }
                                }

                                else
                                {
                                    yield return GManager.instance.OnTargetArrow(
                                        GManager.instance.ExecuteTransform.localPosition,
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        null,
                                        null);
                                }
                            }

                            else
                            {
                                if (cardEffect.card().UnitContainingThisCharacter().ShowingFieldUnitCard != null)
                                {
                                    yield return GManager.instance.OnTargetArrow(
                                        cardEffect.card().UnitContainingThisCharacter().ShowingFieldUnitCard.GetLocalCanvasPosition(),
                                        fieldUnitCard.GetLocalCanvasPosition(),
                                        cardEffect.card().UnitContainingThisCharacter().ShowingFieldUnitCard,
                                        fieldUnitCard);
                                }
                            }
                        }
                    }

                    yield return new WaitForSeconds(0.2f);
                    #endregion

                    #region 選択されたユニットに対して処理を行う
                    switch (mode)
                    {
                        case Mode.Move:
    
                            yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { targetUnit }, true,hashtable).MoveUnits());
                            break;

                        case Mode.Tap:
                            yield return ContinuousController.instance.StartCoroutine(targetUnit.Tap());
                            break;

                        case Mode.UnTap:
                            yield return ContinuousController.instance.StartCoroutine(targetUnit.UnTap(hashtable));
                            break;

                        case Mode.Destroy:

                            Hashtable hashtable1 = new Hashtable();
                            hashtable1.Add("cardEffect", cardEffect);
                            hashtable1.Add("Unit", new Unit(targetUnit.Characters));
                            destroyUnits.Add(new IDestroyUnit(targetUnit, 1, BreakOrbMode.Hand, hashtable1));
                            //yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(targetUnit,1,BreakOrbMode.Hand,hashtable1).Destroy());
                            break;

                        case Mode.Custom:
                            if (SelectUnitCoroutine != null)
                            {
                                yield return StartCoroutine(SelectUnitCoroutine(targetUnit));
                            }
                            break;
                    }
                    #endregion

                    for(int i=0;i<5;i++)
                    {
                        GManager.instance.OffTargetArrow();
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    

                    if (fieldUnitCard != null)
                    {
                        fieldUnitCard.RemoveSelectEffect();
                    }
                }

                if(destroyUnits.Count > 0)
                {
                    foreach (IDestroyUnit destroyUnit in destroyUnits)
                    {
                        yield return ContinuousController.instance.StartCoroutine(destroyUnit.Destroy());
                    }
                }
            }

            if (AfterSelectUnitCoroutine != null)
            {
                yield return StartCoroutine(AfterSelectUnitCoroutine(targetUnits));
            }
        }

        GManager.instance.turnStateMachine.IsSelecting = false;
    }

    #region ユニット選択を決定
    [PunRPC]
    public void SetTargetUnits(bool[] isTurnPlayer, int[] UnitIndex)
    {
        targetUnits = new List<Unit>();

        for(int i=0;i<isTurnPlayer.Length;i++)
        {
            Player player = null;

            if (isTurnPlayer[i])
            {
                player = GManager.instance.turnStateMachine.gameContext.TurnPlayer;
            }

            else
            {
                player = GManager.instance.turnStateMachine.gameContext.NonTurnPlayer;
            }

            Unit unit = player.FieldUnit[UnitIndex[i]];

            targetUnits.Add(unit);
        }

        endSelect = true;
    }
    #endregion

    #region 何も選ばない
    [PunRPC]
    public void SetNoSelectUnit()
    {
        GManager.instance.selectCommandPanel.CloseSelectCommandPanel();

        targetUnits = new List<Unit>();

        NoSelect = true;

        endSelect = true;
    }
    #endregion
}

