using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class VaseManager : MonoBehaviour
{
    private enum VaseState
    {
        NotPranked,
        Pranked,
        Boo,
        Restored,
    }
    private VaseState state;

    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private GameObject completeVase;
    [SerializeField] private GameObject brokenVase;
    [SerializeField] private GameObject debris;
    [SerializeField] private GameObject[] piece;
    [SerializeField] private GameObject[] ghostPiece;
    [SerializeField] private GameObject butlerBoo;
    [SerializeField] private GameObject instanceBooPos;
    [SerializeField] private GameObject vaseEffect;
    [Header("SE")]
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip setSound;

    private bool[] isCorrect;
    private Rigidbody[] rigidbodies;
    private GameObject[] handGrabs;
    private Transform[] correct;

    PlaySound playsound;
    Reset[] reset;
    // Start is called before the first frame update
    void Start()
    {
        ChangeMyState(VaseState.NotPranked);

        playsound = brokenVase.GetComponent<PlaySound>();
        //初期化
        handGrabs = new GameObject[piece.Length];
        reset = new Reset[piece.Length];
        correct = new Transform[piece.Length];
        isCorrect = new bool[piece.Length];
        rigidbodies = new Rigidbody[piece.Length];

        for (int i = 0; i < piece.Length; i++)
        {
            handGrabs[i] = piece[i].transform.GetChild(0).gameObject;
            reset[i] = piece[i].GetComponent<Reset>();
            correct[i] = ghostPiece[i].transform;
            rigidbodies[i] = piece[i].GetComponent<Rigidbody>();
            isCorrect[i] = false;
            handGrabs[i].SetActive(true);
            //Debug.Log("mawatta");
        }

        DifferenceManager.Instance.OnDiffrenceStateChange += CheckChosen;
    }

    //
    //
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.Game)
        {
            if (state == VaseState.Pranked)
            {
                if (DifferenceManager.Instance.state == DifferenceState.Serch)
                {
                    CheckDistance();
                }
            }
            else if(state == VaseState.Boo)
            {
                if (DifferenceManager.Instance.FixedPlank[Boos.Butler])
                {
                    ChangeMyState(VaseState.Restored);
                }
            }
        }

        if (DifferenceManager.Instance.state == DifferenceState.Finish)
        {
            ChangeMyState(VaseState.Restored);
        }
    }
    //
    //
    private void ChangeMyState(VaseState newState)
    {
        if (state == newState) return;
        switch (newState)
        {
            case VaseState.NotPranked:
                SetVaseActive(true);
                break;
            case VaseState.Pranked:
                SetVaseActive(false);
                break;
            case VaseState.Boo:
                SetVaseActive(true);
                DifferenceManager.Instance.ChangeState(DifferenceState.Catch);
                Instantiate(butlerBoo, instanceBooPos.transform.position, instanceBooPos.transform.rotation);
                break;
            case VaseState.Restored:
                SetVaseActive(true);
                break;
            default:
                break;
        }
        state = newState;
    }
    private void SetVaseActive(bool compleat)
    {
        completeVase.SetActive(compleat);

        if(compleat)
        {
            brokenVase.SetActive(false);
            debris.SetActive(false);
        }
        else
        {
            brokenVase.SetActive(true);
            debris.SetActive(true);
        }
       
    }
    private void CheckChosen(DifferenceState newState)
    {
        if (newState == DifferenceState.Butler)
        {
            //ツボは絶対選ばれる
            ChangeMyState(VaseState.Pranked);
            playsound.PlayOneShot(breakSound);
        }
    }
    private void CheckDistance()
    {
        int Count = 0;
        //透明のやつと破片が距離測る
        for (int i = 0; i < piece.Length; i++)
        {
            //終わったやつがあったら次へ飛ばす
            if (ghostPiece[i] == null)
            {
                Count++;
                continue;
            }

            float distance = Vector3.Distance(piece[i].transform.position, ghostPiece[i].transform.position);
            //debugLog.text = "subtitleDistance" + subtitleDistance;

            //正解（かけら）
            if (distance < 0.1f)
            {
                float anglediff = Vector3.Angle(piece[i].transform.forward, ghostPiece[i].transform.forward);
                //debugLog.text = "angle" + anglediff;
                //正解（傾きも）
                if (anglediff < 25f)
                {
                    StartCoroutine(FixPiece(i));
                }
            }
            //不正解
            else
            {
                //debugLog.text = ("none");
                continue;
            }
        }

        if (Count >= piece.Length)
        {
            ChangeMyState(VaseState.Boo);
        }
    }
    private IEnumerator FixPiece(int i)
    {
        isCorrect[i] = true;

        // 先に掴みを完全停止
        handGrabs[i].SetActive(false);
        rigidbodies[i].isKinematic = true;
        rigidbodies[i].useGravity = false;
        reset[i].enabled = false;

        yield return null;

        piece[i].transform.SetPositionAndRotation(
            ghostPiece[i].transform.position,
            ghostPiece[i].transform.rotation
        );

        playsound.PlayOneShot(setSound);
        Instantiate(vaseEffect, piece[i].transform.position, Quaternion.identity);

        Destroy(ghostPiece[i]);
        ghostPiece[i] = null;
    }
}
