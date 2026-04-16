using System.Collections;
using TMPro;
using UnityEngine;

public class AttentionManager : MonoBehaviour
{
    private enum AttentionState
    {
        Standby,
        Set,
        Finish,
    }
    private AttentionState state;

    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private DialogueData attentionData;
    [SerializeField] private DialogueData playerData;
    [SerializeField] private GameObject triggerModel;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject playerPos;
    [SerializeField] private GameObject cameraManager;
    [SerializeField] private GameObject sparklingEffect;
    [SerializeField] private Light dreamLight;
    [SerializeField] private GameObject fadeObj;
    //光動かすやつ
    private bool setTarget;
    private bool isRight;
    private float speed;
    private float radius;
    private GameObject startTrigger;
    Vector3 Direction;
    Vector3 nowTargetPos;
    Vector3 futureTargetPos;
    //マテリアル
    private float intensity;
    private float maxIntensity;
    private float lightRange;
    private Renderer myRenderer;
    private Material myMaterial;
    private Color myColor;
    //その他
    private bool ableMove;
    private bool fadeOut;

    private Animator playerAnimator;
    PlayerSubtitle playersubtitle;
    CameraManager cameramanager;
    Fade fadeScript;
    // Start is called before the first frame update
    void Start()
    {
        ChangeMyState(AttentionState.Standby);

        setTarget = false;
        isRight = false;
        startTrigger = null;
        speed = 0.3f;
        radius = 0.8f;
        Direction = Vector3.zero;
        nowTargetPos = playerPos.transform.position;
        futureTargetPos= playerPos.transform.position;

        intensity = 0f;
        maxIntensity = 2.0f;
        lightRange = 0f;
        myRenderer = GetComponent<Renderer>();
        myMaterial = myRenderer.material;
        myColor = myMaterial.color;

        ableMove = false;
        fadeOut = false;
        playerAnimator=playerModel.GetComponent<Animator>();
        playersubtitle = playerModel.GetComponent<PlayerSubtitle>();
        cameramanager = cameraManager.GetComponent<CameraManager>();
        fadeScript = fadeObj.GetComponent<Fade>();

        playerModel.SetActive(false);

        GameManager.Instance.OnStateChange += WhenChangedGameState;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.Attention)
        {
            //debugLog.text = "ugoki" + ableMove
                //+ "saigo" + fadeOut;
            if (startTrigger != null)
            {
                TriggerRunAway();
            }

            if(ableMove)
            {
                HandPos();
                //debugLog.text = "otete";
            }

            if(fadeOut)
            {
                if (fadeScript.State == FadeState.Dark)
                {
                    GameManager.Instance.ChangeState(GameState.Standby);
                    fadeOut = false;
                }
            }
        }
        //DEBUG//
        if (Input.GetKey(KeyCode.A))
        {
            ChangeMyState(AttentionState.Set);
        }
        //DEBUG//
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.state == GameState.Attention)
        {
            Direction = Vector3.Slerp(Direction, (nowTargetPos - startTrigger.transform.position).normalized, Time.deltaTime * 5f);
        }
    }
    private void ChangeMyState(AttentionState newState)
    {
        if (newState == state) return;

        switch(newState)
        {
            case AttentionState.Set:
                Destroy(startTrigger);
                StartCoroutine(SetRoutine());
                break;
        }
        state = newState;
    }
    private void SetPosition()
    {
        transform.position = playerPos.transform.position;
    }
    private void TriggerRunAway()
    {
        if (Vector3.Distance(playerPos.transform.position, startTrigger.transform.position) < 0.9f)
        {
            if (!setTarget)
            {
                SetTarget();
            }
            else
            {
                //ある程度近づいたら次のターゲット決める
                if (Vector3.Distance(startTrigger.transform.position, nowTargetPos) < 0.5f)
                {
                    setTarget = false;
                }
            }

            //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

            startTrigger.transform.position += Direction * speed * Time.deltaTime;

            if (Vector3.Distance(startTrigger.transform.position, nowTargetPos) < 0.1f)
            {
                nowTargetPos = futureTargetPos;
            }
        }
        else
        {
            startTrigger.transform.position = new Vector3(playerPos.transform.position.x, playerPos.transform.position.y, playerPos.transform.position.z + 0.5f);
        }
    }
    private void SetTarget()
    {
        Vector3 forward = playerPos.transform.forward;
        Vector3 right = playerPos.transform.right;

        float angle;
        if (isRight)
        {
            //ランダムで角度を決めてラジアン化
            angle = Random.Range(-30f, 0f) * Mathf.Deg2Rad;
        }
        else
        {
            angle = Random.Range(0f, 30f) * Mathf.Deg2Rad;
        }

        Vector3 offset = forward * Mathf.Cos(angle) + right * Mathf.Sin(angle);
        float yOffset = Random.Range(-0.1f, 0.1f);  // 上下の揺れ幅
        futureTargetPos = new Vector3((playerPos.transform.position.x + offset.x) * radius, playerPos.transform.position.y + yOffset, (playerPos.transform.position.x + offset.z) * radius);

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
    private void WhenChangedGameState(GameState newState)
    {
        //最初の字幕
        if (newState == GameState.Attention)
        {
            startTrigger = Instantiate(triggerModel, new Vector3(0f, playerPos.transform.position.y, 0.5f), Quaternion.identity);
            Subtitle.Instance.InstanceAttention(attentionData, 0, 0, 0f, false);
        }
        //暗くなったら消す
        if (newState == GameState.Standby)
        {
            gameObject.SetActive(false);
        }
    }
    private void HandPos()
    {
        Vector3 headpos = playerPos.transform.position;
        Vector3 lefthandpos = HandTraking.Instance.GetLeftHandPos();
        Vector3 righthandpos = HandTraking.Instance.GetRightHandPos();

        if (headpos.y <= lefthandpos.y)
        {
            playerAnimator.SetBool("Right", true);
        }
        else
        {
            playerAnimator.SetBool("Right", false);
        }

        if (headpos.y <= righthandpos.y)
        {
            playerAnimator.SetBool("Left", true);
        }
        else
        {
            playerAnimator.SetBool("Left", false);
        }
    }
    private IEnumerator SetRoutine()
    {
        Subtitle.Instance.StopAttention();
        yield return new WaitForSeconds(2f);

        Instantiate(sparklingEffect, playerModel.transform.position, Quaternion.identity);
        playerModel.SetActive(true);
        yield return null;

        playersubtitle.InstancePlayerSubTitle(playerData, 0, 1);
        yield return playersubtitle.nowPlayerSubtitleCoroutine;

        playersubtitle.InstancePlayerSubTitle(playerData, 2, 5);
        ableMove = true;
        yield return playersubtitle.nowPlayerSubtitleCoroutine;

        playerAnimator.SetTrigger("WaveHand");

        fadeOut = true;
        fadeScript.StartFade(1f);
    }
    public void StateChangeSet()
    {
        ChangeMyState(AttentionState.Set);
    }

    /*private void SetEmission()
   {
       if (intensity <= maxIntensity)
       {
           intensity += Time.deltaTime;
           lightRange += 5f * Time.deltaTime;

           Color frash = myColor * intensity;
           myMaterial.SetColor("_EmissionColor", frash);
           dreamLight.range = lightRange;
       }
       else
       {
           GameManager.Instance.ChangeState(GameState.Standby);
           gameObject.SetActive(false);
       }
   }*/
}
