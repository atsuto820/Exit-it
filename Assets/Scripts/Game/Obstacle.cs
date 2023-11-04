using UnityEngine;

// �ړ������Q��
public class Obstacle : MonoBehaviour
{
    public Transform transorm2D, transorm3D;

    public AnimationCurve xAnim, yAnim, zAnim;
    public float maxX, minX;    // x���W�̍ő�A�ŏ� (�����ʒu����Ƃ��鑊�΍��W)
    private float maxXa, minXa; // x���W�̍ő�A�ŏ� (��΍��W)
    public float maxY, minY;    // y���W�̍ő�A�ŏ� (��΍��W)
    public float maxZ, minZ;    // z���W�̍ő�A�ŏ� (��΍��W)
    public float repeatTime;    // ��������

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

        // �ʒu��ݒ�
        transorm2D.position = new Vector3(minXa + xAnim.Evaluate(time) * (maxXa - minXa), minY + yAnim.Evaluate(time) * (maxY - minY), transorm2D.position.z);
        transorm3D.position = new Vector3(transorm3D.position.x, transorm3D.position.y, minZ + zAnim.Evaluate(time) * (maxZ - minZ));
    }
}
