using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBook : MonoBehaviour
{
    [SerializeField] private float move;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        FloatAnimation();
    }

    private void FloatAnimation()
    {
        time += Time.deltaTime;
        if ((int)time % 2 == 0)
        {
            transform.position += new Vector3(0, move, 0);
        }
        else if ((int)time % 2 == 1)
        {
            transform.position -= new Vector3(0, move, 0);
        }
    }
}
