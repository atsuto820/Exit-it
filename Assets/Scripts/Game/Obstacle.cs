using UnityEngine;

// 移動する障害物
public class Obstacle : MonoBehaviour
{
    public Transform transorm2D, transorm3D;

    public AnimationCurve xAnim, yAnim, zAnim;
    public float maxX, minX;    // x座標の最大、最小 (初期位置を基準とする相対座標)
    private float maxXa, minXa; // x座標の最大、最小 (絶対座標)
    public float maxY, minY;    // y座標の最大、最小 (絶対座標)
    public float maxZ, minZ;    // z座標の最大、最小 (絶対座標)
    public float repeatTime;    // 反復時間

    private float time;

    private void Start()
    {
        maxXa = transorm2D.position.x + maxX;
        minXa = transorm2D.position.x + minX;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > repeatTime)
        {
            time -= repeatTime;
        }

        // 位置を設定
        transorm2D.position = new Vector3(minXa + xAnim.Evaluate(time) * (maxXa - minXa), minY + yAnim.Evaluate(time) * (maxY - minY), transorm2D.position.z);
        transorm3D.position = new Vector3(transorm3D.position.x, transorm3D.position.y, minZ + zAnim.Evaluate(time) * (maxZ - minZ));
    }
}
