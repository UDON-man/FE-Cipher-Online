using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Photon;
using Photon.Pun;

public class GManager : MonoBehaviour
{
    [Header("あなた")]
    public Player You;

    [Header("相手")]
    public Player Opponent;

    [Header("カード情報プレハブ")]
    public CardSource CardPrefab;

    [Header("場のカードプレハブ")]
    public FieldUnitCard fieldUnitCardPrefab;

    [Header("手札のカードプレハブ")]
    public HandCard handCardPrefab;

    [Header("読み込み中オブジェクト")]
    public LoadingObject LoadingObject;

    [Header("コマンド選択パネル")]
    public SelectCommandPanel selectCommandPanel;

    [Header("もどるボタン")]
    public SelectCommand BackButton;

    [Header("リザルトテキスト")]
    public Text ResultText;

    [Header("タイトルに戻るボタン")]
    public GameObject ReturnToTitleButton;

    [Header("コマンドテキスト")]
    public CommandText commandText;

    [Header("ターンプレイヤー表示オブジェクト")]
    public ShowTurnPlayerObject showTurnPlayerObject;

    //[Header("ターゲット矢印")]
    //public TargetArrow targetArrow;

    [Header("処理領域のTransform")]
    public Transform ExecuteTransform;

    //[Header("オプションパネル")]
    //public OptionPanel optionPanel;

    [Header("キャンバス")]
    public Canvas canvas;

    [Header("カード詳細表示")]
    public CardDetail cardDetail;

    [Header("ユニット詳細表示")]
    public UnitDetail unitDetail;

    [Header("カード選択パネル")]
    public SelectCardPanel selectCardPanel;

    //[Header("ドラッグ表示")]
    //public GameObject ShowDrag;

    [Header("墓地確認")]
    public TrashCardPanel trashCardPanel;

    [Header("次のフェイズへボタン")]
    public NextPhaseButton nextPhaseButton;

    [Header("オプションパネル")]
    public OptionPanel optionPanel;

    [Header("カットイン")]
    public CutIn cutIn;

    [Header("ターゲット矢印プレハブ")]
    public TargetArrow targetArrowPrefab;

    [Header("ターゲット矢印親")]
    public Transform targetArrowParent;

    [Header("カメラ")]
    public Camera camara;

    public List<MultipleSkills> multipleSkills = new List<MultipleSkills>();

    public MultipleSkills availableMultipleSkills
    {
        get
        {
            foreach(MultipleSkills _multipleSkills in multipleSkills)
            {
                if(!_multipleSkills.isUsing)
                {
                    return _multipleSkills;
                }
            }

            return null;
        }
    }

    //オンライン待機用クラス
    public PhotonWaitController photonWaitController
    {
        get
        {
            return GetComponent<PhotonWaitController>();
        }
    }

    //ターン進行ステートマシン
    public TurnStateMachine turnStateMachine
    {
        get; set;
    }

    public static GManager instance = null;

    public bool IsAI { get; set; }

    private void Awake()
    {
        //return;

        instance = this;

#if UNITY_EDITOR
        if (!PhotonNetwork.IsConnected)
        {
            IsAI = true;
            
        }
#endif
        if(ContinuousController.instance != null)
        {
            if(ContinuousController.instance.isAI)
            {
                IsAI = true;
            }
        }

        turnStateMachine = this.gameObject.AddComponent<TurnStateMachine>();

        Init();

        StartCoroutine(turnStateMachine.Init());

        StartCoroutine(CheckDisconnect());

        Debug.Log("バトル初期化");
    }

    IEnumerator CheckDisconnect()
    {
        if (IsAI)
        {
            yield break;
        }

        yield return new WaitWhile(() => turnStateMachine == null);

        while (true)
        {
            if (!PhotonNetwork.IsConnected)
            {
                break;
            }

            else
            {
                if (!PhotonNetwork.InRoom)
                {
                    break;
                }

                else
                {
                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        if (PhotonNetwork.PlayerList.Length < 2)
                        {
                            break;
                        }
                    }
                }
            }

            yield return null;
        }

        if (!turnStateMachine.endGame)
        {
            turnStateMachine.EndGame(null);
        }
    }

    public void Init()
    {
        selectCommandPanel.Off();

        BackButton.CloseSelectCommandButton();

        ResultText.gameObject.SetActive(false);

        ReturnToTitleButton.SetActive(false);

        commandText.Init();

        showTurnPlayerObject.Init();

        //targetArrow.OffTargetArrow();

        OffTargetArrow();

        cardDetail.CloseCardDetail();

        unitDetail.CloseUnitDetail();

        selectCardPanel.CloseSelectCardPanel();

        //ShowDrag.SetActive(false);

        trashCardPanel.CloseSelectCardPanel();

        optionPanel.Init();

        cutIn.Off();
    }

    public void ReturnToTitle()
    {
        
        //SceneManager.LoadSceneAsync("Opening");

        ContinuousController.instance.EndBattle();
    }

    public void OnClickSurrenderButton()
    {
        if(turnStateMachine != null)
        {
            turnStateMachine.OnClickSurrenderButton();
        }
    }

    public TargetArrow CreateTargetArrow()
    {
        TargetArrow targetArrow = Instantiate(targetArrowPrefab, targetArrowParent);

        return targetArrow;
    }

    public Coroutine OnTargetArrow(Vector3 InitialPosition, Vector3 targetPosition,FieldUnitCard StartFieldUnitCard,FieldUnitCard EndFieldUnitCard)
    {
        TargetArrow targetArrow = CreateTargetArrow();

        Debug.Log("ターゲット矢印表示");
        return targetArrow.OnTargetArrow(InitialPosition, targetPosition,StartFieldUnitCard,EndFieldUnitCard);
    }

    public void OffTargetArrow()
    {
        if(targetArrowParent.childCount > 0)
        {
            if(targetArrowParent.GetChild(targetArrowParent.childCount - 1).GetComponent<TargetArrow>() != null)
            {
                targetArrowParent.GetChild(targetArrowParent.childCount - 1).GetComponent<TargetArrow>().Destroyed = true;
            }
            Destroy(targetArrowParent.GetChild(targetArrowParent.childCount - 1).gameObject);
        }
    }
}
