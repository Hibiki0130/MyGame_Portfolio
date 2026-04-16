using System;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using OVR;
using Meta.WitAi;

public enum GameState
{
    Setting,
    Attention,
    Standby,
    OP,
    Tutorial,
    //Move,
    LookAround,
    Game,
    ED,
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private GameObject passthrough;
    [SerializeField] private Light dreamLight;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject attentionSpace;
    [SerializeField] private GameObject HallWay;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject doorSide;
    [SerializeField] private GameObject room;
    [SerializeField] private GameObject animationBook;
    [SerializeField] private PlayableDirector opDirector;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject eyeMaskBoo;
    [SerializeField] private GameObject subTitle;
    [SerializeField] private GameObject finishRoom;
    public static GameManager Instance { get; private set; }
    public GameState state { get; private set; }

    private float range;
    //stateが変わったときに呼ばれる
    public event Action<GameState> OnStateChange;

    private void Awake()
    {
        opDirector.Stop();

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
        state = GameState.Setting;

        OnStateChange += StartOPMovie;
        OnStateChange += DestroyTitle;
        OnStateChange += StageSetActive;
        //OnStateChange += InstanceEyeMaskBoo;
        OnStateChange += PassthroughOnOff;

        ChangeState(GameState.Attention);
        //ChangeState(GameState.Standby);
        //ChangeState(GameState.Tutorial);
        //ChangeState(GameState.LookAround);
        //ChangeState(GameState.Game);
        //ChangeState(GameState.ED);
        //原点リセット（絶対消すな）
        //OVRManager.display.RecenterPose();
        //OVRManager.instance.usePositionTracking = false;

        range = 10f;
        //int count = GameObject.FindGameObjectsWithTag("MainCamera").Length;
        //Debug.Log(count);
    }

    // Update is called once per frame
    void Update()
    {
        //debugLog.text = "" + state;

        if (Input.GetKey(KeyCode.O))
        {
            if (state == GameState.Standby)
            {
                ChangeState(GameState.OP);
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            if (state == GameState.OP)
            {
                ChangeState(GameState.Tutorial);
            }
        }

        if (Input.GetKey(KeyCode.M))
        {
            if (state == GameState.Tutorial)
            {
                ChangeState(GameState.LookAround);
            }
        }
    }
    private IEnumerator StandbyRoutine()
    {
        yield return null;

        while (range > 0f)
        {
            range -= 8f * Time.deltaTime;
            dreamLight.range = range;
            //Debug.Log("mawaru");
            yield return null;
        }
    }
    public void ChangeState(GameState newState)
    {
        if (state == newState) return;

        state = newState;
        Debug.Log(state);
        OnStateChange?.Invoke(state);
    }
    private void StageSetActive(GameState newState)
    {
        if (newState == GameState.Attention)
        {
            attentionSpace.SetActive(true);
        }
        if (newState == GameState.Standby)
        {
            door.SetActive(true);
            doorSide.SetActive(true);
            //HallWay.SetActive(true);
            //StartCoroutine(StandbyRoutine());
            //Debug.Log("mawaru");
        }
        if (newState == GameState.LookAround)
        {
            room.SetActive(true);
        }
        if (newState == GameState.ED)
        {
            room.SetActive(false);
            door.SetActive(false);
            doorSide.SetActive(false);
        }
    }
    private void DestroyTitle(GameState newstate)
    {
        if (newstate == GameState.OP)
        {
            Destroy(title);
        }
    }

    private void StartOPMovie(GameState newState)
    {
        if (newState == GameState.OP)
        {
            opDirector.Play();
        }
    }

    /*private void InstanceEyeMaskBoo(GameState newState)
    {
        if (newState == GameState.Game)
        {
            Instantiate(eyeMaskBoo, playerPos.transform.position, Quaternion.identity);
        }
    }*/
    private void PassthroughOnOff(GameState newState)
    {
        if (newState == GameState.ED)
        {
            //room.SetActive(false);
            finishRoom.SetActive(true);
            passthrough.SetActive(true);
        }
    }
    //OPが終わったらstate遷移（TimeLineで参照）
    public void FinishOPMovie()
    {
        ChangeState(GameState.Tutorial);
    }
}
