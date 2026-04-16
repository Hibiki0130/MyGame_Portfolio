using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField] private AudioClip waltz;
    private Animator anim;

    PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playsound = GetComponent<PlaySound>();

        GameManager.Instance.OnStateChange += PlayOrStop;
        TutorialManager.Instance.OnTutorialStateChange += WhenChangeTutorialState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AnimationContollorer()
    {

    }

    private void PlayOrStop(GameState newstate)
    {
        if (newstate == GameState.Tutorial)
        {
            anim.SetBool("AbleTurn",true);
            playsound.Play(waltz);
        }
        else if (newstate == GameState.Game)
        {
            anim.SetBool("AbleTurn", false);
            playsound.Stop();
        }
    }

    private void WhenChangeTutorialState(TutorialState newState)
    {
        if (newState == TutorialState.Boo)
        {
            anim.SetBool("AbleTurn", false);
            playsound.Stop();
        }
        else if (newState == TutorialState.finish)
        {
            anim.SetBool("AbleTurn", true);
            playsound.Play(waltz);
        }
    }
}
