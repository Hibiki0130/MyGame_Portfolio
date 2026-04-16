using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float radius = 0.5f;     // 半円の半径
    [SerializeField] private float moveSpeed = 1.0f;  // 移動速度
    [SerializeField] private float waitTime = 2.0f;   // 次の移動先までの待機時間

    private Vector3 targetPos;
    private float timer;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        if (player == null) return;

        MoveInsideHalfCircle();
    }

    private void MoveInsideHalfCircle()
    {
        // ターゲットへ移動
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // ターゲットにほぼ到達したら次を設定
        if (Vector3.Distance(transform.position, targetPos) < 0.02f)
        {
            timer += Time.deltaTime;
            if (timer > waitTime)
            {
                PickNewTarget();
                timer = 0f;
            }
        }
    }

    private void PickNewTarget()
    {
        Vector3 center = player.transform.position;

        // 半円の中のランダムな位置を決める
        // 角度を 0°～180°（プレイヤーの前方半分）で選ぶ
        float angle = Random.Range(0f, 180f) * Mathf.Deg2Rad;
        float dist = Random.Range(0.1f, radius); // 半径の内側にも動けるように

        // プレイヤーの forward を基準に半円を前方に
        Vector3 forward = player.transform.forward;
        Vector3 right = player.transform.right;

        // 半円内でのオフセット計算
        Vector3 offset = forward * Mathf.Cos(angle) * dist + right * Mathf.Sin(angle) * dist;

        // 新しいターゲット
        targetPos = center + offset;

        // Y座標は少しふわふわ
        targetPos.y = center.y + Random.Range(0.05f, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
        // デバッグ表示：プレイヤー前方半円の範囲
        if (player == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Vector3 center = player.transform.position;

        // 半円の扇形を描画
        int segments = 20;
        Vector3 prev = center + player.transform.forward * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(0f, 180f, i / (float)segments) * Mathf.Deg2Rad;
            Vector3 next = center +
                (player.transform.forward * Mathf.Cos(angle) + player.transform.right * Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}