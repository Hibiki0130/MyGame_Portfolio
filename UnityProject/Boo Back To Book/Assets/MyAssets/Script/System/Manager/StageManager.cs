using Oculus;
using OVR;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class StageManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugLog;
    [Header("ステージオブジェクト")]
    [SerializeField] private GameObject hallWay;
    [SerializeField] private GameObject room;

    [Header("VR関連")]
    [SerializeField] private Transform cameraOffset;  // CameraOffset
    [SerializeField] private Transform hmdCamera;// 中のCamera(HMD)
    [SerializeField] private GameObject cameraRig;

    [Header("スタート位置")]
    [SerializeField] private Transform startPointHallway;

    // オフセット値（廊下・部屋の位置調整）
    private Vector3 hallWayLocalOffset = new Vector3(-2.38f, 1.12f, -2.06f);
    private Vector3 roomLocalOffset = new Vector3(-2.55f, 1.102f, -1.51f);

    OVRManager ovrManager;
    private void Awake()
    {
    }
    void Start()
    {
        ovrManager = FindObjectOfType<OVRManager>();
        if (ovrManager == null)
        {
            Debug.Log("nai");
        }
        StartCoroutine(SetupStageCoroutine());
    }

    private IEnumerator SetupStageCoroutine()
    {
        // 1〜2フレーム待ってトラッキング初期化を安定させる
        yield return null;
        yield return null;

        //ovrManager.RecenterPose();
        // OpenXR経由でリセンター
        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        foreach (var sub in subsystems)
        {
            sub.TryRecenter();
        }

        yield return null;

        // さらに1フレーム待つと安定
        yield return null;

        // カメラ補正でスタート位置に合わせる
        ResetCameraOffsetToStart();

        yield return new WaitForSeconds(1f);

        // 廊下・部屋の配置・向きを更新
        SetStageRotation();
    }

    private void ResetCameraOffsetToStart()
    {
        // XZだけ補正（Yは身長補正のため触らない）
        Vector3 delta = startPointHallway.position - hmdCamera.position;
        Vector3 pos = cameraOffset.position;
        pos.x += delta.x;
        pos.z += delta.z;
        cameraOffset.position = pos;

        // 向き合わせ（Y軸のみ）
        Vector3 forward = hmdCamera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 targetForward = startPointHallway.forward;
        targetForward.y = 0;
        targetForward.Normalize();

        float angle = Vector3.SignedAngle(forward, targetForward, Vector3.up);
        cameraOffset.Rotate(Vector3.up, angle);
    }

    private void SetStageRotation()
    {
        Transform playerTrans = hmdCamera;

        // 廊下の位置
        Vector3 forward = playerTrans.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 hallwayOffset = forward * hallWayLocalOffset.z + playerTrans.right * hallWayLocalOffset.x + Vector3.up * hallWayLocalOffset.y;
        hallWay.transform.position = playerTrans.position + hallwayOffset;

        // 部屋の位置
        Vector3 roomOffset = forward * roomLocalOffset.z + playerTrans.right * roomLocalOffset.x + Vector3.up * roomLocalOffset.y;
        room.transform.position = playerTrans.position + roomOffset;

        // 向きを合わせる（Y軸だけ回転）
        hallWay.transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 90f, 0);
        room.transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 90f, 0);
    }
}