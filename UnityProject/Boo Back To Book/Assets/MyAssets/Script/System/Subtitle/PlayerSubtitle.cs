using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSubtitle : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;//centereye
    [SerializeField] private GameObject playerSubTitle;

    public Coroutine nowPlayerSubtitleCoroutine { get; private set; }

    //カメラからの距離
    private float playerSubtitleDistance;
    private float duration;
    private float followSpeed;
    private bool startPlayerSubtitleRoutine;
    private bool isStopPlayerSubtitleRequest;
    private Coroutine playerLoopCoroutine;
    private TextMeshPro playerSubtitleText;
    private Animator myAnimator;

    PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        playerSubtitleDistance = 0.8f;
        duration = 4.0f;
        followSpeed = 3.0f;
        startPlayerSubtitleRoutine = false;
        isStopPlayerSubtitleRequest = false;

        playerSubtitleText = playerSubTitle.GetComponent<TextMeshPro>();
        myAnimator = GetComponent<Animator>();
        playsound = GetComponent<PlaySound>();
    }

    // Update is called once per frame
    void Update()
    {
        SubtitleControl();
    }

    public void InstancePlayerSubTitle(DialogueData dialoguedata, int start, int end)
    {
        StopSubTitle();

        isStopPlayerSubtitleRequest = false;
        nowPlayerSubtitleCoroutine = StartCoroutine(SubTitleRoutine(dialoguedata, start, end));
    }
    public void StopSubTitle()
    {
        isStopPlayerSubtitleRequest = true;

        if (nowPlayerSubtitleCoroutine != null)
        {
            StopCoroutine(nowPlayerSubtitleCoroutine);
            nowPlayerSubtitleCoroutine = null;
        }
        if (playerLoopCoroutine != null)
        {
            StopCoroutine(playerLoopCoroutine);
            playerLoopCoroutine = null;
        }

        playsound.Stop();
        playerSubtitleText.text = "";
        startPlayerSubtitleRoutine = false;
    }
    public IEnumerator SubTitleRoutine(DialogueData dialoguedata, int start, int end)
    {
        startPlayerSubtitleRoutine = true;

        for (int i = start; i <= end; i++)
        {
            if (isStopPlayerSubtitleRequest) break;

            var line = dialoguedata.lines[i];
            /*if (line == null)
            {
                Debug.Log("空");
            }*/

            playerSubtitleText.text = line.SubTitleText;
            //bookAnimator.SetTrigger("Talk");


            // 音声
            if (line.Clip != null)
            {
                playsound.PlayOneShot(line.Clip);
            }

            float t = 0f;
            while (t < duration)
            {
                if (isStopPlayerSubtitleRequest) break;

                t += Time.deltaTime;
                yield return null;
            }

            if (isStopPlayerSubtitleRequest) break;

            //bookAnimator.SetTrigger("Idle");
            // playerSubTitle.gameObject.SetActive(false);
            playerSubtitleText.text = "";
        }

        startPlayerSubtitleRoutine = false;
        //bookAnimator.SetTrigger("Idle");
        playerSubtitleText.text = "";
        nowPlayerSubtitleCoroutine = null;
        //Debug.Log("fadeOut");
    }
    public IEnumerator SubTitleLoopRoutine(DialogueData dialoguedata, int start, int end)
    {
        while (!isStopPlayerSubtitleRequest)
        {
            nowPlayerSubtitleCoroutine = StartCoroutine(SubTitleRoutine(dialoguedata, start, end));
            yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

            yield return new WaitForSeconds(5f);
        }
    }
    private void SubtitleControl()
    {
        //カメラの中心とオブジェクトの中心が常に一緒
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * playerSubtitleDistance;
        targetPos.y = targetPos.y - 0.4f;
        playerSubTitle.transform.position = Vector3.Lerp(playerSubTitle.transform.position, targetPos, Time.deltaTime * followSpeed);

        Quaternion targetRot = Quaternion.LookRotation(playerSubTitle.transform.position - cameraTransform.position);
        playerSubTitle.transform.rotation = Quaternion.Slerp(playerSubTitle.transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }
}
