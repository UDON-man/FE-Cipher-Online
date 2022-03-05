using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Title : MonoBehaviour
{
    public Animator anim;

    public void OffTitle()
    {
        this.gameObject.SetActive(false);
    }

    public void SetUpTitle()
    {
        this.gameObject.SetActive(true);
        anim.enabled = true;
        HomeCharas.SetActive(true);
    }

    bool Clicked = false;
    public List<Text> titleTexts = new List<Text>();
    public CheckUpdate checkUpdate;
    public GameObject HomeCharas;
    public GameObject Parent;
    public GameObject ClickToStart;
    public void OnClick()
    {
        if(Clicked)
        {
            return;
        }

        Clicked = true;

        ContinuousController.instance.StartCoroutine(OnClickCoroutine());
    }

    IEnumerator OnClickCoroutine()
    {
        anim.enabled = false;
        ClickToStart.SetActive(false);

        float waitTime = 0.5f;

        var sequence1 = DOTween.Sequence();
        sequence1.Append(Parent.transform.DOLocalMoveY(200, waitTime));
        sequence1.Play();

        foreach (Text text in titleTexts)
        {
            var sequence = DOTween.Sequence();

            sequence
                .Append(DOTween.To(() => text.color, (x) => text.color = x, new Color(text.color.r, text.color.g, text.color.b, 0), waitTime));

            sequence.Play();
        }

        yield return new WaitForSeconds(waitTime + 0.2f);

        OffTitle();

        yield return ContinuousController.instance.StartCoroutine(checkUpdate.CheckUpdateCoroutine());

        Opening.instance.home.SetUpHome();
    }
}
