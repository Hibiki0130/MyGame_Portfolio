using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum BooState
{
    Disappeared,
    Appeared,
    Moved,
    Catched,
}
public class Boo : MonoBehaviour
{
    [SerializeField] private AudioClip butlerBGM;

    public BooState state { get; set; }

    private GameObject player;
    private Animator anim;
    private bool setTarget;
    private bool isRight;
    private float radius;
    private float speed;

    Vector3 Direction;
    Vector3 nowTargetPos;
    Vector3 futureTargetPos;
    Vector3 gaze;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(butlerBGM);

        setTarget = false;
        isRight = false;
        radius = 0.8f;
        speed = 0.5f;


        player = GameObject.FindGameObjectWithTag("MainCamera");
        anim = GetComponent<Animator>();
        state = BooState.Appeared;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        nowTargetPos = transform.position;
        futureTargetPos=transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == BooState.Appeared)
        {
            transform.LookAt(player.transform);
        }
        else if (state == BooState.Moved)
        {
            if(!anim.GetBool("isCatch"))
            {
                BooRunAway();
            }
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

    private void BooRunAway()
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

        //anim.SetBool("AbleMove", true);

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
        futureTargetPos = player.transform.position + offset * radius;

        if(isRight)
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
    //Idleから動き出すトリガー
    public void OnAnimationTrigger()
    {
        if (GameManager.Instance.state != GameState.Tutorial)
        {
            state = BooState.Moved;
            anim.SetBool("AbleMove", true);
        }
    }
    //捕まった時アニメーション終了時の処理
    public void OnAnimationEnd()
    {
        if (GameManager.Instance.state == GameState.Game)
        {
            DifferenceManager.Instance.SetFixedPlank(Boos.Butler);
        }
        state = BooState.Catched;
        //transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }
}
//dontuse
/*private void CheckDirection()
{
    Vector3 direction = (transform.position-lastPos).normalized;
    lastPos = transform.position;

    float rightorleft = Vector3.Dot(transform.right, direction);

    //左に動いてる
    if (rightorleft > 0.1f)
    {
        anim.SetTrigger("MoveLeft");
        //Debug.Log("migi");
    }
    //右に動いてる
    else if (rightorleft < -0.1f)
    {
        anim.SetTrigger("MoveRight");
        //Debug.Log("hidari");
    }
}
*/

