using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DifferenceState
{
    None,
    Butler,
    Twins,
    Artist,
    //Cat,

    Serch,
    Catch,
    Finish,
}
public enum Planks
{
    //bulter
    Vase = 0,
    //Artist
    Picture = 0,
    Wall = 1,
    //Twins
    Follow = 0,
    Move = 1,
    //Bonus
    TV = 0,
}
public enum Boos
{
    Butler,
    Twins,
    Artist,
    //Bonus,
}
public class DifferenceManager : MonoBehaviour
{
    public static DifferenceManager Instance { get; private set; }
    public DifferenceState state {  get; private set; }

    private Dictionary<Boos, int> chosenPlank = new Dictionary<Boos, int>();
    public Dictionary<Boos, bool> FixedPlank { get; private set; } = new Dictionary<Boos, bool>();

    public bool Light {  get; set; }
    public bool SetDiffrence { get; set; }
    public string ClearTime { get; private set; }
    public int ClearBoo {  get; private set; }

    public event Action<DifferenceState> OnDiffrenceStateChange;

    [Header("SubTitle")]
    [SerializeField] private DialogueData subtitleData;
    [SerializeField] private DialogueData attentionData;
    [SerializeField] private DialogueData FinishData;
    [Header("GetPos")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject confettiInstancePos;
    [Header("GetModel")]
    [SerializeField] private GameObject eyeMaskBoo;
    [SerializeField] private GameObject timerModel;
    [SerializeField] private GameObject confettiPrefub;
    [SerializeField] private GameObject room;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Light roomLight;
    [SerializeField] private Light dreamLight;
    [SerializeField] private GameObject FadeObj;
    [Header("Effect")]
    [SerializeField] private GameObject smokeEffect;

    private GameObject confetti;
    private Animator bookAnimator;
    private TextMeshPro timer;
    private bool notClear;
    private bool mainGameStart;
    private int limitMinutes;
    private int clearPrank;
    private float limitSeconds;
    private float lightTime;
    private float intensity;
    private float speed;

    Fade fadeScript;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //ChangeState(DifferenceState.Serch);

        GameManager.Instance.OnStateChange += WhenChangedGameManagerState;
        OnDiffrenceStateChange += StartFinishRoutine;
        OnDiffrenceStateChange += Score;
        ChangeState(DifferenceState.None);
        StartCoroutine(DesidePlanks());

        timer = timerModel.GetComponent<TextMeshPro>();
        ClearTime = timer.text;
        bookAnimator = book.GetComponentInChildren<Animator>();
        fadeScript = FadeObj.GetComponent<Fade>();
        FixedPlank[Boos.Butler] = false;
        FixedPlank[Boos.Artist] = false;
        FixedPlank[Boos.Twins] = false;

        confetti = null;
        notClear = false;
        Light = true;
        mainGameStart = false;
        clearPrank = 0;
        limitMinutes = 3;
        limitSeconds = limitMinutes * 60;
        //limitSeconds = 10;
        lightTime = 0f;
        intensity = 3f;
        speed = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.LookAround)
        {
            if (SetDiffrence)
            {
                StartCoroutine(SetDifferenceRoutine());
                SetDiffrence = false;
            }

            RoomLightControl();
        }
        else if (GameManager.Instance.state == GameState.Game)
        {
            if (state != DifferenceState.Finish)
            {
                TimerControl();
            }
            else
            {
                if (fadeScript.State == FadeState.Dark)
                {
                    GameManager.Instance.ChangeState(GameState.ED);
                }
            }
        }
    }
    private void WhenChangedGameManagerState(GameState newState)
    {
        //エンディング
        if (newState == GameState.ED)
        {
            Destroy(confetti);
        }
    }
    private IEnumerator DesidePlanks()
    {
        chosenPlank[Boos.Butler] =  UnityEngine.Random.Range((int)Planks.Vase, (int)Planks.Vase);
        chosenPlank[Boos.Artist] = UnityEngine.Random.Range((int)Planks.Picture, (int)Planks.Wall+ 1);
        chosenPlank[Boos.Twins] = UnityEngine.Random.Range((int)Planks.Follow, (int)Planks.Move + 1);
        //Debug.Log(chosenPlank[Boos.Artist]);

        yield return null; 
    }
    private IEnumerator FixedPlanks()
    {
        clearPrank = 0;

        if (FixedPlank[Boos.Butler])
        {
            clearPrank++;
        }
        yield return null;
        if(FixedPlank[Boos.Artist])
        {
            clearPrank++;
        }
        yield return null;
        if(FixedPlank[Boos.Twins])
        {
            clearPrank++;
        }
        yield return null;

        if (clearPrank == 3)
        {
            ChangeState(DifferenceState.Finish);
        }
        else
        {
            switch (clearPrank)
            {
                case 1:
                    Subtitle.Instance.InstanceSubTitle(subtitleData, 10, 10);
                    break;
                case 2:
                    Subtitle.Instance.InstanceSubTitle(subtitleData, 11, 11);
                    break;
                default:
                    break;
            }
        }
    }
    private void StartFinishRoutine(DifferenceState newState)
    {
        if (newState == DifferenceState.Finish)
        {
            StartCoroutine(FinishRoutine());
        }
    }
    private IEnumerator FinishRoutine()
    {
        if (clearPrank == 3)
        {
            confetti = Instantiate(confettiPrefub, confettiInstancePos.transform.position, confettiInstancePos.transform.rotation);
            Subtitle.Instance.InstanceSubTitle(FinishData, 2, 3);
        }
        else
        {
            Subtitle.Instance.InstanceSubTitle(FinishData, 0, 0);
        }
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        timer.text = "";
        Instantiate(smokeEffect, timer.transform.position, Quaternion.identity);
        if (confetti == null)
        {
            confetti = Instantiate(confettiPrefub, confettiInstancePos.transform.position, confettiInstancePos.transform.rotation);
        }
        Subtitle.Instance.InstanceSubTitle(FinishData, 4, 4);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        Subtitle.Instance.InstanceSubTitle(FinishData, 5, 5);
        yield return null;

        //DreamLightControl();
        fadeScript.StartFade(1f);
    }
    private void RoomLightControl()
    {
        //Debug.Log(Light);
        if (!Light)
        {
            lightTime += Time.deltaTime;
            roomLight.intensity = intensity * curve.Evaluate(lightTime * speed);
        }
        else
        {
            roomLight.intensity = intensity;
        }
    }
    private float lightRange = 0f;
    private float maxRange = 10f;
    private bool fadeout = false;
    /*private void DreamLightControl()
    {
        dreamLight.transform.position = player.transform.position;
        if (fadeout)
        {
            room.SetActive(false);
            if (confetti != null)
            {
                Destroy(confetti);
            }
            dreamLight.range = 0f;
            GameManager.Instance.ChangeState(GameState.ED);
        }
        else
        {
            if (lightRange <= maxRange)
            {
                lightRange += 5f * Time.deltaTime;
                dreamLight.range = lightRange;
            }
            else
            {
                fadeout = true;
            }
        }
    }*/
    private void TimerControl()
    {
        Quaternion targetRot = Quaternion.LookRotation(timerModel.transform.position - player.transform.position);
        timerModel.transform.rotation = Quaternion.Slerp(timerModel.transform.rotation, targetRot, Time.deltaTime * 3.0f);

        if (limitSeconds <= 0)
        {
            notClear = true;
            ChangeState(DifferenceState.Finish);
        }
        else
        {
            limitSeconds -= Time.deltaTime;
            var span = new TimeSpan(0, 0, (int)limitSeconds);
            timer.text = span.ToString(@"mm\:ss");
        }
    }
    private IEnumerator SetDifferenceRoutine()
    {
        fadeScript.StartFade(3f);
        ChangeState(DifferenceState.Butler);
        yield return new WaitForSeconds(2f);
        ChangeState(DifferenceState.Twins);
        yield return new WaitForSeconds(2f);
        ChangeState(DifferenceState.Artist);

        ChangeState(DifferenceState.Serch);
    }
    public int GetChosenPlanked(Boos boo)
    {
        return chosenPlank[boo];
    }
    public void SetFixedPlank(Boos boo)
    {
        FixedPlank[boo] = true;
        StartCoroutine(FixedPlanks());
        ChangeState(DifferenceState.Serch);
    }
    public void ChangeState(DifferenceState newState)
    {
        if (state == newState) return;

        state = newState;
        Debug.Log(state);

        OnDiffrenceStateChange?.Invoke(state);
    }
    private void Score(DifferenceState newState)
    {
        if (newState == DifferenceState.Finish)
        {
            if (notClear)
            {
                ClearTime = "おしかった...";
            }
            else
            {
                ClearTime = timer.text;
            }
            ClearBoo = clearPrank;
        }
    }
}
