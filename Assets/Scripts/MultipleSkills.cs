using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class MultipleSkills : MonoBehaviourPunCallbacks
{
    public bool isUsing { get; set; } = false;

   public IEnumerator ActivateMultipleSkills(List<SkillInfo> skillInfos)
   {
        isUsing = true;

        List<SkillInfo> TurnPlayerSkillInfos = new List<SkillInfo>();
        List<SkillInfo> NonTurnPlayerSkillInfos = new List<SkillInfo>();

        foreach(SkillInfo skillInfo in skillInfos)
        {
            if(skillInfo != null)
            {
                ICardEffect cardEffect = skillInfo.cardEffect;
                Hashtable hashtable = skillInfo.hashtable;

                if(cardEffect.card() != null)
                {
                    if (cardEffect.CanUse(hashtable))
                    {
                        if (cardEffect.card().Owner == GManager.instance.turnStateMachine.gameContext.TurnPlayer)
                        {
                            TurnPlayerSkillInfos.Add(skillInfo);
                        }

                        else if (cardEffect.card().Owner == GManager.instance.turnStateMachine.gameContext.NonTurnPlayer)
                        {
                            NonTurnPlayerSkillInfos.Add(skillInfo);
                        }
                    }
                }
            }
        }

        GManager.instance.turnStateMachine.isSync = true;
        yield return ContinuousController.instance.StartCoroutine(ActivateMultipleSkills_OnePlayer(TurnPlayerSkillInfos, GManager.instance.turnStateMachine.gameContext.TurnPlayer));
        yield return ContinuousController.instance.StartCoroutine(ActivateMultipleSkills_OnePlayer(NonTurnPlayerSkillInfos, GManager.instance.turnStateMachine.gameContext.NonTurnPlayer));
        GManager.instance.turnStateMachine.isSync = false;

        isUsing = false;
    }

    bool endSelect = false;
    int skillIndex;

    IEnumerator ActivateMultipleSkills_OnePlayer(List<SkillInfo> skillInfos, Player player)
    {
        while (skillInfos.Count > 0)
        {
            #region 効果リストが一つなら普通に発動
            if (skillInfos.Count == 1)
            {
                ICardEffect cardEffect = skillInfos[0].cardEffect;
                Hashtable hashtable = skillInfos[0].hashtable;

                if(cardEffect != null)
                {
                    if (cardEffect is ActivateICardEffect)
                    {
                        if (cardEffect.CanUse(hashtable))
                        {
                            int ActivateCount = cardEffect.ActivateCount;

                            for (int i=0;i< ActivateCount; i++)
                            {
                                if(i > 0)
                                {
                                    yield return new WaitForSeconds(0.5f);
                                }

                                yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(hashtable));
                            }
                            
                        }
                    }
                }

                skillInfos.RemoveAt(0);
            }
            #endregion

            #region 効果リストが複数あるならどれから発動するか選択
            else
            {
                List<CardSource> RootCardSources = new List<CardSource>();

                foreach (SkillInfo skillInfo in skillInfos)
                {
                    if (skillInfo != null)
                    {
                        ICardEffect _cardEffect = skillInfo.cardEffect;

                        if (_cardEffect != null)
                        {
                            if (_cardEffect.card() != null)
                            {
                                RootCardSources.Add(_cardEffect.card());
                            }
                        }
                    }
                }

                yield return GManager.instance.photonWaitController.StartWait("StartSelectMultipleSkill");

                if(player.isYou)
                {
                    GManager.instance.commandText.OpenCommandText("Select which skill activates first.");

                    yield return StartCoroutine(GManager.instance.selectCardPanel.OpenSelectCardPanel(
                    Message: "Multiple skills have triggered at the same timing.\nWhich ability do you want to activate first?",
                    RootCardSources: RootCardSources,
                    _CanTargetCondition: (cardSource) => true,
                    _CanTargetCondition_ByPreSelecetedList: null,
                    _CanEndSelectCondition: null,
                    _MaxCount: 1,
                     _CanEndNotMax: false,
                    _CanNoSelect: () => false,
                    CanLookReverseCard: true,
                    skillInfos: skillInfos));

                    if (GManager.instance.selectCardPanel.SelectedIndex.Count > 0)
                    {
                        photonView.RPC("SetTargetSkill", RpcTarget.All, GManager.instance.selectCardPanel.SelectedIndex[0]);
                    }
                }

                else
                {
                    GManager.instance.commandText.OpenCommandText("The opponent is selecting which skill activates first.");

                    #region AI戦
                    if(GManager.instance.IsAI)
                    {
                        skillIndex = 0;
                        endSelect = true;
                    }
                    #endregion
                }

                yield return new WaitWhile(() => !endSelect);
                endSelect = false;

                GManager.instance.commandText.CloseCommandText();
                yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                ICardEffect cardEffect = skillInfos[skillIndex].cardEffect;
                Hashtable hashtable = skillInfos[skillIndex].hashtable;

                if (cardEffect is ActivateICardEffect)
                {
                    if (cardEffect.CanUse(hashtable))
                    {
                        int ActivateCount = cardEffect.ActivateCount;

                        for (int i = 0; i < ActivateCount; i++)
                        {
                            if (i > 0)
                            {
                                yield return new WaitForSeconds(0.5f);
                            }

                            yield return ContinuousController.instance.StartCoroutine(((ActivateICardEffect)cardEffect).Activate_Effect_Optional_Cost_Execute(hashtable));
                        }  
                    }
                }

                skillInfos.RemoveAt(skillIndex);

                #region 使用不可能になったスキルをリストから削除
                List<SkillInfo> nonActiveSkillInfos = new List<SkillInfo>();

                foreach(SkillInfo skillInfo in skillInfos)
                {
                    if(!skillInfo.cardEffect.CanUse(skillInfo.hashtable))
                    {
                        nonActiveSkillInfos.Add(skillInfo);
                    }
                }

                foreach (SkillInfo skillInfo in nonActiveSkillInfos)
                {
                    skillInfos.Remove(skillInfo);
                }
                #endregion

                yield return GManager.instance.photonWaitController.StartWait("EndSelectMultipleSkill");
            }
            #endregion
        }
    }

    [PunRPC]
    public void SetTargetSkill(int skillIndex)
    {
        this.skillIndex = skillIndex;
        endSelect = true;
    }
}

public class SkillInfo
{
    public SkillInfo(ICardEffect cardEffect, Hashtable hashtable)
    {
        this.cardEffect = cardEffect;
        this.hashtable = hashtable;
    }

    public ICardEffect cardEffect { get; set; }
    public Hashtable hashtable { get; set; }
}
