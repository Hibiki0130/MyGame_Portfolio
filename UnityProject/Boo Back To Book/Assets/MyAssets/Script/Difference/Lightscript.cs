using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lightscript : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugLog;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject TutorialMane;

    Animator anim;
    bool finish;

    TutorialManager tutorialmanager;
    // Start is called before the first frame update
    void Start()
    {
        finish = false;
        tutorialmanager = TutorialMane.GetComponent<TutorialManager>();

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(subtitleDistance);

        /*if (Input.GetKey(KeyCode.L))
        {
            tutorialmanager.ChangeState(TutorialState.Door);
        }

        if (!fadeOut)
        {
            float subtitleDistance = Vector3.Distance(transform.position, playerPos.transform.position);
            //debugLog.text = "$" + subtitleDistance;

            if (subtitleDistance <= 1.7f)
            {
                tutorialmanager.ChangeState(TutorialState.Door);

                fadeOut = true;
            }
        }*/
    }
    private void LightController()
    {
         
    }
    int lightnum = 4;
    int prenum;
    private void LightAnimation()
    {
        int num;
        while(true)
        {
            num = Random.Range(0, lightnum);

            if(num!=prenum)
            {
                prenum = num;

                break;
            }
        }

        switch (num)
        {
            case 0:
                anim.SetTrigger("Zero");
                break;
            case 1:
                anim.SetTrigger("One");
                break;
            case 2:
                anim.SetTrigger("Two");
                break;
            default:
                break;
        }
    }

    public void OnAnimationEnd()
    {
        LightAnimation();
    }
}
