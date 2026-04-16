using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class RKGKManager : MonoBehaviour
{
    private enum RKGKState
    {
        NotPranked,
        Pranked,
        Boo,
        Restored,
    }
    private RKGKState state;

    [SerializeField] private GameObject pictureRkgk;
    [SerializeField] private GameObject wallRkgk;
    [SerializeField] private GameObject artistBoo;
    [SerializeField] private GameObject cleanEffect;
    [SerializeField] private GameObject[] instanceBooPoints;
    [Header("SE")]
    [SerializeField] private AudioClip plankSound;
    [SerializeField] private AudioClip kirakira;

    private float cleanNum;
    private bool[] cleanSound;
    private GameObject chosenRkgk;
    private GameObject instanceBooPos;
    private GameObject[] chosenRkgks;
    private 
    PlaySound playsound;
    Rkgk[] chosenRkgkScript;
    // Start is called before the first frame update
    void Start()
    {
        ChangeMyState(RKGKState.NotPranked);
        DifferenceManager.Instance.OnDiffrenceStateChange += CheckChosen;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == RKGKState.Pranked)
        {
            if (DifferenceManager.Instance.state == DifferenceState.Serch)
            {
                CheckRKGK();
            }
        }
        if(state == RKGKState.Boo)
        {
            if(DifferenceManager.Instance.FixedPlank[Boos.Artist])
            {
                ChangeMyState(RKGKState.Restored);
            }
        }

        if (DifferenceManager.Instance.state == DifferenceState.Finish)
        {
            ChangeMyState(RKGKState.Restored);
        }
    }
    private void ChangeMyState(RKGKState newState)
    {
        if (state == newState) return;
        switch (newState)
        {
            case RKGKState.NotPranked:
                break;
            case RKGKState.Pranked:
                break;
            case RKGKState.Boo:
                DifferenceManager.Instance.ChangeState(DifferenceState.Catch);
                Instantiate(artistBoo, instanceBooPos.transform.position, instanceBooPos.transform.rotation);
                break;
            case RKGKState.Restored:
                pictureRkgk.SetActive(false);
                wallRkgk.SetActive(false);
                break;
            default:
                break;
        }
        state = newState;
    }

    private void CheckChosen(DifferenceState newState)
    {
        if (newState == DifferenceState.Artist)
        {
            ChangeMyState(RKGKState.Pranked);
            //ëIÇŒÇÍÇƒÇ¢ÇΩÇÁÇ¢ÇΩÇ∏ÇÁ
            if (DifferenceManager.Instance.GetChosenPlanked(Boos.Artist) == 0)
            {
                chosenRkgk = pictureRkgk;
            }
            else if(DifferenceManager.Instance.GetChosenPlanked(Boos.Artist) == 1)
            {
                chosenRkgk = wallRkgk;
            }

            instanceBooPos = instanceBooPoints[DifferenceManager.Instance.GetChosenPlanked(Boos.Artist)];
            //èâä˙âª
            int count = chosenRkgk.transform.childCount;
            cleanSound = new bool[count];
            chosenRkgks = new GameObject[count];
            chosenRkgkScript = new Rkgk[count];
            for (int i = 0; i < count; i++)
            {
                chosenRkgks[i] = chosenRkgk.transform.GetChild(i).gameObject;
                chosenRkgkScript[i] = chosenRkgks[i].GetComponent<Rkgk>();
                cleanSound[i] = true;
            }
            chosenRkgk.SetActive(true);

            playsound = chosenRkgk.GetComponent<PlaySound>();
            playsound.PlayOneShot(plankSound);
        }
    }
    private void CheckRKGK()
    {
        cleanNum = 0;

        for (int i = 0; i < chosenRkgks.Length; i++)
        {
            if (chosenRkgkScript[i].Clear)
            {
                if (cleanSound[i])
                {
                    playsound.PlayOneShot(kirakira);
                    Instantiate(cleanEffect, chosenRkgk.transform.position, Quaternion.identity);
                    cleanSound[i] = false;
                }
                cleanNum++;
            }
        }

        if (cleanNum == chosenRkgks.Length)
        {
            ChangeMyState(RKGKState.Boo);
        }
    }
}
