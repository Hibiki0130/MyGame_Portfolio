using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PositionNumber
{
    Start = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Goal = 4,
    Finish = 5,
}
public class LookAround : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private DialogueData dialogueGameData;
    [SerializeField] private GameObject[] inductionSpawnPos;
    [SerializeField] private GameObject inductionPrefub;
    [SerializeField] private GameObject timer;    
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject eyeMaskBoo;
    [SerializeField] private GameObject smokeEffect;

    public PositionNumber PosNum { get; private set; }

    private GameObject instanceInduction;
    private Animator bookAnimator;
    private bool ableAdvance;

    Induction induction;
    BookController bookController;
    Door doorscript;
    // Start is called before the first frame update
    void Start()
    {
        //PosNum = PositionNumber.Start;
        PosNum = PositionNumber.Goal;

        GameManager.Instance.OnStateChange += BookMoveFirstPos;

        bookAnimator = book.GetComponentInChildren<Animator>();
        bookController = book.GetComponent<BookController>();
        doorscript=door.GetComponent<Door>();

        induction = null;
        instanceInduction = null;

        ableAdvance = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameState.LookAround)
        {
            if (PosNum != PositionNumber.Finish)
            {
                InductionControllere();
            }
        }
    }
    private void ChangePositionNumber()
    {
        if (!ableAdvance) return;

        induction.SetSentBoolTrue();

        if (PosNum < PositionNumber.Goal)
        {
            if (PosNum == PositionNumber.Start)
            {
                //StartCoroutine(LookAroundRoutine());
                PosNum++;
            }
            else
            {
                PosNum++;
                StartCoroutine(bookController.BookAppearanceRoutine(inductionSpawnPos[(int)PosNum], true));
                switch (PosNum)
                {
                    case PositionNumber.Two:
                        Subtitle.Instance.InstanceSubTitle(dialogueData, 29, 30);
                        break;
                    case PositionNumber.Three:
                        Subtitle.Instance.InstanceSubTitle(dialogueData, 31, 31);
                        break;
                    case PositionNumber.Goal:
                        Subtitle.Instance.InstanceSubTitle(dialogueData, 32, 32);
                        break;
                    default:
                        break;
                }
            }
            Debug.Log((int)PosNum);
        }
        else if (PosNum == PositionNumber.Goal)
        {
            //見回り終わり
            StartCoroutine(GameStartRutine());
            PosNum++;
        }

        ableAdvance = false;
    }

    private void InductionControllere()
    {
        if (instanceInduction == null && induction == null)
        {
            instanceInduction = Instantiate(inductionPrefub, inductionSpawnPos[(int)PosNum].transform.position, Quaternion.identity);
            induction = instanceInduction.GetComponent<Induction>();
        }
        //inductionがインスタンスされたら
        else if (instanceInduction != null && induction != null)
        {
            //近づいたら
            if (induction.approached)
            {
                ChangePositionNumber();
            }
        }

        if(!ableAdvance)
        {
            instanceInduction = null;
            induction = null;

            ableAdvance = true;
        }
    }
    private void BookMoveFirstPos(GameState newState)
    {
        if (newState == GameState.LookAround)
        {
            StartCoroutine(bookController.BookAppearanceRoutine(inductionSpawnPos[(int)PosNum], true));
            //こっちだぜ
            Subtitle.Instance.InstanceLoopSubTitle(dialogueData, 24, 24);
        }
    }
    /*private IEnumerator LookAroundRoutine()
    {
        Subtitle.Instance.StopSubTitle();
        yield return null;

        //ドアが閉まるまで
        while(!doorscript.IsInRoom)
        {
            Debug.Log(doorscript.IsInRoom);
            doorscript.CloseDoor();

            yield return null;
        }
        //やっぱりおばけがいる気がする
        Subtitle.Instance.InstanceSubTitle(dialogueData, 25, 25);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        //StartCoroutine(bookController.BookAppearanceRoutine(inductionSpawnPos[(int)PosNum], true));
        //Subtitle.Instance.InstanceSubTitle(dialogueData, 28, 28);
    }*/

    //こことアイマスクお化けが処理をしてる
    private IEnumerator GameStartRutine()
    {
        Subtitle.Instance.StopSubTitle();
        yield return null;

        //ドアが閉まるまで
        while (!doorscript.IsInRoom)
        {
            //Debug.Log(doorscript.IsInRoom);
            doorscript.CloseDoor();

            yield return null;
        }
        //やっぱりおばけがいる気がする
        Subtitle.Instance.InstanceSubTitle(dialogueData, 25, 25);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        Instantiate(eyeMaskBoo, book.transform.position, Quaternion.identity);
        Subtitle.Instance.InstanceSubTitle(dialogueGameData, 2, 3);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        //明るくなるまでまつ
        while (!DifferenceManager.Instance.Light)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        Subtitle.Instance.InstanceSubTitle(dialogueGameData, 5, 6);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        timer.SetActive(true);
        Instantiate(smokeEffect, timer.transform.position, Quaternion.identity);
        Subtitle.Instance.InstanceSubTitle(dialogueGameData, 7, 8);
        yield return new WaitUntil(() => Subtitle.Instance.nowSubtitleCoroutine == null);

        GameManager.Instance.ChangeState(GameState.Game);
    }
}

