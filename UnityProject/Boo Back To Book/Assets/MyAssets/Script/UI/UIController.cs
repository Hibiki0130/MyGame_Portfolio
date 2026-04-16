using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;//centereye

    private float distance;
    private float followSpeed = 5f;
    private TextMeshPro myText;

    // Start is called before the first frame update
    void Start()
    {
        distance = Vector3.Distance(transform.position, cameraTransform.position);
        myText = GetComponent<TextMeshPro>();
        //subtitleDistance = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //subtitleText.text = "" + cameraTransform.position;
        //Debug.Log(subtitleDistance);
        UIControl();
    }

    private void UIControl()
    {
        //カメラの中心とオブジェクトの中心が常に一緒
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * distance;
        //targetPos.y = targetPos.y - 0.2f;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        Quaternion targetRot = Quaternion.LookRotation(transform.position - cameraTransform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }
}
