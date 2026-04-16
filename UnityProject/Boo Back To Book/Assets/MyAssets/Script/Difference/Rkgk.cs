using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rkgk : MonoBehaviour
{
    public bool Clear {  get; private set; }

    private int chosenSound;
    private float rubedPicture;
    private bool ableInstanceSound;
    RKGKSound rkgksound;
    // Start is called before the first frame update
    void Start()
    {
        Clear = false;
        ableInstanceSound = true;
        rubedPicture = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (rkgksound == null)
        {
            rkgksound = GetComponentInParent<RKGKSound>();
        }

        if (GameManager.Instance.state == GameState.Game)
        {
            if (rubedPicture > 3f)
            {
                Clear = true;
                transform.localScale = Vector3.zero;
            }
        }
    }
    private IEnumerator RubSound()
    {
        if (!ableInstanceSound) yield break;

        ableInstanceSound = false;

        rkgksound.RumdomPlay();

        yield return new WaitForSeconds(0.5f);

        rubedPicture++;

        ableInstanceSound = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.state == GameState.Game)
        {
            if (other.CompareTag("LeftHand"))
            {
                if (HandTraking.Instance.GetLeftRub())
                {
                    StartCoroutine(RubSound());
                }
            }
            if (other.CompareTag("RightHand"))
            {
                if (HandTraking.Instance.GetRightRub())
                {
                    StartCoroutine(RubSound());
                }
            }
        }
    }
}
