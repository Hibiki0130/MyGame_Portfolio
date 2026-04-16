using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Subtitle : MonoBehaviour
{
    public static Subtitle Instance { get; private set; }

    [SerializeField] private Transform cameraTransform;//centereye
    [SerializeField] private GameObject subTitle;
    [SerializeField] private GameObject attentionBrack;
    [SerializeField] private GameObject attentionRed;
    [SerializeField] private GameObject book;

    public Coroutine nowSubtitleCoroutine { get; private set; }
    public Coroutine nowAttentionCoroutine { get; private set; }

    //カメラからの距離
    private float subtitleDistance;
    private float attentionDistance;
    //字幕が出て消えるときの間隔
    private float duration;
    private float time;
    private bool startSubtitleRoutine;
    private bool startAttentionRoutine;
    private bool isStopSubtitleRequest;
    private bool isStopAttentionRequest;
    private bool animationTrigger;
    private Coroutine loopCoroutine;
    private TextMeshPro subtitleText;
    private TextMeshPro attentionBrackText;
    private TextMeshPro attentionRedText;
    private Animator bookAnimator;

    PlaySound playsound;
    BookController bookcontroller;
    private void Awake()
    {
        //シングルトン初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //3D
        subtitleDistance = 1f;
        attentionDistance = 1f;
        //subtitleDistance = 0.374f;
        duration = 4.0f;
        time = 10f;

        startSubtitleRoutine = false;
        startAttentionRoutine = false;
        isStopSubtitleRequest = false;
        isStopAttentionRequest = false;

        subtitleText = subTitle.GetComponent<TextMeshPro>();
        attentionBrackText = attentionBrack.GetComponent<TextMeshPro>();
        attentionRedText = attentionRed.GetComponent<TextMeshPro>();
        playsound = book.GetComponentInChildren<PlaySound>();
        bookcontroller = book.GetComponent<BookController>();
        bookAnimator = book.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(subtitleDistance);
        SubtitleControl();
        AttentionControl();

        if (GameManager.Instance.state == GameState.OP)
        {
            //playerSubTitle.SetActive(true);
        }
    }

    public void InstanceSubTitle(DialogueData dialoguedata, int start, int end)
    {
        StopSubTitle();

        isStopSubtitleRequest = false;
        nowSubtitleCoroutine = StartCoroutine(SubTitleRoutine(dialoguedata, start, end));
    }
    public void InstanceLoopSubTitle(DialogueData dialoguedata, int start, int end)
    {
        StopSubTitle();

        isStopSubtitleRequest = false;
        loopCoroutine = StartCoroutine(SubTitleLoopRoutine(dialoguedata, start, end));
    }
    public void StopSubTitle()
    {
        isStopSubtitleRequest = true;

        if (nowSubtitleCoroutine != null)
        {
            StopCoroutine(nowSubtitleCoroutine);
            nowSubtitleCoroutine = null;
        }
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }

        playsound.Stop();
        subtitleText.text = "";
        startSubtitleRoutine = false;
    }
    public void InstanceAttention(DialogueData dialoguedata, int start, int end, float attentionduration,bool color)
    {
        StopAttention();

        isStopAttentionRequest = false;
        nowAttentionCoroutine = StartCoroutine(AttentionRoutine(dialoguedata, start, end, attentionduration, color));
    }
    public void StopAttention()
    {
        isStopAttentionRequest = true;

        if (nowAttentionCoroutine != null)
        {
            StopCoroutine(nowAttentionCoroutine);
            nowSubtitleCoroutine = null;
        }

        attentionRedText.text = "";
        attentionBrackText.text = "";
        startAttentionRoutine = false;
        //Debug.Log("stop");
    }
    public bool GetStartRoutineBool()
    {
        return startSubtitleRoutine;
    }

    public IEnumerator SubTitleRoutine(DialogueData dialoguedata, int start, int end)
    {
        //本が現れてから
        while(true)
        {
            if (isStopSubtitleRequest) break;
            if (bookcontroller.ableTalk) break;

            yield return null;
            Debug.Log("ループ字幕止まってるよ");
        }

        startSubtitleRoutine = true;

        for (int i = start; i <= end; i++)
        {
            if (isStopSubtitleRequest) break;

            var line = dialoguedata.lines[i];
            /*if (line == null)
            {
                Debug.Log("空");
            }*/

            subtitleText.text = line.SubTitleText;
            bookAnimator.SetTrigger("Talk");
  

            // 音声
            if (line.Clip != null)
            {
                playsound.PlayOneShot(line.Clip);
            }

            float t = 0f;
            while(t<duration)
            {
                if (isStopSubtitleRequest) break;

                t += Time.deltaTime;
                yield return null;
            }

            if (isStopSubtitleRequest) break;

            bookAnimator.SetTrigger("Idle");
           // playerSubTitle.gameObject.SetActive(false);
            subtitleText.text = "";
        }

        startSubtitleRoutine = false;
        bookAnimator.SetTrigger("Idle");
        subtitleText.text = "";
        nowSubtitleCoroutine = null;
        //Debug.Log("fadeOut");
    }
    public IEnumerator SubTitleLoopRoutine(DialogueData dialoguedata, int start, int end)
    {
        while (!isStopSubtitleRequest)
        {
            nowSubtitleCoroutine = StartCoroutine(SubTitleRoutine(dialoguedata, start, end));
            yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

            yield return new WaitForSeconds(5f);
        }
    }

    public IEnumerator AttentionRoutine(DialogueData dialoguedata, int start, int end, float attentionduration, bool color)
    {
        startAttentionRoutine = true;

        for (int i = start; i <= end; i++)
        {
            if (isStopAttentionRequest) break;

            var line = dialoguedata.lines[i];

            if (color)
            {
                attentionRedText.text = line.SubTitleText;
            }
            else
            {
                attentionBrackText.text= line.SubTitleText;
            }

            // 音声
            if (line.Clip != null)
            {
                playsound.PlayOneShot(line.Clip);
            }

            //○秒表示
            if (attentionduration != 0f)
            {
                float t = 0f;
                while (t < attentionduration)
                {
                    if (isStopAttentionRequest) break;

                    t += Time.deltaTime;
                    yield return null;
                }
            }
            //ストップかかるまでずっと
            else
            {
                while (!isStopAttentionRequest)
                {
                    yield return null;
                }
            }

            if (isStopAttentionRequest) break;


            attentionRedText.text = "";
            attentionBrackText.text = "";
        }

        startAttentionRoutine = false;
        attentionRedText.text = "";
        attentionBrackText.text = "";
        nowAttentionCoroutine = null;
        //Debug.Log("fadeOut");
    }

    private void SetSubTitleWhenChangeTutorialState(TutorialState newState)
    {

    }

    private float followSpeed = 3.0f;
    private void SubtitleControl()
    {
        //カメラの中心とオブジェクトの中心が常に一緒
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * subtitleDistance;
        targetPos.y = targetPos.y - 0.4f;
        subTitle.transform.position = Vector3.Lerp(subTitle.transform.position, targetPos, Time.deltaTime * followSpeed);
        attentionBrack.transform.position = Vector3.Lerp(attentionBrack.transform.position, targetPos, Time.deltaTime * followSpeed);

        Quaternion targetRot = Quaternion.LookRotation(subTitle.transform.position - cameraTransform.position);
        subTitle.transform.rotation = Quaternion.Slerp(subTitle.transform.rotation, targetRot, Time.deltaTime * followSpeed);
        attentionBrack.transform.rotation = Quaternion.Slerp(attentionBrack.transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }
    private void AttentionControl()
    {
        //カメラの中心とオブジェクトの中心が常に一緒
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * attentionDistance;
        attentionRed.transform.position = Vector3.Lerp(attentionRed.transform.position, targetPos, Time.deltaTime * followSpeed);

        Quaternion targetRot = Quaternion.LookRotation(attentionRed.transform.position - cameraTransform.position);
        attentionRed.transform.rotation = Quaternion.Slerp(attentionRed.transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }
}
