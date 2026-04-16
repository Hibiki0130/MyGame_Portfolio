using Meta.XR.ImmersiveDebugger.UserInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class CameraManager: MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private GameObject cameraoffset;
    [SerializeField] private GameObject allCamera;
    [SerializeField] private TextMeshPro debugLog;

    private float followSpeed = 5f;
    private bool Pos;
    private void Awake()
    {
        //SetOffSet();
        //シングルトン初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //SetOffSet();
    }

    // Start is called before the first frame update
    void Start()
    {
        Pos = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    Vector3 vectorPlayerHight;
    float offset;
    public IEnumerator OffSetRoutine()
    {
        vectorPlayerHight = cameraoffset.transform.position;
        float playerhight = vectorPlayerHight.y;
        float multifive = playerhight * (-5.0f);
        float divithirtyeight = multifive / 38.0f;

        offset = Mathf.Round(divithirtyeight * 100f) / 100f;

        Vector3 start = allCamera.transform.localPosition;
        Vector3 target = new Vector3(start.x, offset, start.z);
        //身長合わせ
        //cameraoffset.transform.position = new Vector3(0f, 10f, 0f);
        while (Mathf.Abs(allCamera.transform.localPosition.y - offset) > 0.01f)
        {
            allCamera.transform.localPosition =
                Vector3.Lerp(allCamera.transform.localPosition, target, 1.5f * Time.deltaTime);

            yield return null;
        }

        allCamera.transform.localPosition = target;
    }
    public void UIControl(Transform obj,float distance,float minus_x,float minus_y)
    {
        //カメラの中心を原点としてオブジェクトのいちを移動
        Vector3 targetPos = transform.position + transform.forward * distance;
        targetPos.x += minus_x;
        targetPos.y += minus_y;
        obj.transform.position = Vector3.Lerp(obj.position, targetPos, Time.deltaTime * followSpeed);

        //カメラへ向く
        Quaternion targetRot = Quaternion.LookRotation(obj.position - transform.position);
        obj.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }

    /*private void OnBecameVisible()
    {
        debugLog.text = "can see!";
    }

    private void OnBecameInvisible()
    {
        debugLog.text = "can't see...";
    }*/
    /*public void SetOffSet()
    {
        vectorPlayerHight = cameraoffset.transform.position;
        float playerhight = vectorPlayerHight.y;
        float multifive = playerhight * (-5.0f);
        float divithirtyeight = multifive / 38.0f;

        offset = Mathf.Round(divithirtyeight * 100f) / 100f;

        //身長合わせ
        //cameraoffset.transform.position = new Vector3(0f, 10f, 0f);
        allCamera.transform.localPosition = new Vector3(0f, offset, 0f);
    }*/
}
