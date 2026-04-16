using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeMaskBoo : MonoBehaviour
{
    [SerializeField] private DialogueData attentionData;
    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private AudioClip laughVoice;

    private GameObject player;
    private Animator myAnimator;
    private float distance;
    private float followSpeed;
    Vector3 instancePos;
    PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera");
        myAnimator = GetComponent<Animator>();
        playsound = GetComponent<PlaySound>();

        distance = 1.5f;
        followSpeed = 5f;

        playsound.PlayOneShot(laughVoice);
        Instantiate(smokeEffect, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        UIControl();
        //現在のちょうどいい距離：0.684
        //float subtitleDistance = Vector3.Distance(transform.position, playerPos.transform.position);
        //Debug.Log(subtitleDistance);
    }

    private void UIControl()
    {
        //カメラの中心とオブジェクトの中心が常に一緒
        Vector3 targetPos = player.transform.position + player.transform.forward * distance;
        instancePos = targetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        Quaternion targetRot = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }
    public void StartEyeMask()
    {
        DifferenceManager.Instance.Light = false;
        DifferenceManager.Instance.SetDiffrence = true;
        Subtitle.Instance.InstanceAttention(attentionData, 0, 0, 0f, true);
    }
    public void FinishEyeMask()
    {
        myAnimator.SetTrigger("Disappear");
    }
    public void DestroyBoo()
    {
        DifferenceManager.Instance.Light = true;
        Subtitle.Instance.StopAttention(); 
        Instantiate(smokeEffect, instancePos, Quaternion.identity);
        Destroy(gameObject);
    }
}
