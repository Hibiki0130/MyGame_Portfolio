using UnityEngine;
using System.Collections;

public class PositionSet : MonoBehaviour
{
    [SerializeField] private Transform cameraRig;      // CameraRig（親）
    [SerializeField] private Transform centerEye;      // CenterEyeAnchor（カメラ）
    [SerializeField] private Transform startPosition;  // リスポーン地点

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f); // 0.2秒だけ待つ

        Vector3 eyeOffset = cameraRig.position - centerEye.position;
        cameraRig.position = startPosition.position + eyeOffset;

        float yawDiff = startPosition.eulerAngles.y - centerEye.eulerAngles.y;
        cameraRig.Rotate(0, yawDiff, 0);
    }
}