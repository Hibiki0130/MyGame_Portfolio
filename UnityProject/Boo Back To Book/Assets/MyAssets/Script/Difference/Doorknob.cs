using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Doorknob : MonoBehaviour
{
    private enum DoorknobState
    {
        Desk,
        Out,
        Correct,
    }
    private DoorknobState state;

    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject doornobGhost;

    private GameObject handGrab;
    private Rigidbody rB;
    private BoxCollider bC;
    private float subtitleTime;
    private bool firstTouch;
    private bool changeSecondSubtitle;
    private bool changeThirdSubtitle;
    private bool rightIsTouch;
    private bool leftIsTouch;   

    Vector3 firstPos;
    Vector3 firstRot;
    Vector3 doorPos;

    Door doorscript;
    Reset reset;
    Doorknob doorknob;
    // Start is called before the first frame update
    void Start()
    {
        state = DoorknobState.Desk;

        TutorialManager.Instance.OnTutorialStateChange += StartFirstSubtitle;

        subtitleTime = 0f;
        firstTouch = false;
        changeSecondSubtitle = true;
        changeThirdSubtitle = true;
        rightIsTouch = false;
        leftIsTouch = false;

        firstPos = transform.position;
        firstRot = transform.eulerAngles;
        doorPos = doornobGhost.transform.position;

        rB = GetComponent<Rigidbody>();
        bC = GetComponent<BoxCollider>();
        doorscript = door.GetComponent<Door>();
        reset = GetComponent<Reset>();
        doorknob = GetComponent<Doorknob>();

        handGrab = transform.GetChild(0).gameObject;
        handGrab.SetActive(true);
        bC.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        if (Input.GetKey(KeyCode.D))
        {
            TutorialManager.Instance.ChangeState(TutorialState.Boo);
            state = DoorknobState.Correct;
        }
        //Debug

        switch (state)
        {
            case DoorknobState.Desk:
                ResetPos();
                break;
            case DoorknobState.Out:
                OnPhysics();
                //落ちた時の処理はReset
                break;
            case DoorknobState.Correct:
                break;
        }

        //字幕
        if (TutorialManager.Instance.state == TutorialState.Door)
        {
            DoorSubtitle();
        }
    }
    //机の上に戻す(Resetに移動)
    private void ResetPos()
    {
        rB.isKinematic = true;
        rB.useGravity = false;

        transform.position = firstPos;
        transform.eulerAngles = firstRot;
    }
    private void OnPhysics()
    {
        rB.isKinematic = false;
        rB.useGravity = true;
    }
    private void DoorSubtitle()
    {
        if(firstTouch)
        {
            //ドアノブを見つけた後
            if (changeSecondSubtitle)
            {
                Subtitle.Instance.StopSubTitle();
                Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 13, 13);
                changeSecondSubtitle = false;
            }
            //ドアノブを見つけた後なかなか戻さないとき
            else
            {
                if (subtitleTime < 10f)
                {
                    subtitleTime = Time.deltaTime;
                }
                else
                {
                    if(changeThirdSubtitle)
                    {
                        Subtitle.Instance.StopSubTitle();
                        Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 14, 14);
                        changeSecondSubtitle = false;
                    }
                }
            }
        }
    }
    private void StartFirstSubtitle(TutorialState newState)
    {
        if (newState == TutorialState.Door)
        {
            bC.enabled = true;
            Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 12, 12);
        }  
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ghost"))
        {
            if (state != DoorknobState.Correct)
            {
                //無効
                handGrab.SetActive(false);
                reset.enabled = false;
                rB.useGravity = false;
                rB.isKinematic = true;

                transform.position = doorPos;
                transform.eulerAngles = firstRot;

                //そのまま子にする
                //gameObject.transform.parent = door.gameObject.transform;
                gameObject.transform.SetParent(door.transform, true);

                //ゴースト破壊
                Destroy(doornobGhost);

                if (TutorialManager.Instance.state == TutorialState.Door)
                {
                    TutorialManager.Instance.ChangeState(TutorialState.Boo);
                }

                //正解
                state = DoorknobState.Correct;
            }
        }

        if (other.gameObject.CompareTag("MiniTable"))
        {
            //Debug.Log("te-buru");
            state = DoorknobState.Desk;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("MiniTable"))
        {
            //Debug.Log("te-buru");
            state = DoorknobState.Desk;
        }
        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))
        {
            if (!firstTouch)
            {
                firstTouch = true;
            }
        }

        if (other.gameObject.CompareTag("RightHand"))
        {
            rightIsTouch = true;
        }

        if (other.gameObject.CompareTag("LeftHand"))
        {
            leftIsTouch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MiniTable"))
        {
            if (state != DoorknobState.Correct)
            {
                state = DoorknobState.Out;
            }
        }

        if (other.gameObject.CompareTag("RightHand"))
        {
            rightIsTouch = false;
        }

        if (other.gameObject.CompareTag("LeftHand"))
        {
            leftIsTouch = false;
        }
    }
}
//dontuse
/*private void Check()
{
    float subtitleDistance = Vector3.Distance(transform.position, doornobGhost.transform.position);

    //正解（かけら）
    if (subtitleDistance < 0.05f)
    {
        float anglediff = Quaternion.Angle(transform.rotation, doornobGhost.transform.rotation);
        //正解（傾きも）
        //if (anglediff < 5f)
            //grabbable（つかめる挙動をつかさどってるとこ）の無効化
            //var grabbable = handGrabs[i].GetComponent<Grabbable>();
            //grabbable.enabled = false;
            //pieceColliders[i].enabled = false;

            //handgrabを無効化
            handGrab.SetActive(false);

            transform.position = doornobGhost.transform.position;
            transform.rotation = doornobGhost.transform.rotation;

            rB.useGravity = false;

            //Destroy(doornobGhost);

        tutorialmanager.ChangeState(TutorialState.Boo);
        state = DoorknobState.Correct;
    }
}

//手をひねって開けようとしたときの残骸
Vector3 touchrot;
Vector3 currentRot;
float twist;
bool setrot = false;
CheckHand();

   if (setrot)
   {
       if (rightIsTouch)
       {
           currentRot = HandTraking.Instance.GetRightHandBack();
           twist = Vector3.SignedAngle(touchrot, currentRot, Vector3.forw ard);
       }
       else
       {
           currentRot = HandTraking.Instance.GetLeftHandBack();
           twist = Vector3.SignedAngle(touchrot, currentRot, Vector3.forward);
       }

       //debugLog.text = "" + twist;

       if (twist > 60f)
       {
           //debugLog.text = "open";
           doorscript.SetAbleOpen(true);
       }
       else
       {
           //debugLog.text = "close";
           //doorscript.SetAbleOpen(false);
       }
   }
private void CheckHand()
{
//どっちかの手で触っているとき
if (leftIsTouch || rightIsTouch)
{
    CheckOpenWay();

    //debugLog.text = "right" + rightIsTouch + "left" + leftIsTouch;
    if (!setrot)
    {
        //両方で触っているor右手だけ
        if (rightIsTouch)
        {
            touchrot = HandTraking.Instance.GetRightHandBack();
        }
        //左手だけ
        else
        {
            touchrot = HandTraking.Instance.GetLeftHandBack();
        }

        setrot = true;
    }
}
//どっちの手も触れていないとき
else
{
    //初期化
    //debugLog.text = "donttouch";
    touchrot = Vector3.zero;
    currentRot = Vector3.zero;
    setrot = false;
}
}

float handrot;
float handrot1;
bool way;
int num;
private void CheckOpenWay()
{
if(rightIsTouch)
{
    debugLog.text = "" + num;
    handrot = Vector3.Angle(HandTraking.Instance.GetRightHandUp(), Vector3.up);
    handrot1 = Vector3.Angle(HandTraking.Instance.GetRightHandBack(), transform.forward);

    if (handrot > 30f && 120f > handrot && 30f >= handrot1 && 120f > handrot1)
    {
        way = true;
        num = 0;
    }
    else if ( 30f >= handrot && 30f >= handrot1)
    {
        way = false;
        num = 1;
    }
    else
    {
        num = 2;
    }
}
else
{
    handrot = Vector3.SignedAngle(HandTraking.Instance.GetLeftHandUp(), Vector3.up, transform.right);
    debugLog.text = "" + handrot;
}

//debugLog.text = "" + handrot;

//if(handrot)


}
    bool rightHandHold = false;
    bool leftHandHold = false;
    Vector3 holdHand = new Vector3();
    float holdDistance = 0.15f;
    Quaternion offset;
    Quaternion rot;
    //押して開くandドアノブ掴んで開く
private void OpenDoor()
{
    //ドアノブを掴んでるとき：ドアノブと手の距離0.13までくらい
    float distancetotheRight = Vector3.Distance(transform.position, HandTraking.Instance.GetRightHandPos());
    float distancetotheLeft= Vector3.Distance(transform.position, HandTraking.Instance.GetLeftHandPos());

    if (rightIsTouch)
    {
        rightHandHold = HandTraking.Instance.GetRightHold();
    }
    else
    {
        rightHandHold = false;
    }

    if (leftIsTouch)
    {
        leftHandHold = HandTraking.Instance.GetLeftHold();
    }
    else
    {
        leftHandHold = false;
    }


    //debugLog.text = "" + leftHandHold;
    if (rightHandHold || leftHandHold)
    {
        doorscript.SetIsHold(true);

        //どっちもか右手
        if (rightHandHold)
        {
            holdHand = HandTraking.Instance.GetRightHandPos();
        }
        //左手
        else
        {
            holdHand = HandTraking.Instance.GetLeftHandPos();
        }

        //door←hand
        Vector3 handTodoor = door.transform.position - holdHand;

        //上下運動を無効化
        handTodoor.y = 0f;
        handTodoor.Normalize();

        //手とのズレを治す
        offset = Quaternion.Euler(0, -10f, 0);
        Vector3 newhandTodoor = offset * handTodoor;

        rot = Quaternion.FromToRotation(transform.right, newhandTodoor) * transform.rotation;
        door.transform.rotation = rot;
    }
    else
    {
        doorscript.SetIsHold(false);
    }

    //debugLog.text = "" + rightHandHold;
}*/
