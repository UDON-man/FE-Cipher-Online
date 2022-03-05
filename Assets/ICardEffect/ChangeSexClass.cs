using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ChangeSexClass : ICardEffect, IChangeSexEffect
{
    public void SetUpChangeSexClass(Func<List<Sex>, List<Sex>> ChangeSex)
    {
        this.ChangeSex = ChangeSex;
    }

    Func<List<Sex>,List<Sex>> ChangeSex { get; set; }

    public List<Sex> GetSex(List<Sex> sex)
    {
        sex = ChangeSex(sex);
        return sex;
    }
}