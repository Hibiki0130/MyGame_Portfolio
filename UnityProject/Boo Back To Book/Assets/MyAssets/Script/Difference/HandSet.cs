using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
//using static TutorialManager;

public enum HandSetState
{
    None,
    Different,
    Done,
}
public class HandSet : MonoBehaviour
{
    public HandSetState state { get; private set; }

    //private static TutorialManager instance;

    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private GameObject telephone;
    [SerializeField] private GameObject handSetGhost;
    [SerializeField] private GameObject AnimationBook;

    [Header("SE")]
    [SerializeField] private AudioClip hangUp;
    [SerializeField] private AudioClip Hallo;

    private float subtitleTime;

    private bool isHandUp;
    private bool ableRing;
    private bool isTouched;

    private bool firstHallo;
    private bool firstEx;
    private bool coroutine;
    private bool loopEx;
    private bool booSurprise;

    private Rigidbody rB;
    private BoxCollider bC;
    private ConfigurableJoint joint;
    private MeshRenderer ghostMesh;
    private Coroutine nowCoroutine;

    Telephone telePhone;
    PlaySound playsound;
    BookController bookcontroller;
    // Start is called before the first frame update
    void Start()
    {
        state = HandSetState.None;

        subtitleTime = 0f;

        isHandUp = false;
        ableRing = false;
        isTouched = false;
        firstHallo = true;
        firstEx = true;
        loopEx = true;
        booSurprise = true;
        coroutine = true;

        rB = GetComponent<Rigidbody>();
        bC = GetComponent<BoxCollider>();
        joint = telephone.GetComponent<ConfigurableJoint>();
        ghostMesh = handSetGhost.GetComponent<MeshRenderer>();
        telePhone = telephone.GetComponent<Telephone>();
        playsound = GetComponent<PlaySound>();
        bookcontroller = AnimationBook.GetComponent<BookController>();

        TutorialManager.Instance.OnTutorialStateChange += ChangeState;

        rB.useGravity = false;
        rB.isKinematic = true;
        bC.enabled = false;
        ghostMesh.enabled = false;
        joint.connectedBody = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.H))
        {
            TutorialManager.Instance.SetIsSolvedBool(true);
            telePhone.StopRing();
            state = HandSetState.Done;
        }

        //受話器を置いた
        if (!isHandUp && !isTouched)
        {
            transform.position = handSetGhost.transform.position;
            transform.rotation = handSetGhost.transform.rotation;

            rB.useGravity = false;
            rB.isKinematic = true;

            ghostMesh.enabled = false;
            joint.connectedBody = null;

            //音
            if (ableRing)
            {
                telePhone.PlayDown();
                ableRing = false;
            }

            //ハローが流れたら
            if (!firstHallo)
            {
                if (state == HandSetState.Different)
                {
                    //halloを止める
                    TutorialManager.Instance.SetIsSolvedBool(true);
                    playsound.Stop();
                    state = HandSetState.Done;
                }
            }
        }

        //持ってる
        if (isHandUp && isTouched)
        {
            ghostMesh.enabled = true;
            joint.connectedBody = rB;

            if (state == HandSetState.Different)
            {
                if (firstHallo)
                {
                    telePhone.StopRing();
                    //取ったら今流れている字幕を止める
                    Subtitle.Instance.StopSubTitle();
                    playsound.PlayOneShot(Hallo);
                    firstHallo = false;
                }
            }
        }
        //受話器セットなし、持ってない
        else if (isHandUp && !isTouched)
        {
            rB.isKinematic = false;
            rB.useGravity = true;
        }

        //字幕
        if (TutorialManager.Instance.state == TutorialState.TelePhone)
        {
            HandSetSubtitle();
        }
    }

    private void ChangeState(TutorialState newState)
    {
        if (newState == TutorialState.TelePhone)
        {
            state = HandSetState.Different;
        }
    }

    private void HandSetSubtitle()
    {
        //戻した
        if (state == HandSetState.Done)
        {
            if (booSurprise)
            {
                Subtitle.Instance.StopSubTitle();
                StartCoroutine(bookcontroller.BookDisappearRoutine());
                booSurprise = false;
            }
        }
        //間違いが起こっているとき
        else if (state == HandSetState.Different)
        {
            //一回だけ説明
            if (firstEx)
            {
                if (coroutine)
                {
                    StartCoroutine(FirstEXRoutine());
                    coroutine = false;
                }
            }
            //最初の説明を聞いてから一度も受話器をとってない
            if (!firstEx && firstHallo)
            {
                //十秒経ったら
                if (loopEx)
                {
                    if (subtitleTime < 15f)
                    {
                        subtitleTime += Time.deltaTime;
                    }
                    else
                    {
                        //最初の説明が終わったら
                        if (Subtitle.Instance.nowSubtitleCoroutine == null)
                        {
                            Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 7, 7);
                            loopEx = false;
                        }
                    }
                }
                else
                {
                    subtitleTime = 0f;
                }
            }
            //受話器をとった
            if (!firstHallo)
            {
                if (subtitleTime < 20f)
                {
                    subtitleTime += Time.deltaTime;
                    loopEx = true;
                }
                //とってから戻さないまま20秒経ったら
                else
                {
                    if (loopEx)
                    {
                        Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 8, 8);
                        loopEx = false;
                    }
                }
            }
        }
    }
    private IEnumerator FirstEXRoutine()
    {
        Subtitle.Instance.InstanceSubTitle(dialogueData, 6, 6);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        Subtitle.Instance.InstanceSubTitle(dialogueData, 7, 7);
        bC.enabled = true;
        firstEx = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Telephone"))
        {
            //debugLog.text = "telephone";
            isHandUp = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))
        {
            //debugLog.text = "sawatta!";
            isTouched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Telephone"))
        {
            //debugLog.text = ("leave!");
            if (!isHandUp)
            {
                playsound.PlayOneShot(hangUp);
                ableRing = true;
                isHandUp = true;
            }
        }

        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))
        {
            //debugLog.text = "sawattenai!";
            isTouched = false;
        }
    }
}
