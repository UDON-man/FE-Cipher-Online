using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CutIn : MonoBehaviour
{
    public Animator anim;
    public GameObject Critical;
    public GameObject Evasion;
    public Image CardImage;
    public CutInGif cutInGif;

    public IEnumerator OpenCutIn(Critical_EvasionMode mode,CardSource cardSource)
    {
        if (mode == Critical_EvasionMode.Critical)
        {
            cutInGif.SetRedBackGround();
        }

        else if (mode == Critical_EvasionMode.Evasion)
        {
            cutInGif.SetBlueBackGround();
        }

        this.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(true);
        CardImage.gameObject.SetActive(false);

        Critical.SetActive(false);
        Evasion.SetActive(false);
        
        CardImage.sprite = cardSource.cEntity_Base.CardImage;

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);

        yield return new WaitForSeconds(Time.deltaTime);

        yield return new WaitWhile(() => transform.localScale.y < 1);

        CardImage.gameObject.SetActive(true);

        if (mode == Critical_EvasionMode.Critical)
        {
            Critical.SetActive(true);
            Evasion.SetActive(false);
        }

        else if (mode == Critical_EvasionMode.Evasion)
        {
            Critical.SetActive(false);
            Evasion.SetActive(true);
        }

        //yield return new WaitWhile(() => CardImage.transform.localPosition.x > -1350);

        yield return new WaitForSeconds(0.7f);

        anim.SetInteger("Open", 0);
        anim.SetInteger("Close", 1);

        yield return new WaitWhile(() => this.gameObject.activeSelf);
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }
}
