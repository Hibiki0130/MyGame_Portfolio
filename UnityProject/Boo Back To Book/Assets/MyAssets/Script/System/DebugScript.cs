using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugLog;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        debugLog.text = ("aaaaaaa");
    }
}
