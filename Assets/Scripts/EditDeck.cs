using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Coffee.UIEffects;
using System;
using DG.Tweening;
using UnityEngine.Events;
public class EditDeck : MonoBehaviour
{
    [Header("デッキ作成オブジェクト")]
    public GameObject CreateDeckObject;

    [Header("カードプレハブ")]
    public CardPrefab_CreateDeck cardPrefab_CreateDeck;

    [Header("主人公のカードを置くScrollRect")]
    public ScrollRect MainCharacterScroll;

    [Header("カードプールのカードを置くScrollRect")]
    public ScrollRect CardPoolScroll;

    [Header("デッキのカードを置くScrollRect")]
    public ScrollRect DeckScroll;

    [Header("デッキ枚数表示テキスト")]
    public Text DeckCountText;

    [Header("読みこみ中オブジェクト")]
    public LoadingObject LoadingObjec;

    [Header("カード詳細表示")]
    public Image DetailCard;

    [Header("デッキ選択画面")]
    public SelectDeck selectDeck;

    [Header("バトルデッキ選択画面")]
    public SelectBattleDeck selectBattleDeck;

    [Header("検索")]
    public SearchCardList searchCardList;

    [Header("フィルター")]
    public FilterCardList filterCardList;

    [Header("フィルターパネル")]
    public FilterPanel filterPanel;

    [Header("デッキ名インプットフィールド")]
    public InputField DeckNameInputField;

    //編集中のデッキデータ
    public DeckData EdittingDeckData { get; set; }

    bool isFromSelectDeck = false;

    public UnityAction EndEditAction;

    public SwitchLanguage switchLanguage;

    #region カード詳細表示
    public void OffDetailCard()
    {
        DetailCard.GetComponent<DetailCard>().OffDetailCard();
    }

    public void OnDetailCard(CEntity_Base cEntity_Base,Vector3 position)
    {
        if(isDragging)
        {
            return;
        }

        DetailCard.GetComponent<DetailCard>().SetUpDetailCard(cEntity_Base, position);
    }
    #endregion

    int count = 0;
    int UpdateFrame = 3;
    private void Update()
    {
        #region 数フレームに一回だけ更新
        count++;

        if (count < UpdateFrame)
        {
            return;
        }

        else
        {
            count = 0;
        }
        #endregion

        ShowOnlyVisibleObjects();
    }

    public void ShowOnlyVisibleObjects()
    {
        if(DoneSetUp)
        {
            foreach (CardPrefab_CreateDeck cardPrefab_CreateDeck in cardPoolPrefabs)
            {
                if (!cardPrefab_CreateDeck.isActive)
                {
                    cardPrefab_CreateDeck.gameObject.SetActive(false);
                }

                else
                {
                    cardPrefab_CreateDeck.gameObject.SetActive(true);

                    if (!((RectTransform)cardPrefab_CreateDeck.transform).IsVisibleFrom(Opening.instance.MainCamera))
                    {
                        cardPrefab_CreateDeck.CardImage.transform.parent.gameObject.SetActive(false);
                        cardPrefab_CreateDeck.PlayCostText.transform.parent.gameObject.SetActive(false);
                        cardPrefab_CreateDeck.CCCostText.transform.parent.gameObject.SetActive(false);
                        
                    }

                    else
                    {
                        cardPrefab_CreateDeck.CardImage.transform.parent.gameObject.SetActive(true);
                        cardPrefab_CreateDeck.PlayCostText.transform.parent.gameObject.SetActive(true);
                        cardPrefab_CreateDeck.SetUpCardPrefab_CreateDeck(cardPrefab_CreateDeck.cEntity_Base);

                        if(EdittingDeckData != null)
                        {
                            cardPrefab_CreateDeck.CheckCover(EdittingDeckData);
                        }
                    }
                }
            }
        }
    }

    #region デッキ編集画面を閉じる
    public void CloseCreateDeck()
    {
        ContinuousController.instance.StartCoroutine(CloseCreateDeckCoroutine());
    }

    public IEnumerator CloseCreateDeckCoroutine()
    {
        yield return ContinuousController.instance.StartCoroutine(LoadingObjec.StartLoading("Now Loading"));

        if(EdittingDeckData.MainCharacter != null)
        {
            if(!EdittingDeckData.DeckCards().Contains(EdittingDeckData.MainCharacter))
            {
                EdittingDeckData.MainCharacterID = -1;
            }
        }

        ContinuousController.instance.SaveDeckDatas();

        for (int i = 0; i < MainCharacterScroll.content.childCount; i++)
        {
            Destroy(MainCharacterScroll.content.GetChild(i).gameObject);
        }

        yield return new WaitUntil(() => MainCharacterScroll.content.childCount == 0);

        for (int i = 0; i < DeckScroll.content.childCount; i++)
        {
            Destroy(DeckScroll.content.GetChild(i).gameObject);
        }

        #region ひとつ前の画面を表示
        if (isFromSelectDeck)
        {
            selectDeck.gameObject.SetActive(true);
            selectDeck.anim.SetInteger("Open", 1);
            selectDeck.anim.SetInteger("Close", 0);
        }
        
        else
        {
            selectBattleDeck.gameObject.SetActive(true);
            selectBattleDeck.anim.SetInteger("Open", 1);
            selectBattleDeck.anim.SetInteger("Close", 0);
        }
        #endregion

        foreach (CardPrefab_CreateDeck cardPrefab_CreateDeck in cardPoolPrefabs)
        {
            cardPrefab_CreateDeck.gameObject.SetActive(false);
        }

        yield return new WaitUntil(() => DeckScroll.content.childCount == 0);

        yield return new WaitForSeconds(0.1f);

        CreateDeckObject.SetActive(false);

        #region ひとつ前の画面のデッキ選択表示
        if (isFromSelectDeck)
        {
            selectDeck.ResetDeckInfoPanel();

            ContinuousController.instance.StartCoroutine(selectDeck.SetDeckList());

            for (int i = 0; i < selectDeck.deckInfoPrefabParentScroll.content.childCount; i++)
            {
                if (selectDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>() != null)
                {
                    if (selectDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().thisDeckData == EdittingDeckData)
                    {
                        selectDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().OnClick();
                    }
                }
            }
        }

        else
        {
            selectBattleDeck.SetDeckList();

            for (int i = 0; i < selectBattleDeck.deckInfoPrefabParentScroll.content.childCount; i++)
            {
                if (selectBattleDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>() != null)
                {
                    if (selectBattleDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().thisDeckData == EdittingDeckData)
                    {
                        selectBattleDeck.deckInfoPrefabParentScroll.content.GetChild(i).GetComponent<DeckInfoPrefab>().OnClick();
                    }
                }
            }
        }
        #endregion

        EndEditAction?.Invoke();

        EndEditAction = null;

        yield return ContinuousController.instance.StartCoroutine(LoadingObjec.EndLoading());
    }
    #endregion

    #region デッキ編集画面を開く
    public void SetUpCreateDeck(DeckData deckData, bool isFromSelectDeck)
    {
        this.isFromSelectDeck = isFromSelectDeck;
        ContinuousController.instance.StartCoroutine(SetUpCreateDeckCoroutine(deckData));
        switchLanguage.SetLanguage();
    }

    public IEnumerator SetUpCreateDeckCoroutine(DeckData deckData)
    {
        yield return ContinuousController.instance.StartCoroutine(LoadingObjec.StartLoading("Now Loading"));

        yield return new WaitForSeconds(0.2f);

        for(int i=0;i<draggable_CardParent.childCount;i++)
        {
            Destroy(draggable_CardParent.GetChild(i).gameObject);
        }

        DraggingCover.SetActive(false);

        CardPoolScroll.verticalNormalizedPosition = 1;
        DeckScroll.verticalNormalizedPosition = 1;

        deckData.DeckCardIDs = DeckData.GetDeckCardCodes(DeckData.SortedList(deckData.DeckCards()));

        EdittingDeckData = deckData;

        ContinuousController.instance.SaveDeckDatas();

        OffDetailCard();

        for (int i = 0; i < MainCharacterScroll.content.childCount; i++)
        {
            Destroy(MainCharacterScroll.content.GetChild(i).gameObject);
        }

        yield return new WaitUntil(() => MainCharacterScroll.content.childCount == 0);

        for (int i = 0; i < DeckScroll.content.childCount; i++)
        {
            Destroy(DeckScroll.content.GetChild(i).gameObject);
        }

        yield return new WaitUntil(() => DeckScroll.content.childCount == 0);

        for (int i = 0; i < DeckScroll.viewport.childCount; i++)
        {
            if (i != 0)
            {
                Destroy(DeckScroll.viewport.GetChild(i).gameObject);
            }
        }

        yield return new WaitUntil(() => DeckScroll.viewport.childCount == 1);

        if (deckData != null)
        {
            //デッキのカードのスクロールを表示
            List<CEntity_Base> NotMainCharacterDeckCards = new List<CEntity_Base>();

            List<CEntity_Base> MainCharacterDeckCards = new List<CEntity_Base>();

            foreach (CEntity_Base cEntity_Base in deckData.DeckCards())
            {
                NotMainCharacterDeckCards.Add(cEntity_Base);
            }

            if(deckData.MainCharacter != null)
            {
                if(deckData.DeckCards().Contains(deckData.MainCharacter))
                {
                    NotMainCharacterDeckCards.Remove(deckData.MainCharacter);
                    MainCharacterDeckCards.Add(deckData.MainCharacter);
                }
            }

            foreach (CEntity_Base cEntity_Base in NotMainCharacterDeckCards)
            {
                CreateDeckCard(cEntity_Base,false);
            }

            foreach (CEntity_Base cEntity_Base in MainCharacterDeckCards)
            {
                CreateDeckCard(cEntity_Base, true);
            }
        }

        CreateDeckObject.SetActive(true);

        foreach(CardPrefab_CreateDeck _CardPrefab_CreateDeck in cardPoolPrefabs)
        {
            if (_CardPrefab_CreateDeck != null)
            {
                _CardPrefab_CreateDeck.CheckCover(deckData);
            }
        }

        SetDeckCountText();

        foreach (CardPrefab_CreateDeck _CardPrefab_CreateDeck in cardPoolPrefabs)
        {
            _CardPrefab_CreateDeck.isActive = true;
        }

        searchCardList.Init();

        filterCardList.Init(ShowPoolCard_MatchCondition);
        filterPanel.Init();

        #region ひとつ前の画面を非表示
        selectDeck.gameObject.SetActive(false);
        selectBattleDeck.gameObject.SetActive(false);
        #endregion

        DeckNameInputField.onEndEdit.RemoveAllListeners();
        DeckNameInputField.text = EdittingDeckData.DeckName;
        DeckNameInputField.onEndEdit.AddListener(OnEndEdit);

        ShowOnlyVisibleObjects();
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(LoadingObjec.EndLoading());
    }

    public void OnEndEdit(string text)
    {
        string deckName = text;

        deckName = ValidateDeckName(deckName);

        EdittingDeckData.DeckName = deckName;

        DeckNameInputField.onEndEdit.RemoveAllListeners();
        DeckNameInputField.text = deckName;
        DeckNameInputField.onEndEdit.AddListener(OnEndEdit);
    }

    public static string ValidateDeckName(string deckName)
    {
        List<string> ErrorLetters = new List<string>()
        {
            ",",
            "_",
            "*",
            "/",
        };

        foreach(string error in ErrorLetters)
        {
            deckName.Replace(error, "");
        }

        return deckName;
    }
    #endregion

    #region ゲーム起動時にデッキ編集画面を初期化
    public List<CardPrefab_CreateDeck> cardPoolPrefabs_all = new List<CardPrefab_CreateDeck>();
    List<CardPrefab_CreateDeck> cardPoolPrefabs = new List<CardPrefab_CreateDeck>();
    bool DoneSetUp { get; set; } = false;
    public IEnumerator InitEditDeck()
    {
        for (int i = 0; i < DeckScroll.content.childCount; i++)
        {
            Destroy(DeckScroll.content.GetChild(i).gameObject);
        }

        for (int i = 0; i < MainCharacterScroll.content.childCount; i++)
        {
            Destroy(MainCharacterScroll.content.GetChild(i).gameObject);
        }

        foreach(CardPrefab_CreateDeck cardPrefab_CreateDeck in cardPoolPrefabs_all)
        {
            if (cardPrefab_CreateDeck.gameObject.activeSelf)
            {
                cardPrefab_CreateDeck.gameObject.SetActive(false);
            }
        }

        //カードプールのスクロールを表示
        for(int i=0;i< ContinuousController.instance.CardList.Count;i++)
        {
            if(i < cardPoolPrefabs_all.Count)
            {
                CardPrefab_CreateDeck _cardPrefab_CreateDeck = cardPoolPrefabs_all[i];
                cardPoolPrefabs.Add(_cardPrefab_CreateDeck);
                CEntity_Base cEntity_Base = ContinuousController.instance.CardList[i];

                foreach (ScrollRect _scroll in _cardPrefab_CreateDeck.scroll)
                {
                    _scroll.content = CardPoolScroll.content;

                    _scroll.viewport = CardPoolScroll.viewport;
                }

                _cardPrefab_CreateDeck.SetUpCardPrefab_CreateDeck(cEntity_Base);

                _cardPrefab_CreateDeck.OnClickAction = () =>
                {
                    if (Input.GetMouseButtonUp(1))
                    {
                        StartCoroutine(AddDeckCardCoroutine_OnClick(_cardPrefab_CreateDeck));
                    }
                };

                _cardPrefab_CreateDeck.OnBeginDragAction = (cardPrefab_CreateDeck) => { StartCoroutine(OnBeginDrag(cardPrefab_CreateDeck)); };

                _cardPrefab_CreateDeck.OnEnterAction = () =>
                {
                    OnDetailCard(cEntity_Base, new Vector3(-21.35f, 1, 99));
                };

                _cardPrefab_CreateDeck.OnExitAction = () => { OffDetailCard(); };
            }
        }

        yield return new WaitForSeconds(0.3f);

        DoneSetUp = true;
    }
    #endregion

    #region デッキ内容変更をUIに反映
    public IEnumerator ReflectDeckData()
    {
        if (EdittingDeckData != null)
        {
            //4枚以上入っているカードプールのカバーをオン
            foreach(CardPrefab_CreateDeck cardPrefab_CreateDeck in cardPoolPrefabs)
            {
                cardPrefab_CreateDeck.CheckCover(EdittingDeckData);
            }

            SetDeckCountText();

            yield return null;
        }
    }
    #endregion

    #region デッキ枚数テキストを表示
    public void SetDeckCountText()
    {
        int MainCharacterCount = 0;

        if(EdittingDeckData.MainCharacter != null)
        {
            if(EdittingDeckData.DeckCards().Contains(EdittingDeckData.MainCharacter))
            {
                MainCharacterCount = 1;
            }
        }

        DeckCountText.text = $"{EdittingDeckData.DeckCards().Count}/({EdittingDeckData.DeckCards().Count - MainCharacterCount}+{MainCharacterCount})";

        if (EdittingDeckData.IsValidDeckData())
        {
            DeckCountText.color = new Color32(69, 255, 69, 255);
        }

        else
        {
            DeckCountText.color = new Color32(255, 64, 64, 255);
        }
    }
    #endregion

    #region デッキカードを生成
    public CardPrefab_CreateDeck CreateDeckCard(CEntity_Base cEntity_Base,bool isMainCharacter)
    {
        CardPrefab_CreateDeck _cardPrefab_CreateDeck = null;

        if(!isMainCharacter)
        {
            _cardPrefab_CreateDeck = Instantiate(cardPrefab_CreateDeck, DeckScroll.content);
        }

        else
        {
            _cardPrefab_CreateDeck = Instantiate(cardPrefab_CreateDeck, MainCharacterScroll.content);
        }
        

        SetUpDeckCard(_cardPrefab_CreateDeck, cEntity_Base,isMainCharacter);

        return _cardPrefab_CreateDeck;
    }

    public void SetUpDeckCard(CardPrefab_CreateDeck _cardPrefab_CreateDeck, CEntity_Base cEntity_Base, bool isMainCharacter)
    {
        foreach (ScrollRect _scroll in _cardPrefab_CreateDeck.scroll)
        {
            if (!isMainCharacter)
            {
                _scroll.content = DeckScroll.content;

                _scroll.viewport = DeckScroll.viewport;
            }

            else
            {
                _scroll.content = MainCharacterScroll.content;

                _scroll.viewport = MainCharacterScroll.viewport;
            }  
        }

        _cardPrefab_CreateDeck.SetUpCardPrefab_CreateDeck(cEntity_Base);

        if(!isMainCharacter)
        {
            _cardPrefab_CreateDeck.OnClickAction = () =>
            {
                if (Input.GetMouseButtonUp(1))
                {
                    StartCoroutine(RemoveDeckCardCoroutine_OnClick(_cardPrefab_CreateDeck));
                }
            };
        }

        else
        {
            _cardPrefab_CreateDeck.OnClickAction = () =>
            {
                if (Input.GetMouseButtonUp(1))
                {
                    EdittingDeckData.ResetMainCharacter();
                    StartCoroutine(RemoveDeckCardCoroutine_OnClick(_cardPrefab_CreateDeck));
                }
            };
        }

        _cardPrefab_CreateDeck.OnBeginDragAction = (cardPrefab_CreateDeck) => { StartCoroutine(OnBeginDrag(cardPrefab_CreateDeck)); };

        _cardPrefab_CreateDeck.OnEnterAction = () => {
            //OnDetailCard(cEntity_Base, _cardPrefab_CreateDeck.transform.position + new Vector3(45, 0, 0));
            OnDetailCard(cEntity_Base, new Vector3(55f, 1, 99));
        };

        _cardPrefab_CreateDeck.OnExitAction = () => { OffDetailCard(); };

        _cardPrefab_CreateDeck.Parent.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
    #endregion

    #region 条件に合うカードだけをカード一覧に表示する

    public void ShowPoolCard_MatchCondition()
    {
        bool Condition(CEntity_Base cEntity_Base)
        {
            if(!searchCardList.OnlyContainsName()(cEntity_Base))
            {
                return false;
            }

            if(!filterCardList.OnlyContainsColor()(cEntity_Base))
            {
                return false;
            }

            if(!filterPanel.filterCardList.OnlyMatchPlayCost()(cEntity_Base))
            {
                return false;
            }

            if (!filterPanel.filterCardList.OnlyMatchRange()(cEntity_Base))
            {
                return false;
            }

            if (!filterPanel.filterCardList.OnlyMatchPower()(cEntity_Base))
            {
                return false;
            }

            if (!filterPanel.filterCardList.OnlyMatchSupportPower()(cEntity_Base))
            {
                return false;
            }

            return true;
        }

        for (int i = 0; i < cardPoolPrefabs.Count; i++)
        {
            CardPrefab_CreateDeck _cardPrefab_CreateDeck = cardPoolPrefabs[i];

            _cardPrefab_CreateDeck.isActive = Condition(_cardPrefab_CreateDeck.cEntity_Base);
        }

        ShowOnlyVisibleObjects();
    }
    #endregion

    #region カードオブジェクトの操作

    [Header("ドラッグ可能なカードオブジェクト")]
    public Draggable_Card draggable_CardPrefab;

    [Header("ドラッグ可能なカードオブジェクト親")]
    public Transform draggable_CardParent;

    [Header("メインキャラクターのドロップエリア")]
    public DropArea MainCharacterDropArea;

    [Header("デッキカードのドロップエリア")]
    public DropArea DeckCardsDropArea;

    [Header("カードプールのドロップエリア")]
    public DropArea CardPoolDropArea;

    [Header("ドラッグ中のカバー")]
    public GameObject DraggingCover;

    [Header("消えるデッキカード座標")]
    public Transform DisappearDeckCardTransform;

    //ドラッグ可能かどうか
    public bool CanDrag { get; set; } = true;

    //ドラッグ中
    public bool isDragging { get; set; } = false;

    #region ドラッグ可能なカードオブジェクトを生成
    public Draggable_Card CreateDraggable_Card()
    {
        Draggable_Card draggable_Card = Instantiate(draggable_CardPrefab, draggable_CardParent);

        draggable_Card.DefaultParent = draggable_CardParent;
        draggable_Card.transform.localScale = new Vector3(1, 1, 1);
        draggable_Card.CardImage.color = new Color(1, 1, 1, 1);

        return draggable_Card;
    }
    #endregion

    #region カードオブジェクトをドラッグ開始した時の処理
    public IEnumerator OnBeginDrag(CardPrefab_CreateDeck _cardPrefab_CreateDeck)
    {
        if (_cardPrefab_CreateDeck.transform.parent == CardPoolScroll.content)
        {
            if(EdittingDeckData.DeckCards().Count((cEntity_Base) => cEntity_Base.CardName == _cardPrefab_CreateDeck.cEntity_Base.CardName) >= _cardPrefab_CreateDeck.cEntity_Base.MaxCountInDeck)
            {
                yield break;
            }
        }

        if(Input.GetMouseButton(1))
        {
            yield break;
        }

        if (!isDragging && CanDrag)
        {
            Draggable_Card draggable_Card = CreateDraggable_Card();

            OffDetailCard();

            SetStartDrag(_cardPrefab_CreateDeck,draggable_Card);

            draggable_Card.OnDropAction = (dropAreas) => { OnEndDrag(dropAreas, _cardPrefab_CreateDeck,draggable_Card); };

            draggable_Card.OnDragAction = (dropAreas) =>
            {
                DeckCardsDropArea.OnPointerExit();
                CardPoolDropArea.OnPointerExit();
                MainCharacterDropArea.OnPointerExit();

                if (_cardPrefab_CreateDeck.transform.parent == MainCharacterScroll.content)
                {
                    if(dropAreas.Count((dropArea) => dropArea == DeckCardsDropArea) > 0)
                    {
                        DeckCardsDropArea.OnPointerEnter();
                    }

                    else if (dropAreas.Count((dropArea) => dropArea == CardPoolDropArea) > 0)
                    {
                        CardPoolDropArea.OnPointerEnter();
                    }
                }

                else if (_cardPrefab_CreateDeck.transform.parent == DeckScroll.content)
                {
                    if (dropAreas.Count((dropArea) => dropArea == MainCharacterDropArea) > 0)
                    {
                        if (_cardPrefab_CreateDeck.cEntity_Base.PlayCost == 1)
                        {
                            MainCharacterDropArea.OnPointerEnter();
                        }
                    }

                    else if (dropAreas.Count((dropArea) => dropArea == CardPoolDropArea) > 0)
                    {
                        CardPoolDropArea.OnPointerEnter();
                    }
                }

                else if (_cardPrefab_CreateDeck.transform.parent == CardPoolScroll.content)
                {
                    if (dropAreas.Count((dropArea) => dropArea == MainCharacterDropArea) > 0)
                    {
                        if (_cardPrefab_CreateDeck.cEntity_Base.PlayCost == 1)
                        {
                            MainCharacterDropArea.OnPointerEnter();
                        }
                    }

                    else if (dropAreas.Count((dropArea) => dropArea == DeckCardsDropArea) > 0)
                    {
                        DeckCardsDropArea.OnPointerEnter();
                    }
                }
            };

            if (_cardPrefab_CreateDeck.transform.parent == MainCharacterScroll.content)
            {
                MainCharacterDropArea.OffDropPanel();
                DeckCardsDropArea.OnDropPanel();
                CardPoolDropArea.OnDropPanel();

                _cardPrefab_CreateDeck.HideDeckCardTab();
            }

            else if(_cardPrefab_CreateDeck.transform.parent == DeckScroll.content)
            {
                if (_cardPrefab_CreateDeck.cEntity_Base.PlayCost == 1)
                {
                    MainCharacterDropArea.OnDropPanel();
                }

                else
                {
                    MainCharacterDropArea.OffDropPanel();
                }

                DeckCardsDropArea.OffDropPanel();
                CardPoolDropArea.OnDropPanel();

                _cardPrefab_CreateDeck.HideDeckCardTab();

                
            }

            else if (_cardPrefab_CreateDeck.transform.parent == CardPoolScroll.content)
            {
                if(_cardPrefab_CreateDeck.cEntity_Base.PlayCost == 1)
                {
                    MainCharacterDropArea.OnDropPanel();
                }

                else
                {
                    MainCharacterDropArea.OffDropPanel();
                }
                
                DeckCardsDropArea.OnDropPanel();
                CardPoolDropArea.OffDropPanel();
            }

            DeckScroll.enabled = false;

            for (int i = 0; i < DeckScroll.content.childCount; i++)
            {
                foreach (ScrollRect scroll in DeckScroll.content.GetChild(i).GetComponent<CardPrefab_CreateDeck>().scroll)
                {
                    scroll.enabled = false;
                }
            }

            CardPoolScroll.enabled = false;

            //for (int i = 0; i < CardPoolScroll.content.childCount; i++)
            for (int i = 0; i < cardPoolPrefabs.Count; i++)
            {
                //foreach (ScrollRect scroll in CardPoolScroll.content.GetChild(i).GetComponent<CardPrefab_CreateDeck>().scroll)
                foreach (ScrollRect scroll in cardPoolPrefabs[i].scroll)
                {
                    scroll.enabled = false;
                }
            }
        }
    }

    public void SetStartDrag(CardPrefab_CreateDeck _cardPrefab_CreateDeck,Draggable_Card draggable_Card)
    {
        isDragging = true;

        CanDrag = false;

        DraggingCover.SetActive(true);

        draggable_Card.SetUpDraggable_Card(_cardPrefab_CreateDeck.cEntity_Base, _cardPrefab_CreateDeck.transform.position);
    }
    #endregion

    #region カードオブジェクトをドラッグ終了した時の処理

    #region ドロップ時の処理
    public void OnEndDrag(List<DropArea> dropAreas, CardPrefab_CreateDeck _cardPrefab_CreateDeck,Draggable_Card draggable_Card)
    {
        MainCharacterDropArea.OffDropPanel();
        DeckCardsDropArea.OffDropPanel();
        CardPoolDropArea.OffDropPanel();

        StartCoroutine(OnEndDragCoroutine(dropAreas, _cardPrefab_CreateDeck,draggable_Card));

        DeckScroll.enabled = true;

        for (int i = 0; i < DeckScroll.content.childCount; i++)
        {
            foreach (ScrollRect scroll in DeckScroll.content.GetChild(i).GetComponent<CardPrefab_CreateDeck>().scroll)
            {
                scroll.enabled = true;
            }
        }

        CardPoolScroll.enabled = true;

        for (int i = 0; i < cardPoolPrefabs.Count; i++)
        {
            foreach (ScrollRect scroll in cardPoolPrefabs[i].scroll)
            {
                scroll.enabled = true;
            }
        }
    }

    IEnumerator OnEndDragCoroutine(List<DropArea> dropAreas, CardPrefab_CreateDeck _cardPrefab_CreateDeck,Draggable_Card draggable_Card)
    {
        if (_cardPrefab_CreateDeck.transform.parent == MainCharacterScroll.content)
        {
            if (dropAreas.Count((dropArea) => dropArea == CardPoolDropArea) > 0)
            {
                EdittingDeckData.ResetMainCharacter();
                yield return StartCoroutine(RemoveDeckCardCoroutine(_cardPrefab_CreateDeck, draggable_Card));
            }

            else if (dropAreas.Count((dropArea) => dropArea == DeckCardsDropArea) > 0)
            {
                EdittingDeckData.ResetMainCharacter();
                EdittingDeckData.RemoveDeckCard(_cardPrefab_CreateDeck.cEntity_Base);
                EdittingDeckData.DeckCardIDs = DeckData.GetDeckCardCodes(DeckData.SortedList(EdittingDeckData.DeckCards()));
                yield return StartCoroutine(AddDeckCardCoroutine(_cardPrefab_CreateDeck, draggable_Card));
            }

            else
            {
                _cardPrefab_CreateDeck.SetUpCardPrefab_CreateDeck(_cardPrefab_CreateDeck.cEntity_Base);
            }
        }

        else if (_cardPrefab_CreateDeck.transform.parent == DeckScroll.content)
        {
            if (dropAreas.Count((dropArea) => dropArea == CardPoolDropArea) > 0)
            {
                yield return StartCoroutine(RemoveDeckCardCoroutine(_cardPrefab_CreateDeck,draggable_Card));
            }

            else if (dropAreas.Count((dropArea) => dropArea == MainCharacterDropArea) > 0 && _cardPrefab_CreateDeck.cEntity_Base.PlayCost == 1)
            {
                yield return StartCoroutine(SetMainCharacterCoroutine(_cardPrefab_CreateDeck,draggable_Card));
            }

            else
            {
                _cardPrefab_CreateDeck.SetUpCardPrefab_CreateDeck(_cardPrefab_CreateDeck.cEntity_Base);
            }
        }

        else if (_cardPrefab_CreateDeck.transform.parent == CardPoolScroll.content)
        {
            if(EdittingDeckData.DeckCardIDs.Count < 99)
            {
                if (dropAreas.Count((dropArea) => dropArea == DeckCardsDropArea) > 0)
                {
                    yield return StartCoroutine(AddDeckCardCoroutine(_cardPrefab_CreateDeck, draggable_Card));
                }

                else if (dropAreas.Count((dropArea) => dropArea == MainCharacterDropArea) > 0 && _cardPrefab_CreateDeck.cEntity_Base.PlayCost == 1)
                {
                    yield return StartCoroutine(AddDeckCards(_cardPrefab_CreateDeck.cEntity_Base));
                    yield return StartCoroutine(SetMainCharacterCoroutine(_cardPrefab_CreateDeck, draggable_Card));
                }
            }
        }

        Reset_EndDrag(draggable_Card);
    }

    public void Reset_EndDrag(Draggable_Card draggable_Card)
    {
        isDragging = false;
        DestroyImmediate(draggable_Card.gameObject);
        CanDrag = true;
        DraggingCover.SetActive(false);
    }
    #endregion

    #region ドロップ時にデッキにカードを追加アニメーション
    IEnumerator AddDeckCardCoroutine(CardPrefab_CreateDeck _cardPrefab_CreateDeck,Draggable_Card draggable_Card)
    {
        if (EdittingDeckData.DeckCards().Count((cEntity_Base) => cEntity_Base == _cardPrefab_CreateDeck.cEntity_Base) >= _cardPrefab_CreateDeck.cEntity_Base.MaxCountInDeck)
        {
            yield break;
        }

        bool end = false;

        yield return StartCoroutine(AddDeckCards(_cardPrefab_CreateDeck.cEntity_Base));

        List<int> DeckCardIDs = new List<int>();

        foreach(int CardID in EdittingDeckData.DeckCardIDs)
        {
            DeckCardIDs.Add(CardID);
        }

        if(EdittingDeckData.MainCharacter != null)
        {
            if(EdittingDeckData.DeckCards().Contains(EdittingDeckData.MainCharacter))
            {
                DeckCardIDs.Remove(EdittingDeckData.MainCharacterID);
            }
        }

        int index = DeckCardIDs.IndexOf(_cardPrefab_CreateDeck.cEntity_Base.CardID) + DeckCardIDs.Count((cardID) => cardID == _cardPrefab_CreateDeck.cEntity_Base.CardID) - 1;

        CardPrefab_CreateDeck newCardPrefab = CreateDeckCard(_cardPrefab_CreateDeck.cEntity_Base,false);

        newCardPrefab.HideDeckCardTab();

        newCardPrefab.transform.SetSiblingIndex(index);

        yield return new WaitForSeconds(Time.deltaTime);

        var sequence = DOTween.Sequence();

        sequence
            .Append(draggable_Card.transform.DOMove(newCardPrefab.transform.position, 0.08f))
            .Join(draggable_Card.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.08f))
            .AppendCallback(() => end = true);

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;

        yield return new WaitForSeconds(0.08f);
        newCardPrefab.SetUpCardPrefab_CreateDeck(newCardPrefab.cEntity_Base);
        draggable_Card.gameObject.SetActive(false);

        if (_cardPrefab_CreateDeck.transform.parent == MainCharacterScroll.content)
        {
            DestroyImmediate(_cardPrefab_CreateDeck.gameObject);
        }
    }
    #endregion

    #region ドロップ時にデッキからカードを抜くアニメーション
    IEnumerator RemoveDeckCardCoroutine(CardPrefab_CreateDeck _cardPrefab_CreateDeck,Draggable_Card draggable_Card)
    {
        bool end = false;

        yield return StartCoroutine(RemoveDeckCards(_cardPrefab_CreateDeck.cEntity_Base));

        var sequence = DOTween.Sequence();

        sequence
            .Append(draggable_Card.transform.DOMove(DisappearDeckCardTransform.position, 0.08f))
            .Join(draggable_Card.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.08f))
            .AppendCallback(() => end = true);

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;

        yield return new WaitForSeconds(0.08f);
        draggable_Card.gameObject.SetActive(false);

        if (_cardPrefab_CreateDeck.transform.parent != CardPoolScroll.content)
        {
            DestroyImmediate(_cardPrefab_CreateDeck.gameObject);
        }
    }
    #endregion

    #region ドロップ時に主人公を設定するアニメーション
    IEnumerator SetMainCharacterCoroutine(CardPrefab_CreateDeck _cardPrefab_CreateDeck,Draggable_Card draggable_Card)
    {
        bool end = false;

        CardPrefab_CreateDeck oldCardPrefab = null;

        if (MainCharacterScroll.content.childCount > 0)
        {
            oldCardPrefab = MainCharacterScroll.content.GetChild(0).GetComponent<CardPrefab_CreateDeck>();
        }

        yield return StartCoroutine(SetMainCharacter(_cardPrefab_CreateDeck.cEntity_Base));

        CardPrefab_CreateDeck newCardPrefab = CreateDeckCard(_cardPrefab_CreateDeck.cEntity_Base,true);

        newCardPrefab.HideDeckCardTab();


        yield return new WaitForSeconds(Time.deltaTime);

        var sequence = DOTween.Sequence();

        sequence
            .Append(draggable_Card.transform.DOMove(newCardPrefab.transform.position, 0.08f))
            .Join(draggable_Card.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.08f))
            .AppendCallback(() => end = true);

        sequence.Play();

        if(oldCardPrefab != null)
        {
            oldCardPrefab.HideDeckCardTab();
            Draggable_Card draggable_Card1 = CreateDraggable_Card();
            draggable_Card1.SetUpDraggable_Card(oldCardPrefab.cEntity_Base, oldCardPrefab.transform.position);
            draggable_Card1.IsDragging = false;
            draggable_Card1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            EdittingDeckData.RemoveDeckCard(oldCardPrefab.cEntity_Base);
            SetDeckCountText();
            StartCoroutine(AddDeckCardCoroutine(oldCardPrefab, draggable_Card1));
        }

        yield return new WaitWhile(() => !end);
        end = false;

        yield return new WaitForSeconds(0.08f);
        newCardPrefab.SetUpCardPrefab_CreateDeck(newCardPrefab.cEntity_Base);
        draggable_Card.gameObject.SetActive(false);

        if(_cardPrefab_CreateDeck.transform.parent != CardPoolScroll.content)
        {
            DestroyImmediate(_cardPrefab_CreateDeck.gameObject);
        }
    }
    #endregion

    #region 右クリック時にデッキにカードを追加アニメーション
    public IEnumerator AddDeckCardCoroutine_OnClick(CardPrefab_CreateDeck _cardPrefab_CreateDeck)
    {
        if (EdittingDeckData.DeckCards().Count((cEntity_Base) => cEntity_Base.CardName == _cardPrefab_CreateDeck.cEntity_Base.CardName) >= _cardPrefab_CreateDeck.cEntity_Base.MaxCountInDeck)
        {
            yield break;
        }

        Draggable_Card draggable_Card = CreateDraggable_Card();

        SetStartDrag(_cardPrefab_CreateDeck,draggable_Card);

        draggable_Card.IsDragging = false;

        OffDetailCard();

        yield return StartCoroutine(AddDeckCardCoroutine(_cardPrefab_CreateDeck,draggable_Card));

        Reset_EndDrag(draggable_Card);
    }
    #endregion


    #region 右クリック時にデッキからカードを抜くアニメーション
    public IEnumerator RemoveDeckCardCoroutine_OnClick(CardPrefab_CreateDeck _cardPrefab_CreateDeck)
    {
        Draggable_Card draggable_Card = CreateDraggable_Card();

        _cardPrefab_CreateDeck.HideDeckCardTab();

        SetStartDrag(_cardPrefab_CreateDeck,draggable_Card);

        draggable_Card.IsDragging = false;

        OffDetailCard();

        yield return StartCoroutine(RemoveDeckCardCoroutine(_cardPrefab_CreateDeck,draggable_Card));

        Reset_EndDrag(draggable_Card);
    }
    #endregion

    #region デッキにカードを追加
    public IEnumerator AddDeckCards(CEntity_Base cEntity_Base)
    {
        if (EdittingDeckData.DeckCards().Count((cardData) => cardData.CardName == cEntity_Base.CardName) >= cEntity_Base.MaxCountInDeck)
        {
            yield break;
        }

        EdittingDeckData.AddDeckCard(cEntity_Base);

        EdittingDeckData.DeckCardIDs = DeckData.GetDeckCardCodes(DeckData.SortedList(EdittingDeckData.DeckCards()));

        yield return StartCoroutine(ReflectDeckData());
    }
    #endregion

    #region デッキからカードを抜く
    public IEnumerator RemoveDeckCards(CEntity_Base cEntity_Base)
    {
        if (EdittingDeckData.DeckCards().Count((cardData) => cardData.CardName == cEntity_Base.CardName) <= 0)
        {
            yield break;
        }

        EdittingDeckData.RemoveDeckCard(cEntity_Base);

        EdittingDeckData.DeckCardIDs = DeckData.GetDeckCardCodes(DeckData.SortedList(EdittingDeckData.DeckCards()));

        yield return StartCoroutine(ReflectDeckData());
    }
    #endregion

    #region 主人公を設定
    public IEnumerator SetMainCharacter(CEntity_Base cEntity_Base)
    {
        if(!EdittingDeckData.DeckCards().Contains(cEntity_Base))
        {
            yield break;
        }

        EdittingDeckData.MainCharacterID = cEntity_Base.CardID;

        yield return StartCoroutine(ReflectDeckData());
    }
    #endregion

    #endregion

    #endregion

}

#region 自動スクロール
[System.Serializable]
public class AutoScrollclass
{
    public ScrollRect _scrollRect;

    /// <summary>
    /// スクロールエリアのRectTransform
    /// </summary>
    [SerializeField]
    private RectTransform _viewportRectransform;

    /// <summary>
    /// Nodeを格納するTransform
    /// </summary>
    [SerializeField]
    private Transform _contentTransform;

    /// <summary>
    /// NodeのRectTransform
    /// </summary>
    [SerializeField]
    private RectTransform _nodePrefab;

    /// <summary>
    /// VerticalLayoutGroup(Spacing取得用)
    /// </summary>
    [SerializeField]
    private VerticalLayoutGroup _verticalLayoutGroup;

    public float top;//-78
    public float bottom;//-717

    public float ScrollTime;

    #region 自動スクロール
    public void AutoScroll(int nodeIndex) 
    { 
        //要素間の間隔
        var spacing = _verticalLayoutGroup.spacing;
        //現在のスクロール範囲の数値を計算しやすい様に上下反転
        var p = 1.0f - _scrollRect.verticalNormalizedPosition;
        //現在の要素数
        var nodeCount = _contentTransform.childCount;
        //描画範囲のサイズ
        var viewportSize = _viewportRectransform.sizeDelta.y;
        //描画範囲のサイズの半分
        var harlViewport = viewportSize * 0.5f;

        //１要素のサイズ
        var nodeSize = _nodePrefab.sizeDelta.y * _nodePrefab.localScale.y+ spacing;

        //現在の描画範囲の中心座標
        var centerPosition = (nodeSize * nodeCount - viewportSize) * p + harlViewport;
        //現在の描画範囲の上端座標
        var topPosition = centerPosition - harlViewport;
        //現在の現在描画の下端座標
        var bottomPosition = centerPosition + harlViewport;

        // 現在選択中の要素の中心座標
        var nodeCenterPosition = nodeSize * nodeIndex + nodeIndex / 2.0f + _verticalLayoutGroup.padding.top;

        RectTransform targetRect = _contentTransform.GetChild(nodeIndex).GetComponent<RectTransform>();

        float pos = targetRect.localPosition.y + _contentTransform.localPosition.y;

        if(bottom <= pos && pos <= top)
        {
            return;
        }
        

        //選択した要素が上側にはみ出ている
        if (topPosition > nodeCenterPosition)
        {
            //選択要素が描画範囲に収まるようにスクロール
            var newP = (nodeSize * nodeIndex) / (nodeSize * nodeCount - viewportSize);

            //ContinuousController.instance.StartCoroutine(ScrollCoroutine(1.0f - newP));
            _scrollRect.verticalNormalizedPosition = 1.0f - newP; //反転していたので戻す
            return;
        }

        //選択した要素が下側にはみ出ている
        if (nodeCenterPosition > bottomPosition)
        {
            //選択要素が描画範囲に収まるようにスクロール
            var newP = (nodeSize * (nodeIndex + 1) + spacing - viewportSize) / (nodeSize * nodeCount - viewportSize);

            //ContinuousController.instance.StartCoroutine(ScrollCoroutine(1.0f - newP));
            _scrollRect.verticalNormalizedPosition = 1.0f - newP; //反転していたので戻す
            return;
        }
    }
    #endregion
    #endregion

}
