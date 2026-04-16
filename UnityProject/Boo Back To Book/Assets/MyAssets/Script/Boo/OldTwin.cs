using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Oculus.Interaction.Context;

public class OldTwin: MonoBehaviour
{
    private enum OldTwinState
    {
        Appearance,
        Usual,
        Crazy,
        Catch,
    }
    private OldTwinState state;

    [SerializeField] private GameObject yangerTwinPrefub;
    [SerializeField] private GameObject kirakiraEffect;
    [SerializeField] private AudioClip dollBGM;

    private GameObject player;
    private GameObject youngerTwin;
    private GameObject book;
    //自分のと妹を取得
    private Animator myAnimator;
    private Animator youngerAnimator;

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

    TwinsAnimation myAnimationScript;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(dollBGM);

        youngerTwin = Instantiate(yangerTwinPrefub, transform.position, transform.rotation);

        player = GameObject.FindGameObjectWithTag("BooCamera");
        book = GameObject.FindGameObjectWithTag("Book");
        myAnimationScript = GetComponentInChildren<TwinsAnimation>();
        myAnimator = GetComponentInChildren<Animator>();
        youngerAnimator = youngerTwin.GetComponentInChildren<Animator>();
        bC = GetComponent<BoxCollider>();

        setTarget = false;
        isRight = false;
        isCrazy = false;
        radius = 0.8f;
        speed = 0.5f;
        distance = 0.5f;

        instancePos = transform.position;
        nowTargetPos = transform.position;
        futureTargetPos = transform.position;
        myScale = new Vector3(0.5f, 0.5f, 0.5f);

        //最初は捕まえられない
        bC.enabled = false;
        //最初のターゲットの数（一人づつ）
        target = new Vector3[3];

        ChangeMyState(OldTwinState.Appearance);
    }

    // Update is called once per frame
    void Update()
    {
        //常にプレイヤーを見る
        transform.LookAt(player.transform.position);

        if (state == OldTwinState.Appearance)
        {
            //playerの上までいく
            Vector3 target = new Vector3(instancePos.x, player.transform.position.y + 0.2f, instancePos.z);
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);

            if (myAnimationScript.OldIsFirstAppearance)
            {
                ChangeMyState(OldTwinState.Usual);
            }
        }
        else if (state == OldTwinState.Usual)
        {
            //妹が捕まえられたら
            if (youngerAnimator.GetBool("isCatch"))
            {
                ChangeMyState(OldTwinState.Crazy);
            }
            //自分が捕まったら
            if (myAnimator.GetBool("isCatch"))
            {
                youngerAnimator.SetBool("Crazy", true);
                ChangeMyState(OldTwinState.Catch);
            }
        }
        else if (state == OldTwinState.Crazy)
        {
            TwinCrazy();

            if (myAnimator.GetBool("isCatch"))
            {
                ChangeMyState(OldTwinState.Catch);
            }
        }

        //三分経ったら消える。
        if (DifferenceManager.Instance.state == DifferenceState.Finish)
        {
            Destroy(gameObject);
        }

        //DEBUG//
        if (Input.GetKey(KeyCode.U))
        {
            myAnimator.SetBool("isCatch", true);
        }
        //DEBUG//
    }
    private void FixedUpdate()
    {
        //usual挙動
        if (state == OldTwinState.Usual)
        {
            UsualSetTarget();
        }
        //crazy挙動
        if (state == OldTwinState.Crazy)
        {
            Direction = Vector3.Slerp(Direction, (nowTargetPos - transform.position).normalized, Time.deltaTime * 5f);
        }
    }
    private void ChangeMyState(OldTwinState newState)
    {
        //変わった時一回だけ処理
        if (state == newState) return;
        state = newState;

        switch (newState)
        {
            case OldTwinState.Usual:
                transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
                usualRoutine = StartCoroutine(UsualRoutine());
                break;
            case OldTwinState.Crazy:
                StopUsualRoutine();
                isCrazy = true;
                transform.localScale = myScale;
                bC.enabled = true;
                myAnimator.SetBool("Crazy", true);
                break;
            case OldTwinState.Catch:
                //大きさはアニメーションで小さくなる
                StopUsualRoutine();
                if (isCrazy)
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

            //おねーちゃんは右側
            target[i] = new Vector3(center.x, target_y, center.z) + (player.transform.right * value_x);
        }
    }
    private int preTarget;
    private  void UsualController()
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
    private void  StopUsualRoutine()
    {
        StopCoroutine(usualRoutine);
    }
    private IEnumerator UsualRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            myAnimator.SetTrigger("LeftTurn");
            while (myAnimationScript.OldIsAppearance)
            {
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
            CrazySetTarget();
        }
        else
        {
            //ある程度近づいたら次のターゲット決める
            if (Vector3.Distance(transform.position, nowTargetPos) < 0.5f)
            {
                setTarget = false;
            }
        }

        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

        transform.position += Direction * speed * Time.deltaTime;
        //常にプレイヤーを見る
        //LookTarget(futureTargetPos);

        if (Vector3.Distance(transform.position, nowTargetPos) < 0.1f)
        {
            nowTargetPos = futureTargetPos;
        }
    }
    private void CrazySetTarget()
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
