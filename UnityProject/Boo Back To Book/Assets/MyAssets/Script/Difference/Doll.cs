using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Doll : MonoBehaviour
{
    private enum DollState
    {
        NotPranked,
        Follow,
        Move,
        Boo,
        Restored,
    }
    private DollState state;

    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject nomalDoll;
    [SerializeField] private GameObject followDoll;
    [SerializeField] private GameObject moveDoll;
    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private GameObject twinsBoo;
    [SerializeField] private AudioClip dollStanp;

    private float stopDistance;
    private bool isLift;
    private GameObject prankDoll;
    private Vector3 playerPos;
    private Vector3 dollPos;
    private Vector3 returnPos;
    private NavMeshAgent agent;
    private PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        state = DollState.NotPranked;

        stopDistance = 1.5f;
        isLift = false;
        prankDoll = null;
        dollPos = nomalDoll.transform.position;
        //camera = Camera.main;

        agent = followDoll.GetComponent<NavMeshAgent>();

        DifferenceManager.Instance.OnDiffrenceStateChange += CheckChosen;

        nomalDoll.SetActive(true);
        followDoll.SetActive(false);
        moveDoll.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.Game)
        {
            if (state == DollState.Follow)
            {
                FollowDollController();
            }

            if (DifferenceManager.Instance.state == DifferenceState.Serch)
            {
                PrankDollControl();
            }
        }

        if (DifferenceManager.Instance.state == DifferenceState.Finish)
        {
            ChangeMyState(DollState.Restored);
        }
        //Debug.Log(state);
    }
    private void CheckChosen(DifferenceState newState)
    {
        if (newState == DifferenceState.Twins)
        {
            //選ばれていたらいたずら
            if (DifferenceManager.Instance.GetChosenPlanked(Boos.Twins) == 0)
            {
                prankDoll = followDoll;
                ChangeMyState(DollState.Follow);
            }
            if(DifferenceManager.Instance.GetChosenPlanked(Boos.Twins) == 1)
            {
                prankDoll = moveDoll;
                ChangeMyState(DollState.Move);
            }
            returnPos = prankDoll.transform.position;
            playsound = prankDoll.GetComponent<PlaySound>();
            playsound.PlayOneShot(dollStanp);
        }
    }
    private void ChangeMyState(DollState newState)
    {
        //変わった時一回だけ処理
        if (state == newState) return;
        state = newState;
        switch (newState)
        {
            case DollState.NotPranked:
                break;
            case DollState.Follow:
                nomalDoll.SetActive(false);
                followDoll.SetActive(true);
                break;
            case DollState.Move:
                nomalDoll.SetActive(false);
                moveDoll.SetActive(true);
                break;
            case DollState.Boo:
                DifferenceManager.Instance.ChangeState(DifferenceState.Catch);
                break;
            case DollState.Restored:
                nomalDoll.SetActive(true);
                followDoll.SetActive(false);
                moveDoll.SetActive(false);
                break;
            default:
                break;
        }
    }
    private void PrankDollControl()
    {
        //0.3
        //debugLog.text = "" + Vector3.Distance(prankDoll.transform.position, HandTraking.Instance.GetLeftHandPos());
        float leftToDoll = Vector3.Distance(prankDoll.transform.position, HandTraking.Instance.GetLeftHandPos());
        float rightToDoll= Vector3.Distance(prankDoll.transform.position, HandTraking.Instance.GetRightHandPos());
        float leftToRight = Vector3.Distance(HandTraking.Instance.GetRightHandPos(), HandTraking.Instance.GetLeftHandPos());

        //人形とどちらかの手が近い
        if (leftToDoll <= 0.3f || rightToDoll <= 0.3f)
        {
            //手同士も近い
            if (leftToRight < 0.4f)
            {
                //debugLog.text = "ableHand";
                isLift = true;
                agent.enabled = false;
                LookAtPlayer();
                prankDoll.transform.position = (HandTraking.Instance.GetRightHandPos() + HandTraking.Instance.GetLeftHandPos()) / 2;
            }
        }
        else
        {
            //落とした
            if(isLift)
            {
                prankDoll.transform.position = returnPos;
                isLift = false;
            }
        }

        //正解
        if (Vector3.Distance(prankDoll.transform.position, dollPos) < 0.2f)
        {
            StartCoroutine(BooRoutine());
        }
    }
    private IEnumerator BooRoutine()
    {
        ChangeMyState(DollState.Boo);
        yield return null;

        Instantiate(smokeEffect, prankDoll.transform.position, Quaternion.identity);
        Instantiate(twinsBoo, new Vector3(dollPos.x, 1.5f, dollPos.z), prankDoll.transform.rotation);

        prankDoll.SetActive(false);
    }

    private void FollowDollController()
    {
        playerPos = new Vector3(camera.transform.position.x, 0f, camera.transform.position.z);

        if (!IsVisibleFromCamera())
        {
            //プレイヤーをずっと見る
            LookAtPlayer();

            if (Vector3.Distance(followDoll.transform.position, playerPos) <= stopDistance)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(camera.transform.position);
            }
        }
        else
        {
            agent.isStopped = true;   
        }
    }
    private bool IsVisibleFromCamera()
    {
        Vector3 viewportpos = camera.WorldToViewportPoint(transform.position);

        bool isVisible =
             viewportpos.z > 0 &&
            viewportpos.x > 0 && viewportpos.x < 1 &&
            viewportpos.y > 0 && viewportpos.y < 1;

        return isVisible;
    }
    private void LookAtPlayer()
    {
        Vector3 target = camera.transform.position;
        Vector3 mypos = followDoll.transform.position;

        target.y = mypos.y;

        followDoll.transform.LookAt(target);
    }
}
