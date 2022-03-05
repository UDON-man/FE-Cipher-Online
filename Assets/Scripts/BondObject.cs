using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
public class BondObject : MonoBehaviour
{
    public Text BondText;

    public Text FaceBondText;

    int showingBond = 0;

    int MaxBond = 0;

    public Player player;

    [SerializeField] float waitTime = 0.3f;

    Vector3 oldScale;

    [Header("コスト支払い時エフェクト")]
    public GameObject PayCostEffect;

    private void Start()
    {
        ResetBondObject();

        oldScale = transform.localScale;

        PayCostEffect.SetActive(false);

        BondText.text = "";
    }

    private void Update()
    {
        FaceBondText.transform.parent.gameObject.SetActive(false);

        if (player != null)
        {
            if(GManager.instance.turnStateMachine != null)
            {
                if(GManager.instance.turnStateMachine.DoseStartGame)
                {
                    FaceBondText.transform.parent.gameObject.SetActive(true);
                    FaceBondText.text = $"×{player.BondCards.Count((cardSource) => !cardSource.IsReverse)}";
                }
            }
        }
    }

    public void ResetBondObject()
    {
        BondText.text = $"{player.BondCards.Count}/{player.BondCards.Count}";
        showingBond = 0;
        MaxBond = 0;
    }

    public IEnumerator CountBondPhaseCountUpMP(Player player)
    {
        showingBond = 0;

        yield return StartCoroutine(CountUpBond(player));

        MaxBond = showingBond;
    }

    public IEnumerator CountUpBond(Player player)
    {
        bool end = false;

        BondText.text = $"{showingBond}/{showingBond}";

        if (player.Bond > 0)
        {
            var sequence = DOTween.Sequence();

            Vector3 targetScale = oldScale * 1.3f;

            sequence.Append(DOTween.To(() => transform.localScale, (x) => transform.localScale = x, targetScale, waitTime * 2))
                        .Append(DOTween.To(() => transform.localScale, (x) => transform.localScale = x, oldScale, waitTime))
                        .AppendCallback(() => { end = true; sequence.Kill(); });

            sequence.Play();

            float _waitTime = waitTime / player.Bond;

            yield return new WaitForSeconds(_waitTime);

            while (showingBond < player.Bond)
            {
                showingBond++;

                BondText.text = $"{showingBond}/{showingBond}";

                yield return new WaitForSeconds(_waitTime);
            }

            BondText.text = $"{player.BondCards.Count - player.BondConsumed}/{player.BondCards.Count}";
            yield return new WaitWhile(() => !end);
            end = false;
        }
    }

    public IEnumerator CountDownBond(Player player)
    {
        bool end = false;

        var sequence = DOTween.Sequence();

        Vector3 targetScale = oldScale * 1.3f;

        sequence.Append(DOTween.To(() => transform.localScale, (x) => transform.localScale = x, targetScale, waitTime * 2))
                    .Append(DOTween.To(() => transform.localScale, (x) => transform.localScale = x, oldScale, waitTime))
                    .AppendCallback(() => { end = true; sequence.Kill(); });

        sequence.Play();

        PayCostEffect.SetActive(true);

        while (showingBond > player.Bond)
        {
            showingBond--;

            BondText.text = $"{showingBond}/{MaxBond}";

            yield return new WaitForSeconds(Time.deltaTime / 3);
        }

        BondText.text = $"{player.BondCards.Count - player.BondConsumed}/{player.BondCards.Count}";
        yield return new WaitWhile(() => !end);
        end = false;

        PayCostEffect.SetActive(false);
    }

    public IEnumerator SetBond_Skill(Player player)
    {
        bool end = false;

        var sequence = DOTween.Sequence();

        Vector3 targetScale = oldScale * 1.3f;

        sequence.Append(DOTween.To(() => transform.localScale, (x) => transform.localScale = x, targetScale, waitTime * 2))
                    .Append(DOTween.To(() => transform.localScale, (x) => transform.localScale = x, oldScale, waitTime))
                    .AppendCallback(() => { end = true; sequence.Kill(); });

        sequence.Play();

        PayCostEffect.SetActive(true);

        //BondText.text = $"{showingBond}/{player.BondCards.Count}";
        BondText.text = $"{player.BondCards.Count - player.BondConsumed}/{player.BondCards.Count}";
        yield return new WaitForSeconds(Time.deltaTime / 3);

        yield return new WaitWhile(() => !end);
        end = false;

        PayCostEffect.SetActive(false);
    }

    public void SetDefaultSize()
    {
        transform.localScale = oldScale;
    }

    public void SetExpandSize()
    {
        transform.localScale = oldScale * 1.3f;
    }
}
