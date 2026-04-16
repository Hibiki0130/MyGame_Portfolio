using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RKGKSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] rubSound;
    [SerializeField] private BoxCollider[] boxCollider;

    PlaySound playsound;
    // Start is called before the first frame update
    void Start()
    {
        playsound = GetComponent<PlaySound>();
        //DifferenceManager.Instance.OnDiffrenceStateChange += WhenChangeDifferenceState;
    }

    // Update is called once per frame
    void Update()
    {
        if (playsound == null)
        {
            playsound = GetComponent<PlaySound>();
        }
    }
    /*private void WhenChangeDifferenceState(DifferenceState newState)
    {
        if (newState == DifferenceManager.Instance.state) return;

        if (newState == DifferenceState.Serch)
        {
            for (int i = 0;i < boxCollider.Length;i++)
            {
                boxCollider[i].enabled = true;
            }
        }
        if (newState == DifferenceState.Catch)
        {
            for (int i = 0; i < boxCollider.Length; i++)
            {
                boxCollider[i].enabled = false;
            }
        }
    }*/
    public void RumdomPlay()
    {
        int chosenSound = UnityEngine.Random.Range(0, rubSound.Length);
        playsound.PlayOneShot(rubSound[chosenSound]);
    }
}
