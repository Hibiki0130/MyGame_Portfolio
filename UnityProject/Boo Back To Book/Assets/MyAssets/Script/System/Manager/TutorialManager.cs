using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TutorialState
{
    //始まってない時
    OP,
    //始まった時
    Walk,
    TelePhone,
    Door,
    Boo,
    finish,
}
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public TutorialState state { get; private set; }

    //public static TutorialManager instance { get; private set; } 

    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private GameObject book;
    [Header("walk")]
    [SerializeField] private GameObject induction;
    [SerializeField] private GameObject inductionSpawnPoint;
    [Header("Boo")]
    [SerializeField] private GameObject instanceBooPoint;
    [SerializeField] private GameObject butler;
    [Header("Lights")]
    [SerializeField] private GameObject[] lights;

    private GameObject boo;
    private Material titleMate;
    private Animator bookAnimator;
    private bool animationTrigger;
    private bool startWalkEx;
    private bool isApproach;
    public bool isSolved { get; private set; }
    public bool clearWalk { get; private set; }

    public event Action<TutorialState> OnTutorialStateChange;

    private float time;

    Induction inductionScript;
    Boo booscript;
    private void Awake()
    {
        //シングルトン初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        isSolved = false;
        clearWalk = false;
        animationTrigger = false;
        startWalkEx = true;
        isApproach = false;

        OnTutorialStateChange += ChangeLight;
        OnTutorialStateChange += WhenChangeMyState;

        ChangeState(TutorialState.OP);
        //ChangeState(TutorialState.Door);

        time = 10f;

        bookAnimator = book.GetComponent<Animator>();
        booscript = null;
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.Tutorial)
        {
            if (state == TutorialState.OP)
            {
                ChangeState(TutorialState.Walk);
            }

            if (state == TutorialState.Walk)
            {
               // bookController.BookMove(inductionSpawnPoint, 1.0f);
                CheckWalk();
            }

            if (state == TutorialState.Boo)
            {
                GhostController();
            }

            if (state == TutorialState.finish)
            {
                //GameManager.Instance.ChangeState(GameState.LookAround);
                //ED.SetActive(true);
            }
        }

        //DEBUG//
        //debugLog.text = "" + state;
        if (Input.GetKey(KeyCode.F))
        {
            ChangeState(TutorialState.finish);
        }
        //DEBUG//
    }
    //廊下の光制御
    private void ChangeLight(TutorialState newstate)
    {
        switch(newstate)
        {
            case TutorialState.OP:
                lights[0].SetActive(true);
                break;
            case TutorialState.Walk:
                lights[0].SetActive(false);
                lights[4].SetActive(true);
                break;
            case TutorialState.TelePhone:
                lights[0].SetActive(false);
                lights[1].SetActive(true);
                break;
            case TutorialState.Door:
                lights[1].SetActive(false);
                lights[2].SetActive(true);
                break;
            case TutorialState.Boo:
                lights[2].SetActive(false);
                lights[3].SetActive(true);
                break;
        }
    }
    private void WhenChangeMyState(TutorialState newState)
    {
        switch(newState)
        {
            case TutorialState.Walk:
                Instantiate(induction, inductionSpawnPoint.transform.position, Quaternion.identity);
                Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 3, 3);
                break;
            case TutorialState.Boo:
                boo = Instantiate(butler, instanceBooPoint.transform.position, Quaternion.identity);
                break;
                case TutorialState.finish:
                StartCoroutine(FinishSubTitle());
                break;
            default:
                break;
        }
    }
    private void CheckWalk()
    {
        GameObject obj = GameObject.FindWithTag("Induction");

        if (obj == null)
        {
            //Debug.Log("空");
            return;
        }

        inductionScript = obj.GetComponent<Induction>();
        if (inductionScript == null)
        {
            Debug.Log("空");
            return;
        }

        if (inductionScript.approached)
        {
            isApproach = true;

            if (startWalkEx)
            {
                StartCoroutine(WalkSubTitle());
                startWalkEx = false;
            }
        }
    }
    private void GhostController()
    {
        if (booscript == null)
        {
            booscript = boo.GetComponent<Boo>();
        }
    }
    public void Moveboo()
    {
        if (booscript == null) return;

        booscript.state = BooState.Moved;
    }
    private IEnumerator WalkSubTitle()
    {
        Subtitle.Instance.StopSubTitle();

        yield return null;

        bookAnimator.SetTrigger("Talk");
        Subtitle.Instance.InstanceSubTitle(dialogueData, 4, 5);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        ChangeState(TutorialState.TelePhone);
        inductionScript.SetSentBoolTrue();
    }
    private IEnumerator FinishSubTitle()
    {
        SoundManager.Instance.BGMStop();
        Subtitle.Instance.StopSubTitle();

        yield return null;

        Subtitle.Instance.InstanceSubTitle(dialogueData, 20, 22);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        GameManager.Instance.ChangeState(GameState.LookAround);
    }
    public void SetIsSolvedBool(bool a)
    {
        isSolved = a;
    }

    public void ChangeState(TutorialState newState)
    {
        if (state == newState) return;

        state = newState;
        OnTutorialStateChange?.Invoke(state);
        Debug.Log(newState);
    }

    //dontuse//
    /*2秒でフェードアウト(できん！！諦めーわら)
    float fadeDuration = 2.0f;
    float currentFadeTime = 0f;
    titleMate = title.GetComponent<MeshRenderer>().material;

        if (titleMate == null)
        {
            Debug.Log("ないよ");
        }
    private void FadeTitle()
    {
        if (currentFadeTime < fadeDuration)
        {
            // 現在のAlpha値を計算
            float alphaValue = 1 - (currentFadeTime / fadeDuration);
            // マテリアルの色を更新
            //titleMate.color = new Color(titleMate.color.r, titleMate.color.g, titleMate.color.b, alphaValue);
            Color currentColor = titleMate.GetColor("_Color");
            currentColor.a = alphaValue;
            titleMate.SetColor("_Color", currentColor);

            Color c = titleMate.color;
            c.a = alphaValue;
            titleMate.color = c;
            // 時間を更新
            currentFadeTime += Time.deltaTime;
        }
    }
     public void LoopSubTitle(int start, int end)
    {
        //字幕
        lightTime += Time.deltaTime;

        if (lightTime > 10f)
        {
            if (!Subtitle.Instance.GetStartRoutineBool())
            {
                bookAnimator.SetTrigger("Talk");
                Subtitle.Instance.InstanceSubTitle(subtitleData, start, end);
                animationTrigger = true;
                lightTime = 0f;
            }
        }
        else if (lightTime > 5f)
        {
            if (animationTrigger)
            {
                bookAnimator.SetTrigger("Idle");
                animationTrigger = false;
            }
        }
        //字幕
    }
     */
}
