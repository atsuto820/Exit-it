using UnityEngine;

// �I�u�W�F�N�g�̔z�uz���W�������_���Ɍ��肷��
public class Shuffler : MonoBehaviour
{
    public Transform[] obj, obj2, obj3;
    public float[] zs;

    private void Start()
    {
        // obj�̈ʒu�������_���Ɍ���
        int z1 = Random.Range(0, zs.Length);
        foreach (Transform t in obj)
        {
            t.position = new Vector3(t.position.x, t.position.y, zs[z1]);
        }

        if (obj2 != null)
        {
            // obj2�̈ʒu�������_���Ɍ���
            int z2 = Random.Range(0, zs.Length - 1);
            if(z2 == z1)
            {
                // obj�Ɣ��Ȃ��悤�ɂ���
                z2 = zs.Length - 1;
            }
            foreach (Transform t in obj2)
            {
                t.position = new Vector3(t.position.x, t.position.y, zs[z2]);
            }

            if (obj3 != null)
            {
                // obj3�̈ʒu�������_���Ɍ���
                int z3 = Random.Range(0, zs.Length - 2);
                if (z3 == z1 || z3 == z2)
                {
                    // obj�Aobj2�Ɣ��Ȃ��悤�ɂ���
                    if(z1 != zs.Length - 2 && z2 != zs.Length - 1 && z1 == zs.Length - 1 && z2 == zs.Length - 2)
                    {
                        z3 = Random.Range(zs.Length - 2, zs.Length);
                    }
                    else
                    {
                        z3 = (z1 == zs.Length - 2 || z2 == zs.Length - 2) ? zs.Length - 1 : zs.Length - 2;
                    }
                }
                foreach (Transform t in obj3)
                {
                    t.position = new Vector3(t.position.x, t.position.y, zs[z3]);
                }
            }
        }
    }
}
