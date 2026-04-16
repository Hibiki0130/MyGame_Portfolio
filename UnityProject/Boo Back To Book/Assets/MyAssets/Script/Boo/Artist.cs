using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artist: MonoBehaviour
{
    [SerializeField] private GameObject[] mobsPrefub;
    [SerializeField] private AudioClip artistBGM;

    private int mobsCatchCount;
    private bool ableCoroutine;
    private bool[] mobsCaught;
    private float speed;
    private Vector3 myPos;
    private Vector3 target;
    private BoxCollider bC;
    private Animator myAnimator;
    private GameObject[] mobs;
    ArtistAnimation artistanimation;
    Mob[] mobscripts;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(artistBGM);

        myAnimator = GetComponentInChildren<Animator>();
        bC = GetComponentInChildren<BoxCollider>();
        artistanimation= GetComponentInChildren<ArtistAnimation>();
        ableCoroutine = true;

        //èâä˙âª
        mobs = new GameObject[mobsPrefub.Length];
        mobscripts = new Mob[mobsPrefub.Length];
        mobsCaught = new bool[mobsPrefub.Length];
        for (int i = 0; i < mobsPrefub.Length; i++)
        {
            mobs[i] = Instantiate(mobsPrefub[i], transform.position, transform.rotation);
            mobscripts[i] = mobs[i].GetComponent<Mob>();
            mobsCaught[i] = false;
        }

        bC.enabled = false;
        speed = 1.0f;
        myPos = transform.position;
        target = myPos + new Vector3(0f, 2f, 0f);
        transform.localScale = new Vector3(2f, 2f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        //ÇæÇÒÇæÇÒâ∫Ç™ÇÈ
        if(artistanimation.AbleMove)
        {
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
        }

        if (DifferenceManager.Instance.state == DifferenceState.Finish)
        {
            Destroy(gameObject);
        }

        if(ableCoroutine)
        {
            StartCoroutine(MyAnimation());
        }
    }
    private IEnumerator MyAnimation()
    {
        ableCoroutine = false;

        CheckMobs();

        switch (mobsCatchCount)
        {
            case 0:
                target = myPos + new Vector3(0f, 2f, 0f);
                break;
            case 1:
                target = myPos + new Vector3(0f, 1.5f, 0f);
                break;
            case 2:
                target = myPos + new Vector3(0f, 1f, 0f);
                break;
            case 3:
                target = myPos;
                myAnimator.SetBool("AllMobCaught", true);
                bC.enabled = true;
                break;
            default:
                break;
        }

        yield return null;

        ableCoroutine = true;
    }
    private void CheckMobs()
    {
        mobsCatchCount = 0;

        for (int i = 0; i < mobsPrefub.Length; i++)
        {
            if(mobscripts[i].caught)
            {
                if (!mobsCaught[i])
                {
                    myAnimator.SetTrigger("MobCaught");
                    mobsCaught[i] = true;
                }
                mobsCatchCount++;
            }
        }
    }
}
