using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public bool caught {  get; private set; }

    private GameObject player;
    private bool setTarget;
    private bool isRight;
    private float speed;
    private float radius;
    Vector3 Direction;
    Vector3 nowTargetPos;
    Vector3 futureTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera");
        setTarget = false;
        caught = false;
        isRight = true;
        speed = 0.5f;
        radius = 0.8f;

        nowTargetPos = transform.position;
        futureTargetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //捕まった
        if(caught)
        {
            transform.localScale = Vector3.zero;
        }
        //逃げてる
        else
        {
            MobRunAway();
        }

        if (DifferenceManager.Instance.state == DifferenceState.Finish)
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        Direction = Vector3.Slerp(Direction, (nowTargetPos - transform.position).normalized, Time.deltaTime * 5f);
    }
    private void MobRunAway()
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

        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

        transform.position += Direction * speed * Time.deltaTime;
        LookTarget(futureTargetPos);

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
        float yOffset = Random.Range(-0.2f, 0.2f);  // 上下の揺れ幅

        /*futureTargetPos = new Vector3((player.transform.position.x + offset.x) * radius, 
            player.transform.position.y + yOffset, 
            (player.transform.position.z + offset.z) * radius);*/
        futureTargetPos = player.transform.position + offset * radius;
        futureTargetPos.y += yOffset;

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
    private void LookTarget(Vector3 target)
    {
        //敵とプレイヤーの間を見る
        Vector3 midPos = (target + player.transform.position) * 0.5f;

        Quaternion targetRot = Quaternion.LookRotation(midPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3.0f);
    }

    public void SetCaught(bool b)
    {
        caught = b;
    }
}
