using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRPlugin;

public class OPBooAnimation : MonoBehaviour
{
    private enum BooState
    {
        None,
        Appearance,
        Arrive,
        RunAway,
    }
    private BooState booState;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject boo;
    [SerializeField] private GameObject secondTarget;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject sneezeEffect;
    [Header("SE")]

    private GameObject instancedBoo;
    private Transform target;
    private Vector3 targetScale;
    private Animator booAnimator;
    private bool animationTrigger;
    private bool runAway;
    private float speed;
    private float time;
    private float duration;

    PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        booState = BooState.None;

        target = transform;
        targetScale = new Vector3(1.5f, 1.5f, 1.5f);
        animationTrigger = true;
        runAway = false;
        time = 0f;
        duration = 0.5f;
        speed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.OP)
        {
            if (booState != BooState.None)
            {
                if (booState == BooState.Arrive)
                {
                    if (booAnimator == null)
                    {
                        booAnimator = instancedBoo.GetComponent<Animator>();
                    }
                    if (playsound == null)
                    {
                        playsound = instancedBoo.GetComponent<PlaySound>();
                    }
                }

                ChangeTarget();
                BooAnimation();

                if (booState == BooState.RunAway)
                {
                    Destroy(instancedBoo);
                    booState = BooState.None;
                }
            }
        }
    }

    private void ChangeTarget()
    {
        if (runAway)
        {
            target.position = secondTarget.transform.position;
            booAnimator.SetTrigger("RunAway");
        }
    }

    private void BooAnimation()
    {
        time += Time.deltaTime;
        float scalespeed = time / duration;
        instancedBoo.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, scalespeed);

        if(!runAway)
        {
            instancedBoo.transform.LookAt(player.transform);

            if (instancedBoo.transform.position == target.position)
            {
                booState = BooState.Arrive;
            }
        }
        else
        {
            instancedBoo.transform.LookAt(secondTarget.transform);

            if (booState == BooState.Arrive)
            {
                if (instancedBoo.transform.position == target.position)
                {
                    Instantiate(smoke, instancedBoo.transform.position, Quaternion.identity);
                    booState = BooState.RunAway;
                }
            }
        }

        instancedBoo.transform.position = Vector3.MoveTowards(instancedBoo.transform.position, target.transform.position, speed * Time.deltaTime);
    }

    public void InstanceBoo()
    {
        Instantiate(sneezeEffect, book.transform.position, Quaternion.identity);
        instancedBoo = Instantiate(boo, book.transform.position, Quaternion.identity);
        booState = BooState.Appearance;
    }
    public void SetRunAwayBool()
    {
        runAway = true;
    }
}
