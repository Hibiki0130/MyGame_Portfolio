using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static Oculus.Interaction.Context;
using static OVRPlugin;
using static SoundManager;

public enum BookState
{
    DontAble,
    Bag,
    Have,
    Catched,
    Result,
}
public class BookGesture : MonoBehaviour
{
    private bool debug;
    private BookState state;
    private Animator bookAnim;

    [Header("デバッグに使ってるtext")]
    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private GameObject middleOfHands;
    [SerializeField] private GameObject bag;
    [SerializeField] private GameObject resultPos;
    [SerializeField] private TextMeshPro resultText;

    [Header("0..左手 1..右手")]
    //掌
    [SerializeField] private GameObject[] palms;
    //親指第一関節
    [SerializeField] private GameObject[] thumbs;
    //中指と人差し指の第2関節
    [SerializeField] private GameObject[] indexs;
    [SerializeField] private GameObject[] middles;
    //小指の付け根
    [SerializeField] private GameObject[] pinkys;

    [Header("使うSE")]
    [SerializeField] private AudioClip openABook;
    [SerializeField] private AudioClip closeABook;
    [SerializeField] private AudioClip catchAGhost;

    [SerializeField] private GameObject tutorialMane;

    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private GameObject catchEffect;

    private float bagDistance;
    private bool ableBag;
    private bool tutorialAbleCatch;
    private bool doClose;
    private bool result;
    private bool aimBoo;
    private bool aimMob;
    private bool isCatch;

    TutorialManager tutorialmanager;
    PlaySound playsound;
    void Start()
    {
        ChangeBookState(BookState.Bag);

        bookAnim = GetComponent<Animator>();
        tutorialmanager = tutorialMane.GetComponent<TutorialManager>();
        playsound = GetComponent<PlaySound>();

        GameManager.Instance.OnStateChange += WhenGameStateChange;
        TutorialManager.Instance.OnTutorialStateChange += WhenTutorialStateChange;
        DifferenceManager.Instance.OnDiffrenceStateChange += WhenDifferenceStateChange;

        bagDistance = 0.6f;
        tutorialAbleCatch = false;
        doClose = false;
        ableBag = false;
        aimBoo = false;
        aimMob = false;
        isCatch = false;
        resultText.text = "";

        //DEBUG//
        debug = false;
        //DEBUG//
    }

    //
    //
    // Update is called once per frame
    void Update()
    {
        CheckBookState();
          
        if (state != BookState.DontAble || state != BookState.Catched)
        {
            //チュートリアル
            if (GameManager.Instance.state == GameState.Tutorial)
            {
                if (tutorialmanager.state == TutorialState.Boo)
                {
                    GetMiddle();
                    BookRotation();
                    OpenAndClose();
                    HaveOrBag();
                }
            }
            //本番
            if (GameManager.Instance.state == GameState.Game)
            {
                if (DifferenceManager.Instance.state == DifferenceState.Catch)
                {
                    GetMiddle();
                    BookRotation();
                    OpenAndClose();
                    HaveOrBag();
                }
            }

            if(GameManager.Instance.state == GameState.ED)
            {
                if (state == BookState.Have)
                {
                    GetMiddle();
                    BookRotation();
                    OpenAndClose();
                }
                    ResultBook();
            }
        }

        //DEBUG//
        //debugLog.text = "" + state;
        if (debug)
        {
            GetMiddle();
            BookRotation();
            OpenAndClose();
            HaveOrBag();
        }


        if (Input.GetKey(KeyCode.B))
        {
            GameManager.Instance.ChangeState(GameState.Tutorial);
        }
        //DEBUG//
    }
    void FixedUpdate()
    {
        //OpenAndClose();
    }
    //
    //

    //手の真ん中とる
    Vector3 Pos;
    private void GetMiddle()
    {
        Pos = (pinkys[0].transform.position + pinkys[1].transform.position) / 2;
        Pos.y += 0.05f;

        middleOfHands.transform.position = Pos;
        //Debug.Log("mannaka");
    }

    //このステイト中はこれをする
    private float speed = 3.0f;
    private void CheckBookState()
    {
        switch (state)
        {
            case BookState.Bag:
                if (ableBag)
                {
                    transform.position = bag.transform.position;
                }
                break;
            case BookState.Have:
                transform.position = Vector3.MoveTowards(transform.position, middleOfHands.transform.position, speed * Time.deltaTime);
                transform.localScale = new Vector3(4f, 4f, 4f);
                break;
            case BookState.Catched:
                break;
            case BookState.Result:
                transform.localScale = new Vector3(4f, 4f, 4f);
                break;
            default:
                break;
        }
        //Debug.Log(state);
    }

    //ステイトが変わったときに一度実行
    private void ChangeBookState(BookState newState)
    {
        if (state == newState) return;

        state = newState;

        switch (state)
        {
            case BookState.Bag:
                //Instantiate(smokeEffect, middleOfHands.transform.position, Quaternion.identity);
                StartCoroutine(BookToBagRoutine());
                resultText.text = "";
                break;
            case BookState.Have:
                ableBag = false;
                transform.localScale = new Vector3(4f, 4f, 4f);
                if (GameManager.Instance.state != GameState.Tutorial)
                {
                    tutorialAbleCatch = true;
                }
                Instantiate(smokeEffect, middleOfHands.transform.position, Quaternion.identity);
                break;
            case BookState.Result:
                resultText.text = "持ってみて！";
                transform.localScale = new Vector3(4f, 4f, 4f);
                transform.position = resultPos.transform.position;
                transform.localRotation = resultPos.transform.localRotation;
                break;
            default:
                break;
        }

        Debug.Log(newState);
    }
    //捕まえた時一瞬で戻らないように
    private IEnumerator BookToBagRoutine()
    {
        yield return null;

        transform.localScale = Vector3.zero;
        ableBag = true;
    }

    //↓↓ここからは本の動き↓↓//
    private float angle = 45f;
    //private float threshold;
    private float rightHand;
    private float leftHand;
    private bool CheckDirection()
    {
        //右から左へのベクトル　←
        Vector3 handdirection = HandTraking.Instance.GetLeftHandPos() - HandTraking.Instance.GetRightHandPos();

        //ズレが35度以内でおけ
        //threshold = Mathf.Cos(Mathf.Deg2Rad * angle);
        leftHand = Vector3.Angle(HandTraking.Instance.GetLeftHandBack(), -handdirection);
        rightHand = Vector3.Angle(HandTraking.Instance.GetRightHandBack(), handdirection);

        if (leftHand <= angle || rightHand <= angle)
        {

            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CheckDistance()
    {
        //手が離れすぎたら戻る
        if (thumb_Distance > bagDistance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void HaveOrBag()
    {
        if (CheckDistance() && CheckDirection())
        {
            ChangeBookState(BookState.Have);
        }
        else
        {
            ChangeBookState(BookState.Bag);
        }
    }

    bool ableclose = false;
    private bool playOnce = true;//SE一回だけ鳴らす
    private float thumb_Distance;
    private float index_Distance;
    private float tutorial = 0f;
    private void OpenAndClose()
    {
        ableclose = bookAnim.GetBool("ableClose");

        //薬指同士がどの程度近づいているか
        index_Distance = Vector3.Distance(HandTraking.Instance.GetLeftHandIndexPos(), HandTraking.Instance.GetRightHandIndexPos());
        //debugLog.text = "" + index_Distance;

        //親指同士がどの程度近づいているか
        thumb_Distance = Vector3.Distance(HandTraking.Instance.GetLeftHandThumbPos(), HandTraking.Instance.GetRightHandThumbPos());
        debugLog.text = "" + thumb_Distance;

        //近づいているとき
        //本が閉じる
        if (thumb_Distance <= 0.1f)
        {
            bookAnim.SetBool("ableClose", true);
        }
        //離れているとき
        //本が開く
        else if (thumb_Distance >= 0.12f && thumb_Distance < bagDistance)
        {
            bookAnim.SetBool("ableClose", false);
        }

        //閉じた
        if (ableclose)
        {
            //何回か閉じたり開けたりしてもらう
            if (TutorialManager.Instance.state == TutorialState.Boo)
            {
                if (!doClose)
                {
                    tutorial++;
                }
            }
        }

        if (tutorial > 3f)
        {
            doClose = true;
        }
    }
    private float rotationSpeed = 5.0f;
    private void BookRotation()
    {
        //掌のpositionを取得してベクトルに
        Vector3 hand = (palms[0].transform.position - palms[1].transform.position).normalized;

        //両方の手の角度が著しく違っていたら手の角度は判定しない
        float handAngle = Vector3.Angle(palms[0].transform.up, palms[1].transform.up);

        //本のZ軸を決める
        //二つのベクトルと直行（垂直）になるベクトルを求める
        Vector3 zAxis = Vector3.Cross(Vector3.down, hand).normalized;

        //本のY軸を zAxis と hand の外積で作る（直交するベクトル）
        Vector3 yAxis = Vector3.Cross(zAxis, hand).normalized;

        Quaternion finalrotation;
        // 回転を作る（X軸 = hand, Y軸 = yAxis）
        //LookRotation(z軸が向く方向、Y軸が向く方向)
        if (handAngle <= 35.0f)
        {
            //debugLog.text = ("same!");
            Quaternion baserotation = Quaternion.LookRotation(zAxis, yAxis);

            float pitch = palms[0].transform.rotation.eulerAngles.x;
            if (pitch > 180f)
            {
                pitch -= 360f; //-180~180
            }

            Quaternion pitchonly = Quaternion.Euler(-pitch, 0f, 0f);
            finalrotation = baserotation * pitchonly;
        }
        else
        {
            //debugLog.text = ("Umm..");
            finalrotation = Quaternion.LookRotation(zAxis, yAxis);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, finalrotation, Time.deltaTime * rotationSpeed);
    }
    private IEnumerator BookCloseRutine()
    {
        if(aimBoo)
        {
            booAnim.SetBool("isCatch", true);
            Instantiate(catchEffect, transform.position, Quaternion.identity);
            Instantiate(smokeEffect, transform.position, Quaternion.identity);
            if (GameManager.Instance.state == GameState.Tutorial)
            {
                ChangeBookState(BookState.Bag);
                tutorialmanager.ChangeState(TutorialState.finish);
            }
            playsound.PlayOneShot(catchAGhost);
            yield return new WaitForSeconds(1f);

            booAnim = null;

            aimBoo = false;
        }
        else
        {
            if(aimMob)
            {
                mob.SetCaught(true);
                mob = null;

                aimMob = false;
            }
            playsound.PlayOneShot(closeABook);
        }
    }

    private void ResultBook()
    {
        float lefthandTobook = Vector3.Distance(HandTraking.Instance.GetLeftHandPos(), resultPos.transform.position);
        float righthandTobook = Vector3.Distance(HandTraking.Instance.GetRightHandPos(), resultPos.transform.position);

        if (lefthandTobook <= 1.0f || righthandTobook <= 1.0f)
        {
            ChangeBookState(BookState.Have);
        }
        else
        {
            ChangeBookState(BookState.Result);
        }

        if (state == BookState.Have)
        {
            if(ableclose)
            {
                resultText.text = "";
            }
            else
            {
                if(result)
                {
                    resultText.text = "クリアタイム:" + DifferenceManager.Instance.ClearTime;
                }
                else
                {
                    resultText.text = "捕まえたおばけ" + DifferenceManager.Instance.ClearBoo.ToString() + "ひき";
                }
            }
        }
    }
    //↑↑ここからは本の動き↑↑//

    //GAMEMANAGER//
    private void WhenGameStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Attention:
                ChangeBookState(BookState.Bag);
                break;
            case GameState.OP:
                break;
            case GameState.Tutorial:
                break;
            case GameState.LookAround:
                break;
            case GameState.Game:
                //ChangeBookState(BookState.Bag);
                break;
            case GameState.ED:
                ChangeBookState(BookState.Result);
                break;
            default:
                break;
        }
    }
    //TUTORIALMANAGER//
    private void WhenTutorialStateChange(TutorialState newState)
    {
        if (newState == TutorialState.Boo)
        {
            StartCoroutine(GhostSubTitle());
            ChangeBookState(BookState.Have);
        }
    }
    private void WhenDifferenceStateChange(DifferenceState newState)
    {
        if (newState == DifferenceState.Serch)
        {
            ChangeBookState(BookState.Bag);
        }
        else if (newState == DifferenceState.Catch)
        {
            ChangeBookState(BookState.Have);
        }
        else if (newState == DifferenceState.Finish)
        {
            ChangeBookState(BookState.Bag);
        }
    }
    private IEnumerator GhostSubTitle()
    {
        Subtitle.Instance.StopSubTitle();

        yield return null;

        Subtitle.Instance.InstanceSubTitle(dialogueData, 15, 16);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 17, 17);
        while (!doClose)
        {
            yield return null;
        }
        Subtitle.Instance.StopSubTitle();

        Subtitle.Instance.InstanceSubTitle(dialogueData, 18, 19);
        TutorialManager.Instance.Moveboo();
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        tutorialAbleCatch = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (bookAnim.GetBool("IsCatch"))
        {
            if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
            {
                //debugLog.text = ("touch!!");
                bookAnim.SetTrigger("BackBag");
                ChangeBookState(BookState.Bag);
            }
        }*/
    }
    Animator booAnim;
    Mob mob;
    private void OnTriggerStay(Collider other)
    {
        //チュートリアルで使う
        if (tutorialAbleCatch)
        {
            if (other.gameObject.CompareTag("Boo"))
            {
                //colliderに入っているとき
                if (!aimBoo)
                {
                    booAnim = other.GetComponentInChildren<Animator>();
                    if (booAnim == null)
                    {
                        Debug.Log("null");
                    }

                    aimBoo = true;
                }
            }
            if (other.gameObject.CompareTag("Twins"))
            {
                //colliderに入っているとき
                if (!aimBoo)
                {
                    booAnim = other.GetComponentInChildren<Animator>();
                    if (booAnim == null)
                    {
                        Debug.Log("null");
                    }

                    aimBoo = true;
                }
            }
            if (other.gameObject.CompareTag("Mob"))
            {
                if (!aimMob)
                {
                    mob = other.GetComponent<Mob>();
                    if (mob == null)
                    {
                        Debug.Log("null");
                    }

                    aimMob = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Boo"))
        {
            aimBoo = false;
        }
        if (other.gameObject.CompareTag("Twins"))
        {
            aimBoo = false;
        }
        if (other.gameObject.CompareTag("Mob"))
        {
            aimMob = false;
        }
    }

    public void BookClose()
    {
        StartCoroutine(BookCloseRutine());

        if (GameManager.Instance.state == GameState.ED)
        {
            if(result)
            {
                result = false;
            }
            else
            {
                result = true;
            }
        }
    }
    /*public void InstanceCatchEffect()
    {
        //Instantiate(catchEffect, transform.position, Quaternion.identity);
    }
    public void EndCatchAnimation()
    { 
        catchGhost = false;
    }*/
    //<<DontUse>>//
    /*private void PositionCheak()
    {
        //指が閉じているか(一旦没)
        //left_Distance = Vector3.Distance(indexs[0].transform.position, middles[0].transform.position);
        //right_Distance = Vector3.Distance(indexs[1].transform.position, middles[1].transform.position);

        //小指同士がある程度近づいているか(一旦没)
        //pinky_Distance = Vector3.Distance(pinkys[0].transform.position, pinkys[1].transform.position);

        if (left_Distance <= 0.02f && right_Distance <= 0.02)
        {
            fingerIsClose = true;
        }
        else
        {
            fingerIsClose = false;
        }

        if(pinky_Distance<=0.02f)
        {
            pinkyIsClose = true;
        }
        else
        {
            pinkyIsClose = false;
        }

        //debugLog.text = pinky_Distance.ToString();
    }
    for (int i = 0; i < 2; i++)
       {
           // 上方向ベクトル同士の内積を計算（1に近いほど向きが同じ)（没）
           //float frontOrBack = Vector3.Dot(palms[i].transform.up.normalized, Vector3.up);
           //掌のベクトルが逆になってるのでこれでおけ
           //<<手の平が下向き180度に向いているか>>(没)
           //float handAngle = Vector3.Angle(palms[i].transform.up,Vector3.up);
           //rightOrLeft= Vector3.Dot(palms[0].transform.up.normalized, palms[1].transform.up.normalized);

           //<<手の甲が向かい合っているかを判定>>//
           //手はvector3.forwardが下に伸びてる
           //左
           if (i == 0)
           {
               //rightOrLeft = Vector3.Angle(palms[i].transform.up.normalized, Vector3.right);
               rightOrLeft = Vector3.Angle(palms[i].transform.up.normalized, Vector3.left);
               //rightOrLeft = Vector3.Dot(palms[i].transform.up.normalized, Vector3.right);
           }
           //右
           else if (i == 1)
           {
               //rightOrLeft = Vector3.Angle(palms[i].transform.up.normalized, Vector3.left);
               rightOrLeft = Vector3.Angle(palms[i].transform.up.normalized, Vector3.right);
               //rightOrLeft = Vector3.Dot(palms[i].transform.up.normalized, Vector3.left);
           }

           if (rightOrLeft <= angle)
           {
               //debugLog.text = ("umm...");
               //state = BookState.Back;
               state = BookState.Bag;
               //haveBook = false;
               breakSound;
           }
           else
           {
               //本がバッグにあったら
               if (state == BookState.Bag)
               {
                   //debugLog.text = ("book!!");
                   state = BookState.Have;
                   //state = BookState.Go;
                   //haveBook = true;
               }
           }
       }
    //動いていい時にだけ発動
    if (state == BookState.Go || state == BookState.Back)
    {
        if (other.gameObject == bag)
        //if (other.gameObject.name == "BookBag")
        {
            state = BookState.Bag;
        }
        else if (other.gameObject == middleOfHands)
        //else if (other.gameObject.name == "MiddleOfHands")
        {
            //transform.position = Pos;
            SoundManager.Instance.PlaySE(openABook);
            state = BookState.Have;
        }
    }
}
      private void MoveToCenter()
    {
        CameraManager.Instance.SubtitleControl(transform, 0.8f, 0, -0.2f);
    }
    */
}