using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeckMode : MonoBehaviour
{
    [Header("デッキボタン")]
    public OpeningButton DeckButton;

    [Header("デッキ選択")]
    public SelectDeck selectDeck;

    [Header("デッキ編集")]
    public EditDeck editDeck;

    [Header("デッキリスト")]
    public DeckListPanel deckListPanel;

    bool first = false;

    public void OffDeck()
    {
        selectDeck.OffSelectDeck();

        editDeck.CreateDeckObject.SetActive(false);

        if(!first)
        {
            DeckButton.OnExit();
            first = true;
        }
        

        deckListPanel.Off();
    }

    public void SetUpDeckMode()
    {
        if(selectDeck.isOpen)
        {
            return;
        }

        editDeck.CreateDeckObject.SetActive(false);

        selectDeck.SetUpSelectDeck();
    }
}
