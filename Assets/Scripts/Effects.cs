using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AutoLayout3D;
using Cinemachine;
using System;
using System.Linq;
public class Effects : MonoBehaviour
{
    public void Init()
    {
        foreach(Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            player.SupportHandCard.gameObject.SetActive(false);

            player.LibraryCountText.transform.parent.gameObject.SetActive(false);
        }

        ShowUseHandCardParent.gameObject.SetActive(false);

        OffShowCard();
    }

    #region 手札のカードを使用する
    [Header("手札のカード消滅時エフェクト")]
    public GameObject DeleteHandCardEffect;

    #region 手札のカードを削除
    public IEnumerator DeleteHandCardEffectCoroutine(CardSource card)
    {
        bool end = false;

        HandCard handCard = card.ShowingHandCard;

        if(handCard == null)
        {
            yield break;
        }

        handCard.PlayCostText.transform.parent.gameObject.SetActive(false);
        handCard.CCCostText.transform.parent.gameObject.SetActive(false);

        handCard.CardImage.sprite = null;

        float shrinkTime = 0.5f;

        var sequence = DOTween.Sequence();

        sequence
            .Append(DOTween.To(() => handCard.transform.localScale, (x) => handCard.transform.localScale = x, new Vector3(0, 0, 0), shrinkTime).SetEase(Ease.OutCubic))
            .AppendCallback(() => { end = true; });

        sequence.Play();

        while (handCard.transform.localScale.x > 0.2f)
        {
            yield return null;
        }

        GameObject deleteHandCerdEffect = Instantiate(DeleteHandCardEffect, GManager.instance.canvas.transform);

        deleteHandCerdEffect.transform.position = handCard.transform.position;

        StartCoroutine(DeleteCoroutine(deleteHandCerdEffect));

        while (!end)
        {
            yield return null;
        }

        end = false;

        yield return new WaitForSeconds(0.15f);

        card.Owner.HandCards.Remove(card);
        Destroy(handCard.gameObject);
        CardObjectController.AlignHand(card.Owner);
    }
    #endregion

    #endregion

    #region エフェクトを削除する
    public static IEnumerator DeleteCoroutine(GameObject effect)
    {
        yield return new WaitForSeconds(5f);

        Destroy(effect);
    }
    #endregion

    #region 支援カードを表示エフェクト
    public IEnumerator ShowSupportEffect(CardSource cardSource)
    {
        Player player = cardSource.Owner;

        bool end = false;
        var sequence = DOTween.Sequence();

        //支援カードアクティブ化
        player.SupportHandCard.gameObject.SetActive(true);

        player.SupportHandCard.SupportSkillIconParent.SetActive(false);

        player.SupportHandCard.transform.localScale = new Vector3(1, 1, 1);

        player.SupportHandCard.SkillNameText.transform.parent.gameObject.SetActive(false);

        //支援カードを裏面にする
        player.SupportHandCard.SetUpReverseCard();

        //支援カード初期座標セット
        player.SupportHandCard.transform.localPosition = Vector3.zero;

        //支援カード初期回転値セット
        player.SupportHandCard.transform.localRotation = player.LibraryCountText.transform.parent.GetChild(0).localRotation;

        //アウトラインオフ
        player.SupportHandCard.OnRemoveSelect();

        #region カードをその場で回転、表裏反転

        #region 0°→90°まで回転
        sequence = DOTween.Sequence();
        sequence
            .Append(player.SupportHandCard.transform.DOLocalRotate(new Vector3(90, 0, player.SupportHandCard.transform.localRotation.eulerAngles.z), 0.1f))
            .AppendCallback(() => end = true);

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;
        #endregion

        //支援カードを表面にする
        player.SupportHandCard.SetUpHandCard(cardSource);
        player.SupportHandCard.SetUpHandCardImage();

        //支援値表示
        player.SupportHandCard.ShowSupport();

        //アウトライン表示
        player.SupportHandCard.OnOutline();
        player.SupportHandCard.SetBlueOutline();

        #region 90°→0°まで回転
        sequence = DOTween.Sequence();
        sequence
            .Append(player.SupportHandCard.transform.DOLocalRotate(new Vector3(0, 0, player.SupportHandCard.transform.localRotation.eulerAngles.z), 0.1f))
            .AppendCallback(() => end = true);

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;
        #endregion

        #endregion

        #region 支援エリアの場所まで回転して移動
        sequence = DOTween.Sequence();
        sequence
            .Append(player.SupportHandCard.transform.DOLocalMove(targetPos(), 0.15f))
            .Join(player.SupportHandCard.transform.DOLocalRotate(Vector3.zero, 0.15f))
            .AppendCallback(() => end = true);

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;
        #endregion

        //支援スキルあるならアイコン表示
        yield return ContinuousController.instance.StartCoroutine(ShowSupportIconEffect(player.SupportHandCard));

        #region 支援カード最終座標
        Vector3 targetPos()
        {
            if (player.isYou)
            {
                return new Vector3(0, -150, 0);
            }

            else
            {
                return new Vector3(0, 150, 0);
            }
        }
        #endregion

    }
    #endregion

    #region 支援スキルアイコンを表示
    public IEnumerator ShowSupportIconEffect(HandCard handCard)
    {
        if (handCard.cardSource != null)
        {
            if(handCard.cardSource.cEntity_EffectController.GetAllSupportEffects().Count((cardEffect) => cardEffect.CanUse(null)) > 0)
            {
                handCard.SupportSkillIconParent.SetActive(true);
                handCard.SupportSkillIconParent.transform.localPosition = new Vector3(0, 4, 0);
                handCard.SupportSkillIconParent.transform.localScale = new Vector3(0f, 0f, 0.3f);

                if (handCard.cardSource.Owner == GManager.instance.turnStateMachine.gameContext.TurnPlayer)
                {
                    handCard.SupportSkillIcon_Attack.gameObject.SetActive(true);
                    handCard.SupportSkillIcon_Defence.gameObject.SetActive(false);
                }

                else
                {
                    handCard.SupportSkillIcon_Attack.gameObject.SetActive(false);
                    handCard.SupportSkillIcon_Defence.gameObject.SetActive(true);
                }

                #region 支援スキルアイコンのアニメーション
                if(handCard.cardSource.Owner == GManager.instance.turnStateMachine.gameContext.TurnPlayer)
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit.CanNotSupportThisUnit(handCard.cardSource))
                    {
                        yield break;
                    }
                }

                else
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit.CanNotSupportThisUnit(handCard.cardSource))
                    {
                        yield break;
                    }
                }

                bool end = false;
                var sequence = DOTween.Sequence();

                sequence = DOTween.Sequence();
                sequence
                    .Append(handCard.SupportSkillIconParent.transform.DOScale(new Vector3(1f,1f,1),0.2f))
                    .AppendCallback(() => end = true);

                sequence.Play();

                yield return new WaitWhile(() => !end);
                end = false;

                yield return new WaitForSeconds(0.4f);
                #endregion
            }
        }
    }
    #endregion

    #region 場のユニットのスキルが発動した時のエフェクト
    [Header("場のユニットのスキルが発動した時のエフェクト")]
    public GameObject FieldUnitEffectPrefab;
    public IEnumerator ActivateFieldUnitSkillEffect(Unit unit,ICardEffect cardEffect)
    {
        bool end = false;
        var sequence = DOTween.Sequence();

        if(unit.ShowingFieldUnitCard != null)
        {
            if(unit.ShowingFieldUnitCard.transform.parent.GetComponent<XAxisLayoutGroup3D>() != null)
            {
                unit.ShowingFieldUnitCard.OnSkillName(cardEffect);

                unit.ShowingFieldUnitCard.transform.parent.GetComponent<XAxisLayoutGroup3D>().enabled = false;

                sequence = DOTween.Sequence();

                sequence
                    .Append(unit.ShowingFieldUnitCard.transform.DOMoveY(12, 0.3f))
                    .AppendCallback(() => end = true);

                sequence.Play();

                GameObject Effect = Instantiate(FieldUnitEffectPrefab);
                Effect.transform.position = unit.ShowingFieldUnitCard.transform.position + new Vector3(0,13,0);

                yield return new WaitWhile(() => !end);
                end = false;

                yield return new WaitForSeconds(0.3f);

                sequence = DOTween.Sequence();

                sequence
                    .Append(unit.ShowingFieldUnitCard.transform.DOMoveY(0.1f, 0.1f))
                    .AppendCallback(() => end = true);

                sequence.Play();

                unit.ShowingFieldUnitCard.transform.parent.GetComponent<XAxisLayoutGroup3D>().enabled = true;

                StartCoroutine(DeleteCoroutine(Effect));

                unit.ShowingFieldUnitCard.OnUsingSkillEffect();
            }        
        }
    }
    #endregion

    #region 墓地・無限ゾーンのカード効果が発動した時のエフェクト
    public IEnumerator ActivateTrashInfinityCardSkillEffect(CardSource cardSource,ICardEffect cardEffect)
    {
        bool end = false;
        var sequence = DOTween.Sequence();

        if (cardSource.Owner.TrashCards.Contains(cardSource)||cardSource.Owner.InfinityCards.Contains(cardSource))
        {
            cardSource.Owner.TrashHandCard.gameObject.SetActive(true);
            cardSource.Owner.TrashHandCard.SetUpHandCard(cardSource);
            cardSource.Owner.TrashHandCard.SetUpHandCardImage();
            cardSource.Owner.TrashHandCard.OnOutline();
            cardSource.Owner.TrashHandCard.SetBlueOutline();
            cardSource.Owner.TrashHandCard.transform.localScale = new Vector3(1, 1, 1);
            cardSource.Owner.TrashHandCard.SetSkillName(cardEffect);

            Vector3 startPosition = Vector3.zero;
            Vector3 targetPosition = Vector3.zero;
            if(cardSource.Owner.InfinityCards.Contains(cardSource))
            {
                startPosition = new Vector3(-110,0,0);
            }

            if(cardSource.Owner.isYou)
            {
                targetPosition = new Vector3(60, 120, 0);
            }
            else
            {
                targetPosition = new Vector3(60, -120, 0);
            }
            cardSource.Owner.TrashHandCard.transform.localPosition = startPosition;
            
            sequence = DOTween.Sequence();

            sequence
                .Append(cardSource.Owner.TrashHandCard.transform.DOScale(new Vector3(1.4f, 1.4f, 1), 0.3f))
                .Join(cardSource.Owner.TrashHandCard.transform.DOLocalMove(targetPosition, 0.3f))
                .AppendCallback(() => end = true);

            sequence.Play();

            yield return new WaitWhile(() => !end);
            end = false;

            sequence = DOTween.Sequence();

            sequence
                .Append(cardSource.Owner.TrashHandCard.transform.DOScale(new Vector3(2f, 2f, 1), 0.3f))
                .AppendCallback(() => end = true);
            sequence.Play();

            GameObject Effect = Instantiate(FieldUnitEffectPrefab);
            Effect.transform.position = cardSource.Owner.TrashHandCard.transform.position + new Vector3(0, 1, 0);

            yield return new WaitWhile(() => !end);
            end = false;

            yield return new WaitForSeconds(0.3f);

            sequence = DOTween.Sequence();

            sequence
                   .Append(cardSource.Owner.TrashHandCard.transform.DOScale(new Vector3(1.4f, 1.4f, 1), 0.1f))
                   .AppendCallback(() => end = true);

            sequence.Play();

            StartCoroutine(DeleteCoroutine(Effect));
        }
    }
    #endregion

    #region 絆ゾーンのカード効果が発動した時のエフェクト
    public IEnumerator ActivateBondCardSkillEffect(CardSource cardSource,ICardEffect cardEffect)
    {
        bool end = false;
        var sequence = DOTween.Sequence();

        if (cardSource.Owner.BondCards.Contains(cardSource))
        {
            cardSource.Owner.TrashHandCard.gameObject.SetActive(true);
            cardSource.Owner.TrashHandCard.SetUpHandCard(cardSource);
            cardSource.Owner.TrashHandCard.SetUpHandCardImage();
            cardSource.Owner.TrashHandCard.OnOutline();
            cardSource.Owner.TrashHandCard.SetBlueOutline();
            cardSource.Owner.TrashHandCard.transform.localScale = new Vector3(1, 1, 1);
            cardSource.Owner.TrashHandCard.SetSkillName(cardEffect);

            Vector3 startPosition = Vector3.zero;
            Vector3 targetPosition = Vector3.zero;

            if (cardSource.Owner.isYou)
            {
                startPosition = new Vector3(-35, -280, 0);
                targetPosition = new Vector3(150, -170, 0);
            }
            else
            {
                startPosition = new Vector3(-35, 280, 0);
                targetPosition = new Vector3(150, 170, 0);
            }

            cardSource.Owner.TrashHandCard.transform.localPosition = startPosition;

            sequence = DOTween.Sequence();

            sequence
                .Append(cardSource.Owner.TrashHandCard.transform.DOScale(new Vector3(1.4f, 1.4f, 1), 0.3f))
                .Join(cardSource.Owner.TrashHandCard.transform.DOLocalMove(targetPosition, 0.3f))
                .AppendCallback(() => end = true);

            sequence.Play();

            yield return new WaitWhile(() => !end);
            end = false;

            sequence = DOTween.Sequence();

            sequence
                .Append(cardSource.Owner.TrashHandCard.transform.DOScale(new Vector3(2f, 2f, 1), 0.3f))
                .AppendCallback(() => end = true);
            sequence.Play();

            GameObject Effect = Instantiate(FieldUnitEffectPrefab);
            Effect.transform.position = cardSource.Owner.TrashHandCard.transform.position + new Vector3(0, 1, 0);

            yield return new WaitWhile(() => !end);
            end = false;

            yield return new WaitForSeconds(0.3f);

            sequence = DOTween.Sequence();

            sequence
                   .Append(cardSource.Owner.TrashHandCard.transform.DOScale(new Vector3(1.4f, 1.4f, 1), 0.1f))
                   .AppendCallback(() => end = true);

            sequence.Play();

            StartCoroutine(DeleteCoroutine(Effect));
        }
    }
    #endregion

    #region 支援スキルが発動した時のエフェクト
    public IEnumerator ActivateSupportCardSkillEffect(CardSource cardSource,ICardEffect cardEffect)
    {
        bool end = false;
        var sequence = DOTween.Sequence();

        if(cardSource.Owner.SupportCards.Contains(cardSource))
        {
            if(cardSource.Owner.SupportHandCard.cardSource == cardSource)
            {
                cardSource.Owner.SupportHandCard.SetSkillName(cardEffect);
                sequence = DOTween.Sequence();

                sequence
                    .Append(cardSource.Owner.SupportHandCard.transform.DOScale(new Vector3(1.5f,1.5f,1),0.3f))
                    .AppendCallback(() => end = true);

                sequence.Play();

                GameObject Effect = Instantiate(FieldUnitEffectPrefab);
                Effect.transform.position = cardSource.Owner.SupportHandCard.transform.position + new Vector3(0,1, 0);

                yield return new WaitWhile(() => !end);
                end = false;

                yield return new WaitForSeconds(0.3f);

                sequence
                    .Append(cardSource.Owner.SupportHandCard.transform.DOScale(new Vector3(1f, 1f, 1), 0.1f))
                    .AppendCallback(() => end = true);

                sequence.Play();

                StartCoroutine(DeleteCoroutine(Effect));
            }
        }
    }
    #endregion

    #region 手札のカードの効果が発動した時のエフェクト
    public IEnumerator ActivateHandCardSkillEffect(CardSource cardSource, ICardEffect cardEffect)
    {
        bool end = false;
        var sequence = DOTween.Sequence();

        float targetPivotY = 0;

        if(cardSource.Owner.isYou)
        {
            targetPivotY = 0.08f;
        }

        else
        {
            targetPivotY = 1.2f;
        }

        if (cardSource.Owner.HandCards.Contains(cardSource))
        {
            if (cardSource.ShowingHandCard != null)
            {
                foreach(HandCard handCard in GManager.instance.You.HandCardObjects)
                {
                    handCard.GetComponent<Draggable_HandCard>().CanPointerEnterExitAction = false;
                }

                cardSource.ShowingHandCard.SetSkillName(cardEffect);
                cardSource.ShowingHandCard.ShowOpponent = true;
                cardSource.ShowingHandCard.Outline_Select.SetActive(true);
                cardSource.ShowingHandCard.SetOrangeOutline();

                sequence = DOTween.Sequence();

                sequence
                    .Append(cardSource.ShowingHandCard.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.3f))
                    .Join(DOTween.To(() => cardSource.ShowingHandCard.GetComponent<RectTransform>().pivot,(x) => cardSource.ShowingHandCard.GetComponent<RectTransform>().pivot = x,new Vector2(0.5f, targetPivotY),0.15f))
                    .AppendCallback(() => end = true);

                sequence.Play();

                GameObject Effect = Instantiate(FieldUnitEffectPrefab);
                Effect.transform.position = cardSource.ShowingHandCard.transform.position + new Vector3(0, 1, 0);

                yield return new WaitWhile(() => !end);
                end = false;

                cardSource.ShowingHandCard.GetComponent<RectTransform>().pivot = new Vector2(0.5f, targetPivotY);
                yield return new WaitForSeconds(0.3f);

                foreach (HandCard handCard in GManager.instance.You.HandCardObjects)
                {
                    handCard.GetComponent<Draggable_HandCard>().CanPointerEnterExitAction = true;
                }
            }
        }
    }
    #endregion

    #region フィールドユニットカード生成時のエフェクト
    [Header("新ユニット生成時着地エフェクト")]
    public GameObject NewUnitEffect_OnLand;

    [Header("ユニット重ねる時エフェクト")]
    public GameObject EvolutionUnitEffect;

    [Header("赤ユニット重ねる時エフェクト")]
    public GameObject RedEvolutionEffect;

    [Header("青ユニット重ねる時エフェクト")]
    public GameObject BlueEvolutionEffect;

    [Header("緑ユニット重ねる時エフェクト")]
    public GameObject GreenEvolutionEffect;

    [Header("黄ユニット重ねる時エフェクト")]
    public GameObject YellowEvolutionEffect;

    [Header("紫ユニット重ねる時エフェクト")]
    public GameObject PurpleEvolutionEffect;

    [Header("白ユニット重ねる時エフェクト")]
    public GameObject WhiteEvolutionEffect;

    [Header("黒ユニット重ねる時エフェクト")]
    public GameObject BlackEvolutionEffect;

    [Header("茶ユニット重ねる時エフェクト")]
    public GameObject BrownEvolutionEffect;

    [Header("無色ユニット重ねる時エフェクト")]
    public GameObject ColorlessEvolutionEffect;


    public IEnumerator CreateFieldUnitCardEffect(FieldUnitCard fieldUnitCard)
    {
        XAxisLayoutGroup3D xAxisLayoutGroup3D = fieldUnitCard.transform.parent.GetComponent<XAxisLayoutGroup3D>();

        if (xAxisLayoutGroup3D != null)
        {
            yield return new WaitForSeconds(Time.deltaTime * 2);

            bool end = false;

            xAxisLayoutGroup3D.enabled = false;

            float fallTime = 0.25f;

            fieldUnitCard.transform.localPosition = new Vector3(fieldUnitCard.transform.localPosition.x, 30, fieldUnitCard.transform.localPosition.z);

            var sequence = DOTween.Sequence();

            sequence
                .Append(DOTween.To(() => fieldUnitCard.transform.localPosition, (x) => fieldUnitCard.transform.localPosition = x, new Vector3(fieldUnitCard.transform.localPosition.x, xAxisLayoutGroup3D.center.y, fieldUnitCard.transform.localPosition.z), fallTime).SetEase(Ease.OutBounce))
                .AppendCallback(() => { end = true; });

            sequence.Play();

            while (Mathf.Abs(fieldUnitCard.transform.localPosition.y - xAxisLayoutGroup3D.center.y) < 1)
            {
                yield return null;
            }

            //振動
            GManager.instance.GetComponent<CinemachineImpulseSource>().GenerateImpulseAt(Vector3.zero, new Vector3(5, 5, 5));

            //エフェクト生成
            GameObject effect = Instantiate(NewUnitEffect_OnLand);
            effect.transform.position = new Vector3(fieldUnitCard.transform.position.x, 0.05f, fieldUnitCard.transform.position.z);
            StartCoroutine(DeleteCoroutine(effect));

            GameObject effect2 = null;

            if (fieldUnitCard.thisUnit.Character != null)
            {
                switch (fieldUnitCard.thisUnit.Character.cEntity_Base.cardColors[0])
                {
                    case CardColor.Red:
                        effect2 = Instantiate(RedEvolutionEffect);
                        break;

                    case CardColor.Blue:
                        effect2 = Instantiate(BlueEvolutionEffect);
                        break;

                    case CardColor.Green:
                        effect2 = Instantiate(GreenEvolutionEffect);
                        break;

                    case CardColor.Yellow:
                        effect2 = Instantiate(YellowEvolutionEffect);
                        break;

                    case CardColor.Purple:
                        effect2 = Instantiate(PurpleEvolutionEffect);
                        break;

                    case CardColor.Black:
                        effect2 = Instantiate(BlackEvolutionEffect);
                        break;

                    case CardColor.White:
                        effect2 = Instantiate(WhiteEvolutionEffect);
                        break;

                    case CardColor.Brown:
                        effect2 = Instantiate(BrownEvolutionEffect);
                        break;

                    case CardColor.Colorless:
                        effect2 = Instantiate(ColorlessEvolutionEffect);
                        break;
                }

                effect2.transform.position = new Vector3(fieldUnitCard.transform.position.x, 0.05f, fieldUnitCard.transform.position.z);
                effect2.transform.localScale = new Vector3(4, 1, 4);
                StartCoroutine(DeleteCoroutine(effect2));

                while (!end)
                {
                    yield return null;
                }

                end = false;
            }

            yield return new WaitForSeconds(0.2f);

            xAxisLayoutGroup3D.enabled = true;
        }
    }
    #endregion

    #region フィールドユニットカードに重ねてプレイする時のエフェクト
    public IEnumerator EvolutionFieldUnitCardEffect(CardSource soulCard, FieldUnitCard targetFieldUnitCard)
    {
        XAxisLayoutGroup3D xAxisLayoutGroup3D = targetFieldUnitCard.transform.parent.GetComponent<XAxisLayoutGroup3D>();

        if (xAxisLayoutGroup3D != null)
        {
            yield return new WaitForSeconds(Time.deltaTime * 2);

            List<CardSource> characters = new List<CardSource>();

            characters.Add(soulCard);

            foreach (CardSource cardSource in targetFieldUnitCard.thisUnit.Characters)
            {
                characters.Add(cardSource);
            }

            Unit unit = new Unit(characters);

            unit.IsTapped = targetFieldUnitCard.thisUnit.IsTapped;

            FieldUnitCard fieldUnitCard = Instantiate(GManager.instance.fieldUnitCardPrefab, targetFieldUnitCard.transform.parent);
            fieldUnitCard.anim.enabled = false;
            fieldUnitCard.Collider.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled = false;

            fieldUnitCard.SetFieldUnitCard(unit);

            if(unit.IsTapped)
            {
                fieldUnitCard.transform.localRotation = Quaternion.Euler(90,20,0);
            }

            bool end = false;

            xAxisLayoutGroup3D.enabled = false;

            float fallTime = 0.25f;

            fieldUnitCard.transform.localPosition = new Vector3(targetFieldUnitCard.transform.localPosition.x, 30, targetFieldUnitCard.transform.localPosition.z);

            var sequence = DOTween.Sequence();

            sequence
                .Append(DOTween.To(() => fieldUnitCard.transform.localPosition, (x) => fieldUnitCard.transform.localPosition = x, new Vector3(fieldUnitCard.transform.localPosition.x, xAxisLayoutGroup3D.center.y, fieldUnitCard.transform.localPosition.z), fallTime).SetEase(Ease.OutBounce))
                .AppendCallback(() => { end = true; });

            sequence.Play();

            //エフェクト生成

            GameObject PlaySoulEffect = Instantiate(EvolutionUnitEffect);
            PlaySoulEffect.transform.position = new Vector3(fieldUnitCard.transform.position.x, 0.2f, fieldUnitCard.transform.position.z);
            PlaySoulEffect.transform.localScale = new Vector3(7, 1, 7);
            StartCoroutine(DeleteCoroutine(PlaySoulEffect));

            GameObject PlaySoulEffect2 = null;

            switch (soulCard.cEntity_Base.cardColors[0])
            {
                case CardColor.Red:
                    PlaySoulEffect2 = Instantiate(RedEvolutionEffect);
                    break;

                case CardColor.Blue:
                    PlaySoulEffect2 = Instantiate(BlueEvolutionEffect);
                    break;

                case CardColor.Green:
                    PlaySoulEffect2 = Instantiate(GreenEvolutionEffect);
                    break;

                case CardColor.Yellow:
                    PlaySoulEffect2 = Instantiate(YellowEvolutionEffect);
                    break;

                case CardColor.Purple:
                    PlaySoulEffect2 = Instantiate(PurpleEvolutionEffect);
                    break;

                case CardColor.Black:
                    PlaySoulEffect2 = Instantiate(BlackEvolutionEffect);
                    break;

                case CardColor.White:
                    PlaySoulEffect2 = Instantiate(WhiteEvolutionEffect);
                    break;

                case CardColor.Brown:
                    PlaySoulEffect2 = Instantiate(BrownEvolutionEffect);
                    break;

                case CardColor.Colorless:
                    PlaySoulEffect2 = Instantiate(ColorlessEvolutionEffect);
                    break;
            }
            //PlaySoulEffect2 = Instantiate(PlayerDamageEffect);
            PlaySoulEffect2.transform.position = new Vector3(fieldUnitCard.transform.position.x, 0.15f, fieldUnitCard.transform.position.z);
            PlaySoulEffect2.transform.localScale = new Vector3(6, 1, 6);
            StartCoroutine(DeleteCoroutine(PlaySoulEffect2));

            while (Mathf.Abs(fieldUnitCard.transform.localPosition.y - (xAxisLayoutGroup3D.center.y)) < 1)
            {
                yield return null;
            }

            targetFieldUnitCard.Parent.SetActive(false);

            //振動
            GManager.instance.GetComponent<CinemachineImpulseSource>().GenerateImpulseAt(Vector3.zero, new Vector3(5, 5, 5));

            //エフェクト生成
            GameObject effect = Instantiate(NewUnitEffect_OnLand);
            effect.transform.position = new Vector3(fieldUnitCard.transform.position.x, 0.05f, fieldUnitCard.transform.position.z);
            StartCoroutine(DeleteCoroutine(effect));

            while (!end)
            {
                yield return null;
            }

            end = false;

            yield return new WaitForSeconds(0.55f);

            Destroy(fieldUnitCard.gameObject);

            targetFieldUnitCard.Parent.SetActive(true);

            if(targetFieldUnitCard.thisUnit.IsTapped)
            {
                //targetFieldUnitCard.transform.localRotation = Quaternion.Euler(90, 20, 0);
            }

            xAxisLayoutGroup3D.enabled = true;
        }
    }
    #endregion

    #region カードを公開する
    [Header("カード公開親")]
    public Transform ShowCardParent;

    [Header("カード公開テキスト")]
    public Text ShowCardTitleText;
    public IEnumerator ShowCardEffect(List<CardSource> ShownCards,string Title,bool willHide)
    {
        ShowCardTitleText.text = Title;

        for(int i=0;i<ShowCardParent.childCount;i++)
        {
            Destroy(ShowCardParent.GetChild(i).gameObject);
        }

        yield return new WaitForSeconds(Time.deltaTime * 2);

        foreach(CardSource cardSource in ShownCards)
        {
            HandCard handCard = Instantiate(GManager.instance.handCardPrefab, ShowCardParent);
            yield return new WaitUntil(() => handCard != null);
            handCard.SetUpHandCard(cardSource);
            handCard.SetUpHandCardImage();
        }

        ShowCardParent.transform.parent.parent.gameObject.SetActive(true);
        ShowCardParent.transform.parent.gameObject.SetActive(true);
        ShowCardParent.transform.parent.GetComponent<Animator>().SetInteger("Close", 0);

        if(willHide)
        {
            yield return new WaitForSeconds(3f);

            OffShowCard();
        }
    }

    public void OffShowCard()
    {
        ShowCardParent.transform.parent.gameObject.SetActive(false);
    }
    #endregion

    #region カードをドローした時のエフェクト
    [Header("中央のカード表示エフェクト")]
    public GameObject ShowUseHandCardEffectPrefab;

    [Header("使用カード拡大表示HandCard2")]
    public HandCard ShowUseHandCard;

    [Header("使用カード拡大表示HandCard親")]
    public Transform ShowUseHandCardParent;
    public IEnumerator AddHandCardEffect(CardSource cardSource)
    {
        Player player = cardSource.Owner;

        bool end = false;

        var sequence = DOTween.Sequence();
        var sequence2 = DOTween.Sequence();

        ShowUseHandCardParent.parent.gameObject.SetActive(true);
        ShowUseHandCardParent.gameObject.SetActive(true);
        ShowUseHandCard.gameObject.SetActive(true);
        ShowUseHandCard.transform.SetParent(ShowUseHandCardParent);
        
        ShowUseHandCard.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        ShowUseHandCard.OnRemoveSelect();

        Vector3 targetPositon = new Vector3(150, 0, 0);

        if (cardSource.Owner.isYou)
        {
            targetPositon -= new Vector3(0, 30, 0);
            ShowUseHandCard.transform.localPosition = new Vector3(210, 10, 0);
            ShowUseHandCard.SetUpHandCard(cardSource);
            ShowUseHandCard.SetUpHandCardImage();
            ShowUseHandCard.CardImage.color = new Color(1, 1, 1, 1);
        }

        else
        {
            targetPositon += new Vector3(0, 30, 0);
            ShowUseHandCard.transform.localPosition = new Vector3(210, 30, 0);
            ShowUseHandCard.SetUpReverseCard();
        }

        ShowUseHandCard.CardImage.color = new Color(0, 0, 0, 1);
        ShowUseHandCard.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -60));

        float time = 0.15f;

        sequence = DOTween.Sequence();

        sequence
            .Append(ShowUseHandCard.transform.DOLocalMove(targetPositon, time))
            .Join(ShowUseHandCard.transform.DOScale(new Vector3(0.45f, 0.45f, 0.45f), time))
            .Join(ShowUseHandCard.transform.DOLocalRotate(Vector3.zero, time))
            .AppendCallback(() => { end = true; });

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;

        if (player.isYou)
        {
            ShowUseHandCard.SetUpHandCard(cardSource);
            ShowUseHandCard.SetUpHandCardImage();
            ShowUseHandCard.CardImage.color = new Color(0, 0, 0, 1);

            #region エフェクト生成
            GameObject effect = Instantiate(ShowUseHandCardEffectPrefab, ShowUseHandCardParent);
            effect.transform.localScale *= 0.5f;
            effect.transform.localPosition = ShowUseHandCard.transform.localPosition;
            effect.transform.SetSiblingIndex(0);
            StartCoroutine(DeleteCoroutine(effect));

            GameObject effect2 = Instantiate(ShowUseHandCardEffectPrefab, ShowUseHandCardParent);
            effect2.transform.localScale *= 0.5f;
            effect2.transform.localPosition = ShowUseHandCard.transform.localPosition;
            effect2.transform.SetSiblingIndex(0);
            effect2.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            StartCoroutine(DeleteCoroutine(effect2));
            #endregion

            yield return new WaitForSeconds(0.12f);
        }

        yield return new WaitForSeconds(0.12f);

        //縮小して上に上がる
        StartCoroutine(ShrinkUpUseHandCard(ShowUseHandCard));

        yield return new WaitForSeconds(0.18f);

    }
    #endregion

    #region 表示したカードが縮小して上に上がる
    public IEnumerator ShrinkUpUseHandCard(HandCard handCard)
    {
        bool end = false;

        var sequence = DOTween.Sequence();

        handCard.gameObject.SetActive(true);

        #region 縮小
        float shrinkTime2 = 0.15f;

        sequence = DOTween.Sequence();

        sequence
            .Append(handCard.transform.DOScaleX(0.06f, shrinkTime2))
            .Join(handCard.transform.DOScaleY(1.4f, shrinkTime2))
            .Join(DOTween.To(() => handCard.CardImage.color, (x) => handCard.CardImage.color = x, new Color32(205, 205, 205, 255), shrinkTime2))
            .AppendCallback(() => { end = true; });

        sequence.Play();

        yield return new WaitForSeconds(shrinkTime2 / 2);

        handCard.CardImage.sprite = null;

        while (!end)
        {
            yield return null;
        }

        end = false;
        #endregion

        #region 上に上がる
        float upTime = 0.12f;

        handCard.transform.DOLocalMoveY(220, upTime);

        yield return new WaitForSeconds(upTime);

        #endregion

        handCard.gameObject.SetActive(false);
    }
    #endregion

    #region プレイするカードを中央に表示
    public IEnumerator ShowUseHandCardEffect_PlayUnit(CardSource card)
    {
        yield return StartCoroutine(ShowUseHandCardEffect(card));

        yield return StartCoroutine(ShrinkUpUseHandCard(ShowUseHandCard));
    }
    #endregion

    #region プレイするカードを中央に表示
    public IEnumerator ShowUseHandCardEffect(CardSource card)
    {
        bool end = false;

        var sequence = DOTween.Sequence();

        #region エフェクト生成
        GameObject effect = Instantiate(ShowUseHandCardEffectPrefab, ShowUseHandCardParent);
        effect.transform.SetSiblingIndex(0);
        StartCoroutine(DeleteCoroutine(effect));

        GameObject effect2 = Instantiate(ShowUseHandCardEffectPrefab, ShowUseHandCardParent);
        effect2.transform.SetSiblingIndex(0);
        effect2.transform.localRotation = Quaternion.EulerAngles(new Vector3(0, 90, 0));
        StartCoroutine(DeleteCoroutine(effect2));
        #endregion

        #region 白いカードが回転
        ShowUseHandCard.gameObject.SetActive(true);
        ShowUseHandCard.transform.SetParent(ShowUseHandCardParent);
        ShowUseHandCard.transform.parent.gameObject.SetActive(true);

        ShowUseHandCard.transform.localPosition = Vector3.zero;
        ShowUseHandCard.transform.localScale = new Vector3(1, 1, 1);

        ShowUseHandCard.CardImage.sprite = null;
        ShowUseHandCard.PlayCostBackGround[0].transform.parent.gameObject.SetActive(false);
        ShowUseHandCard.CCCostBackGround.transform.parent.gameObject.SetActive(false);

        ShowUseHandCard.CardImage.color = new Color32(255, 255, 255, 140);

        ShowUseHandCard.transform.localRotation = Quaternion.Euler(0, 35, 0);

        float rotateTime = 0.2f;

        sequence = DOTween.Sequence();

        sequence
            .Append(ShowUseHandCard.transform.DOLocalRotate(new Vector3(0, 0, 0), rotateTime, RotateMode.FastBeyond360))
            .Join(DOTween.To(() => ShowUseHandCard.CardImage.color, (x) => ShowUseHandCard.CardImage.color = x, new Color(1, 1, 1, 0), rotateTime))
            .AppendCallback(() => { end = true; });

        sequence.Play();

        while (!end)
        {
            yield return null;
        }

        end = false;
        #endregion

        #region カードを表示
        ShowUseHandCard.CardImage.color = new Color(0, 0, 0, 0);

        ShowUseHandCard.SetUpHandCard(card);
        ShowUseHandCard.SetUpHandCardImage();
        ShowUseHandCard.CardImage.color = new Color(1, 1, 1, 1);

        ShowUseHandCard.Outline_Select.SetActive(true);
        ShowUseHandCard.SetOutlineColor(DataBase.CardColor_ColorLightDictionary[card.cEntity_Base.cardColors[0]]);

        float showCardTime = 0.2f;

        sequence = DOTween.Sequence();

        sequence
            .Append(DOTween.To(() => ShowUseHandCard.CardImage.color, (x) => ShowUseHandCard.CardImage.color = x, new Color(0, 0, 0, 1), showCardTime))
            .AppendCallback(() => { end = true; });

        sequence.Play();

        while (!end)
        {
            yield return null;
        }

        end = false;
        #endregion

        

        yield return new WaitForSeconds(0.3f);
    }
    #endregion

    #region 攻撃アニメーション
    [Header("射程攻撃エフェクト")]
    public GameObject RangeAttackEffect;

    public IEnumerator AttackAnimationCoroutine()
    {
        bool end = false;
        var sequence = DOTween.Sequence();

        if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
        {
            if(GManager.instance.turnStateMachine.AttackingUnit.Character != null && GManager.instance.turnStateMachine.DefendingUnit.Character != null)
            {
                if(GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard != null && GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard != null)
                {
                    FieldUnitCard Attacker = GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard;
                    
                    FieldUnitCard Defender = GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard;

                    Vector3 TargetPosition = GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.transform.position;

                    Vector3 oldPosition = Attacker.transform.position;

                    float GoTime = 0.3f;

                    float BackTime = 0.25f;

                    float BackDelay = 0;

                    #region 射程1攻撃
                    if (new CheckRange(Attacker.thisUnit,Defender.thisUnit).Distance() <= 1)
                    {
                        XAxisLayoutGroup3D xAxisLayoutGroup3D = null;

                        if (Attacker.thisUnit.Character.Owner.GetFrontUnits().Contains(Attacker.thisUnit))
                        {
                            xAxisLayoutGroup3D = GManager.instance.turnStateMachine.gameContext.TurnPlayer.FrontZoneTransform.GetComponent<XAxisLayoutGroup3D>();
                        }

                        else
                        {
                            xAxisLayoutGroup3D = GManager.instance.turnStateMachine.gameContext.TurnPlayer.BackZoneTransform.GetComponent<XAxisLayoutGroup3D>();
                        }

                        sequence
                                .OnStart(() =>
                                {
                                    xAxisLayoutGroup3D.enabled = false;
                                    Attacker.transform.position += new Vector3(0, 0.2f, 0);
                                })
                                .Append(Attacker.transform.DOMove(TargetPosition, GoTime).SetEase(Ease.InOutBack, 4.5f))
                                .Append(Attacker.transform.DOMove(oldPosition, BackTime).SetEase(Ease.OutCubic).SetDelay(BackDelay))
                                .AppendCallback(() =>
                                {
                                    Attacker.transform.position -= new Vector3(0, 0.2f, 0);
                                    xAxisLayoutGroup3D.enabled = true;
                                    end = true;
                                });

                        sequence.Play();

                        yield return new WaitWhile(() => Mathf.Abs(Attacker.transform.position.z - TargetPosition.z) < 0.1f);
                    }
                    #endregion

                    #region 射程2以上攻撃
                    else
                    {
                        GameObject rangeAttackEffect = CreateUnitBattleEffect(RangeAttackEffect, oldPosition);
                        Vector3 targetScale = rangeAttackEffect.transform.localScale * 1.1f;

                        sequence
                                .OnStart(() =>
                                {
                                    rangeAttackEffect.transform.position += new Vector3(0, 0.2f, 0);
                                    rangeAttackEffect.transform.localScale = new Vector3(0, 0, 0);
                                })
                                .Append(rangeAttackEffect.transform.DOScale(targetScale, BackTime))
                                .Append(rangeAttackEffect.transform.DOMove(TargetPosition, 0.4f))
                                .AppendCallback(() =>
                                {
                                    rangeAttackEffect.SetActive(false);
                                    end = true;
                                });

                        sequence.Play();

                        yield return new WaitWhile(() => Mathf.Abs(rangeAttackEffect.transform.position.z - TargetPosition.z) < 0.1f);
                        yield return new WaitForSeconds(0.3f);
                    }
                    #endregion
                }
            }
        }

        //振動
        GManager.instance.GetComponent<CinemachineImpulseSource>().GenerateImpulseAt(Vector3.zero, new Vector3(5, 5, 5));

        #region ダメージエフェクト
        yield return new WaitForSeconds(0.3f);

        bool NotHit = GetComponent<Critical_Evasion>().DiscardCard != null && GManager.instance.turnStateMachine.AttackingUnit.CanBeEvaded(GManager.instance.turnStateMachine.DefendingUnit);

        if(!NotHit && GManager.instance.turnStateMachine.AttackingUnit.Power >= GManager.instance.turnStateMachine.DefendingUnit.Power)
        {
            Vector3 effectPosition = GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.transform.position + new Vector3(0, 3, 0);

            CreateUnitBattleEffect(UnitBattleEffect, effectPosition);
        }
        #endregion

        yield return new WaitWhile(() => !end);

        yield return new WaitForSeconds(0.2f);
    }
    #endregion

    #region ユニット同士のバトルエフェクト
    [Header("ユニットバトルエフェクト")]
    public GameObject UnitBattleEffect;

    public GameObject CreateUnitBattleEffect(GameObject EffectPrefab, Vector3 position)
    {
        GameObject effect = Instantiate(EffectPrefab);

        effect.transform.localScale *= 6;

        effect.transform.position = position;

        StartCoroutine(DeleteCoroutine(effect));

        return effect;
    }
    #endregion
}
