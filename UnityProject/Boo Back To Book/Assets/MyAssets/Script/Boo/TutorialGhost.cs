using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static TutorialManager;


public class TutorialGhost : MonoBehaviour
{
    private enum GhostState
    {
        Standby,
        Appeared,
        Moved,
        Disappeared,
    }
    private GhostState state;

    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private GameObject animationBook;
    [SerializeField] private GameObject bookTarget;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject smokeEffect;

    [Header("SE")]
    [SerializeField] private AudioClip booLaught; 

    private GameObject target;
    private Animator booAnim;

    private float speed;
    private bool changeState;

    PlaySound playsound;
    BookController bookcontroller;
    // Start is called before the first frame update
    void Start()
    {
        state = GhostState.Standby;

        changeState = true;

        speed = 1.0f;

        transform.localScale = Vector3.zero;

        playsound = GetComponent<PlaySound>();
        booAnim = GetComponent<Animator>();
        bookcontroller = animationBook.GetComponent<BookController>();

        target = door;
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(state);
        BooMoveAnimation();
    }
    //


    private void BooMoveAnimation()
    {
        if (state == GhostState.Standby)
        {
            transform.localScale = Vector3.zero;
        }

        //ギミックを解いたら
        if (TutorialManager.Instance.isSolved)
        {
            booAnim.SetTrigger("Boo");
            state = GhostState.Appeared;

            playsound.PlayOneShot(booLaught);
            transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            TutorialManager.Instance.SetIsSolvedBool(false);
            //Debug.Log("BOO");
        }
        //解いた後
        switch (state)
        {
            case GhostState.Appeared:
                transform.LookAt(player.transform);
                break;
            case GhostState.Moved:
                transform.LookAt(target.transform);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

                //ターゲットの下に着いたら
                if (transform.position == target.transform.position)
                {
                    booAnim.SetTrigger("Disappear");
                }
                break;
            case GhostState.Disappeared:
                if (changeState)
                {
                    StartCoroutine(TutorialGhostSubTitle());
                    changeState = false;
                }
                break;
            default:
                break;
        }
    }
    private IEnumerator TutorialGhostSubTitle()
    {
        StartCoroutine(bookcontroller.BookAppearanceRoutine(animationBook, false));

        Subtitle.Instance.InstanceSubTitle(dialogueData, 9, 10);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        StartCoroutine(bookcontroller.BookAppearanceRoutine(bookTarget, true));

        TutorialManager.Instance.ChangeState(TutorialState.Door);
    }
    //移動するタイミング
    public void OnAnimationTrigger()
    {
        state = GhostState.Moved;
        booAnim.SetTrigger("MoveRight"); 
    }
    //おばけが消えるトリガー
    public void DestroyBoo()
    {
        Instantiate(smokeEffect, transform.position, Quaternion.identity);
        transform.localScale = Vector3.zero;
        state = GhostState.Disappeared;    
    }

    //dontuse//
    /*if (newState != TutorialState.fadeOut)
        {
            //Debug.Log("instancedBoo");
            playsound.PlayOneShot(booLaught);
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //ableMove = true;

            if (!telephoneDone && newState == TutorialState.Light)
            {
                Debug.Log("Light");
                booAnim.SetTrigger("instancedBoo");
                target = lightPos;
                telephoneDone = true;
            }
            else if (!lightDone && newState == TutorialState.Door)
            {
                Debug.Log("Door");
                booAnim.SetTrigger("instancedBoo");
                target = door;
                lightDone = true;
            }
            else if (!doorDone && newState == TutorialState.Boo)
            {
                Debug.Log("ghost");
                booAnim.SetTrigger("instancedBoo");
                fadeOut = true;
                doorDone = true;
                //tutorialmanager.OnTutorialStateChange -= BooContllorer;
            }
        }
        else
        {
            //Destroy(gameObject);
        }*/
}
