using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActivateClass : ICardEffect, ActivateICardEffect
{
    public void SetUpActivateClass(Func<Hashtable, IEnumerator> ActivateCoroutine)
    {
        this.ActivateCoroutine = ActivateCoroutine;
    }

    Func<Hashtable,IEnumerator> ActivateCoroutine;
    public IEnumerator Activate(Hashtable hash)
    {
        if (ActivateCoroutine != null)
        {
            yield return ContinuousController.instance.StartCoroutine(ActivateCoroutine(hash));
        }
    }
}