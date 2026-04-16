using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    [SerializeField] private GameObject GhostPos;
    [SerializeField] private GameObject Ghost;

    private bool ableInstance = true;

    // Start is called before the first frame update
    void Start()
    {
        GhostInstance();
    }

    private void FirstSet()
    {
        ableInstance = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.G))
        {
            ableInstance = true;
        }

        GhostInstance();
        CheckGhost();
    }

    private void GhostInstance()
    {
        if(ableInstance)
        {
            Instantiate(Ghost, GhostPos.transform.position, Quaternion.identity);
            ableInstance = false;
        }
    }

    //àÍêlÇ‡Ç¢Ç»Ç≠Ç»Ç¡ÇΩÇÁèoåª
    private float time = 0f;
    private void CheckGhost()
    {
        GameObject[] Ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        if (Ghosts.Length <= 0)
        {
            time += Time.deltaTime;

            if (time >= 3.0f)
            {
                ableInstance = true;
            }
        }
        else
        {
            time = 0f;
        }
    }
}
