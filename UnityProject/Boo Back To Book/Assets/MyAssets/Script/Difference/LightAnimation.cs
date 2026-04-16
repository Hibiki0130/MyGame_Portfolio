using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimation : MonoBehaviour
{
    Animator anim;

    private bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("On", isOn);
    }

    int lightnum = 4;
    int prenum;
    private void BlinkAnimation()
    {
        int num;
        while (true)
        {
            num = Random.Range(0, lightnum);

            if (num != prenum)
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

    public void SetisOnBool(bool b)
    {
        isOn = b;
    }
    public void OnAnimationEnd()
    {
        BlinkAnimation();
    }
}
