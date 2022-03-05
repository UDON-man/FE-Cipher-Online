using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
public enum BreakOrbMode
{
    Hand,
    Trash,
    Bond,
}

public class BreakOrb : MonoBehaviourPunCallbacks
{
    List<CardSource> BrokenOrbs = new List<CardSource>();
    bool endSelect = false;

    public IEnumerator BreakOrbCoroutine(Player player,int breakCount,BreakOrbMode mode)
    {
        if(breakCount > player.OrbCount)
        {
            breakCount = player.OrbCount;
        }

        BrokenOrbs = new List<CardSource>();
        endSelect = false;

        yield return GManager.instance.photonWaitController.StartWait("BreakOrb");

        if(player.isYou)
        {
            GManager.instance.commandText.OpenCommandText("Select broken Orbs");

            yield return StartCoroutine(GManager.instance.selectCardPanel.OpenSelectCardPanel(
                    Message: "Select broken Orbs.",
                    RootCardSources: player.OrbCards,
                    _CanTargetCondition: (cardSource) => true,
                    _CanTargetCondition_ByPreSelecetedList: null,
                    _CanEndSelectCondition: null,
                    _MaxCount: breakCount,
                     _CanEndNotMax: false,
                    _CanNoSelect: () => false,
                    CanLookReverseCard: false,
                    skillInfos: null));

            List<int> cardIDs = new List<int>();

            foreach(CardSource cardSource in GManager.instance.selectCardPanel.SelectedList)
            {
                cardIDs.Add(cardSource.cardIndex);
            }

            photonView.RPC("SetBrokenOrb",RpcTarget.All,cardIDs.ToArray());
        }

        else
        {
            GManager.instance.commandText.OpenCommandText("The opponent is selecting broken Orbs");

            #region デバッグモード
            if (GManager.instance.IsAI)
            {
                while(BrokenOrbs.Count < breakCount)
                {
                    CardSource cardSource = player.OrbCards[UnityEngine.Random.Range(0, player.OrbCards.Count)];

                    if(!BrokenOrbs.Contains(cardSource))
                    {
                        BrokenOrbs.Add(cardSource);
                    }
                }

                endSelect = true;
            }
            #endregion
        }

        yield return new WaitWhile(() => !endSelect);
        endSelect = false;

        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

        foreach(CardSource cardSource in BrokenOrbs)
        {
            cardSource.SetFace();

            switch(mode)
            {
                case BreakOrbMode.Hand:
                    yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddHandCard(cardSource, false));
                    break;

                case BreakOrbMode.Trash:
                    CardObjectController.AddTrashCard(cardSource);
                    break;
                case BreakOrbMode.Bond:
                    yield return StartCoroutine(new ISetBondCard(cardSource, true).SetBond());
                    yield return StartCoroutine(cardSource.Owner.bondObject.SetBond_Skill(cardSource.Owner));
                    break;
            }

            cardSource.Owner.OrbCards.Remove(cardSource);
        }
    }

    [PunRPC]
    public void SetBrokenOrb(int[] cardIDs)
    {
        BrokenOrbs = new List<CardSource>();

        foreach(int cardID in cardIDs)
        {
            BrokenOrbs.Add(GManager.instance.turnStateMachine.gameContext.ActiveCardList[cardID]);
        }

        endSelect = true;
    }
}
