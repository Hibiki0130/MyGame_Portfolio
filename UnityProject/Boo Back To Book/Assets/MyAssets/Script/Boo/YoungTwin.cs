using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YoungTwin : MonoBehaviour
{
    private enum YoungTwinState
    {
        Appearance,
        Usual,
        Crazy,
        Catch,
    }
    private YoungTwinState state;

    [SerializeField] private GameObject kirakiraEffect;

    private GameObject player;
    private GameObject book;
    private Animator myAnimator;
    private Coroutine usualRoutine;
    private BoxCollider bC;
    private bool setTarget;
    private bool isRight;
    private bool isCrazy;
    private float radius;
    private float speed;
    private float distance;

    Vector3 Direction;
    Vector3 nowTargetPos;
    Vector3 futureTargetPos;
    Vector3 instancePos;
    Vector3 myScale;
    Vector3[] target;
    private TwinsAnimation twinsanimation;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("BooCamera");
        book = GameObject.FindGameObjectWithTag("Book");
        twinsanimation = GetComponentInChildren<TwinsAnimation>();
        myAnimator = GetComponentInChildren<Animator>();
        bC = GetComponent<BoxCollider>();

        setTarget = false;
        isRight = true;
        isCrazy = false;
        radius = 0.8f;
        speed = 1.0f;
        distance = 0.5f;

        instancePos = transform.position;
        nowTargetPos = transform.position;
        futureTargetPos = transform.position;
        myScale = new Vector3(0.5f, 0.5f, 0.5f);
        target = new Vector3[3];
        bC.enabled = false;

        ChangeMyState(YoungTwinState.Appearance);
    }

    // Update is called once per frame
    void Update()
    {
        //常にプレイヤーを見る
        transform.LookAt(player.transform.position);

        if (state == YoungTwinState.Appearance)
        {
            Vector3 target = new Vector3(instancePos.x, player.transform.position.y + 0.2f, instancePos.z);
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);

            if (twinsanimation.YoungIsFirstAppearance)
            {
                ChangeMyState(YoungTwinState.Usual);
            }
        }
        else if (state == YoungTwinState.Usual)
        {
            //おねーちゃんが捕まったら
            if (myAnimator.GetBool("Crazy"))
            {
                ChangeMyState(YoungTwinState.Crazy);
            }
            //自分が捕まったら
            if (myAnimator.GetBool("isCatch"))
            {
                ChangeMyState(YoungTwinState.Catch);
            }
        }
        if (state == YoungTwinState.Crazy)
        {
            TwinCrazy();

            if (myAnimator.GetBool("isCatch"))
            {
                ChangeMyState(YoungTwinState.Catch);
            }
        }
       if (DifferenceManager.Instance.state == DifferenceState.Finish)
       {
           Destroy(gameObject);
       }

        //DEBUG//
        if (Input.GetKey(KeyCode.Y))
        {
            myAnimator.SetBool("isCatch", true);
        }
        //DEBUG//
    }
    void FixedUpdate()
   {
       //usual挙動
       if (state == YoungTwinState.Usual)
       {
           UsualSetTarget();
       }
       //crazy挙動
       if (state == YoungTwinState.Crazy)
       {
           Direction = Vector3.Slerp(Direction, (nowTargetPos - transform.position).normalized, Time.deltaTime * 5f);
       }
   }
   private void ChangeMyState(YoungTwinState newState)
   {
       //変わった時一回だけ処理
       if (state == newState) return;

       state = newState;
       switch (newState)
       {
           case YoungTwinState.Usual:
               transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z);
               usualRoutine = StartCoroutine(UsualRoutine());
               break;
           case YoungTwinState.Crazy:
               StopRunAwayRoutine();
                isCrazy = true;
               transform.localScale = myScale;
               myAnimator.SetBool("Crazy", true);
               break;
           case YoungTwinState.Catch:
                //大きさはアニメーションで小さくなる
                StopRunAwayRoutine();
                if(isCrazy)
                {
                    DifferenceManager.Instance.SetFixedPlank(Boos.Twins);
                }
                break;
            default:
                break;
        }
    }
    private void UsualSetTarget()
    {
        float value_x = 0.4f;
        float value_y = 0.2f;
        Vector3 center = player.transform.position + player.transform.forward * distance;

        for (int i = 0; i < target.Length; i++)
        {
            //float target_x = 0f;
            float target_y = 0f;

            switch (i)
            {
                case 0:
                    target_y = center.y + value_y; break;
                case 1:
                    target_y = center.y; break;
                case 2:
                    target_y = center.y - value_y; break;
                default:
                    break;
            }

            //target_x = center.x + (player.transform.right.x * value_x);

            //妹は左側
            target[i] = new Vector3(center.x, target_y, center.z) + (-player.transform.right * value_x);
        }
    }
    private int preTarget;
    private void UsualController()
    {
        int selectTarget = 0;
        while (true)
        {
            selectTarget = Random.Range(0, 3);
            if (preTarget != selectTarget)
            {
                break;
            }
        }

        futureTargetPos = target[selectTarget];

        preTarget = selectTarget;
    }
    private void StopRunAwayRoutine()
    {
        StopCoroutine(usualRoutine);

    }
    private IEnumerator UsualRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            myAnimator.SetTrigger("LeftTurn");
            while (twinsanimation.YoungIsAppearance)
            {
                //Debug.Log("tomatteru");
                yield return null;
            }

            bC.enabled = false;
            transform.localScale = Vector3.zero;
            UsualController();
            yield return new WaitForSeconds(1f);
            transform.position = futureTargetPos;
            yield return null;

            myAnimator.SetTrigger("RightTurn");
            Instantiate(kirakiraEffect, transform.position, Quaternion.identity);
            transform.localScale = myScale;
            bC.enabled = true;
        }
    }
    private void TwinCrazy()
    {
        if (!setTarget)
        {
            SetTarget();
        }
        else
        {
            //ある程度近づいたら次のターゲット決める
            if (Vector3.Distance(transform.position, nowTargetPos) < 0.5f)
            {
                setTarget = false;
            }
        }

        myAnimator.SetBool("AbleMove", true);

        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

        transform.position += Direction * speed * Time.deltaTime;
        //常にプレイヤーを見る
        //LookTarget(futureTargetPos);

        if (Vector3.Distance(transform.position, nowTargetPos) < 0.1f)
        {
            nowTargetPos = futureTargetPos;
        }
    }
    private void SetTarget()
    {
        Vector3 forward = player.transform.forward;
        Vector3 right = player.transform.right;

        float angle;
        if (isRight)
        {
            //ランダムで角度を決めてラジアン化
            angle = Random.Range(-60f, 0f) * Mathf.Deg2Rad;
        }
        else
        {
            angle = Random.Range(0f, 60f) * Mathf.Deg2Rad;
        }

        Vector3 offset = forward * Mathf.Cos(angle) + right * Mathf.Sin(angle);
        futureTargetPos = player.transform.position + offset * radius;

        if (isRight)
        {
            isRight = false;
        }
        else
        {
            isRight = true;
        }

        setTarget = true;
    }
    /*private void LookTarget(Vector3 target)
    {
        //敵とプレイヤーの間を見る
        Vector3 midPos = (target + player.transform.position) * 0.5f;

        Quaternion targetRot = Quaternion.LookRotation(midPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3.0f);
    }*/
}
