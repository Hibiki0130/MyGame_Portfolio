//using Meta.XR.Editor.Tags;
using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BookController : MonoBehaviour
{
    [SerializeField] private DialogueData standbyData;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bookBag;
    [SerializeField] private GameObject firstInstancePos;
    [SerializeField] private GameObject standbyPos;
    [SerializeField] private GameObject lastInstancePos;
    [SerializeField] private GameObject[] targets;
    [Header("Effect")]
    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private GameObject dustEffect;
    [Header("SE")]
    [SerializeField] private AudioClip rub;
    [SerializeField] private AudioClip appearanceSound;
    public bool ableTalk { get; private set; }

    private int rubedBook;
    private int rubedCount;
    private bool ableInstanceDust;
    private bool setPlayerGaze;
    private bool approachText;
    private bool notapproachText;
    private GameObject nowTarget;
    private GameObject gaze;
    private Animator bookAnimator;

    BookGesture bookgesture;
    PlaySound playsound;
    // Start is called before the first frame update
    private void Awake()
    {
       
    }
    void Start()
    {
        nowTarget = targets[0];
        rubedBook = 0;
        rubedCount = 2;

        ableInstanceDust = true;
        setPlayerGaze = false;
        approachText = true;
        notapproachText = true;
        ableTalk = true;

        gaze = player;

        TutorialManager.Instance.OnTutorialStateChange += ChangeTutorialState;
        GameManager.Instance.OnStateChange += ChangeBook;
        DifferenceManager.Instance.OnDiffrenceStateChange += ChangeDifferenceState;

        bookgesture = GetComponent<BookGesture>();
        playsound = GetComponentInChildren<PlaySound>();
        bookAnimator = GetComponentInChildren<Animator>();
    }
    //
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.state == GameState.Standby)
        {
            StandbyAnimation();
        }
        if(GameManager.Instance.state==GameState.Tutorial)
        {
            if (TutorialManager.Instance.state == TutorialState.Boo)
            {
                transform.position = bookBag.transform.position;
            }
        }
        if (GameManager.Instance.state == GameState.Game)
        {
            if (DifferenceManager.Instance.state == DifferenceState.Catch)
            {
                transform.position = bookBag.transform.position;
            }
        }
        else
        {
            LookTarget(player);
            //↓だとうまくいかないよ
            //transform.LookAt(playerPos.transform);
            //MoveToCenter();
        }
    }

    //
    //
    //プレイヤーの目線に合わせる
    private void ChangeBook(GameState newState)
    {
        switch (newState)
        {
            case GameState.Attention:
                MoveToBag();
                break;
            case GameState.Standby:
                StartCoroutine(BookAppearanceRoutine(firstInstancePos, false));
                bookAnimator.SetTrigger("Standby");
                break;
            case GameState.OP:
                Subtitle.Instance.StopSubTitle();
                ableTalk = false;
                bookAnimator.SetTrigger("Exit");
                break;
            case GameState.LookAround:
                break;
            case GameState.ED:
                MoveToBag();
                break;
            default:
                break;
        }
    }
    private void ChangeTutorialState(TutorialState newstate)
    {
        switch(newstate)
        {
            case TutorialState.Walk:
                nowTarget = targets[0];
                StartCoroutine(BookAppearanceRoutine(nowTarget, true));
                break;
            case TutorialState.TelePhone:
                nowTarget = targets[1];
                StartCoroutine(BookAppearanceRoutine(nowTarget, true));
                break;
            case TutorialState.Boo:
                MoveToBag();
                break;
            case TutorialState.finish:
                nowTarget = targets[2];
                StartCoroutine(BookAppearanceRoutine(nowTarget, false));
                break;
            default:
                break;
        }
    }
    private void ChangeDifferenceState(DifferenceState newState)
    {
        
        if (newState == DifferenceState.Serch)
        {
            StartCoroutine(BookAppearanceRoutine(standbyPos, false));
        }
        else if (newState == DifferenceState.Catch)
        {
            MoveToBag();
        }
        if (newState == DifferenceState.Finish)
        {
            StartCoroutine(BookAppearanceRoutine(standbyPos, false));
        }
    }
    private void LookTarget(GameObject target)
    {
        //Quaternion targetRot = Quaternion.LookRotation(target.transform.position - transform.position);
        Quaternion targetRot = Quaternion.LookRotation(transform.position - target.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3.0f);
    }
    float time = 0f;
    private void StandbyAnimation()
    {
        time += Time.deltaTime;
        if (time >= 2f)
        {
            if (rubedBook > rubedCount)
            {
                GameManager.Instance.ChangeState(GameState.OP);
            }

            //本に近づいていないとき
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) >= 0.8f)
            {
                if (!approachText)
                {
                    Subtitle.Instance.StopSubTitle();
                    approachText = true;
                }
                if (notapproachText)
                {
                    Subtitle.Instance.InstanceLoopSubTitle(standbyData, 0, 1);
                    notapproachText = false;
                }
            }
            //本に近づいたとき
            else
            {
                if (!notapproachText)
                {
                    Subtitle.Instance.StopSubTitle();
                    notapproachText = true;
                }
                if (approachText)
                {
                    Subtitle.Instance.InstanceLoopSubTitle(standbyData, 2, 2);
                    approachText = false;
                }
            }
        }
    }
    private void MoveToBag()
    {
        //バックにしまわれるときは会話はするのでDissaperroutineは使えない
        Instantiate(smokeEffect, transform.position, Quaternion.identity);
        transform.localScale = Vector3.zero;
        transform.position = bookBag.transform.position;
    }
    private IEnumerator DustEffect()
    {
        ableInstanceDust = false;

        playsound.PlayOneShot(rub);
        Instantiate(dustEffect, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        rubedBook++;
        ableInstanceDust = true;
    }
    public IEnumerator BookAppearanceRoutine(GameObject target, bool disappear)
    {
        ableTalk = false;

        if (disappear)
        {
            yield return StartCoroutine(BookDisappearRoutine());
        }

        transform.position = target.transform.position;

        yield return null;

        playsound.PlayOneShot(appearanceSound);
        Instantiate(smokeEffect, transform.position, Quaternion.identity);
        transform.localScale = Vector3.one;
        ableTalk = true;
    }

    public IEnumerator BookDisappearRoutine()
    {
        ableTalk = false;
        transform.localScale = Vector3.zero;
        Instantiate(smokeEffect, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);
    }
    private void OnTriggerStay(Collider other)
    {
        //埃払うとき
        if (GameManager.Instance.state == GameState.Standby)
        {
            if (other.CompareTag("LeftHand"))
            {
                if (HandTraking.Instance.GetLeftRub())
                {
                    if(ableInstanceDust)
                    {
                        StartCoroutine(DustEffect());
                    }
                }
            }
            if (other.CompareTag("RightHand"))
            {
                if (HandTraking.Instance.GetRightRub())
                {
                    if (ableInstanceDust)
                    {
                        StartCoroutine(DustEffect());
                    }
                }
            }
        }
    }
}
//dontuse
/*public void BookMove(GameObject target)
{
    if (Goal)
    {
        gaze = playerPos;
    }
    else
    {


        //Debug.Log(Vector3.Distance(transform.position, target.transform.position));
        if (Vector3.Distance(transform.position, target.transform.position) < 0.01f)
        {
            Goal = true;
        }
    }

    LookTarget(gaze);
}
    private void MoveToCenter()
    {
        CameraManager.Instance.UIControl(transform, 0.8f, 0, -0.2f);
    }
    private void CrazySetTarget()
    {
        Vector3 targetPos = nowTarget.transform.position;
        if (setPlayerGaze)
        {
            targetPos.y = playerPos.transform.position.y;
        }

        nowTarget.transform.position = targetPos;
    }
*/