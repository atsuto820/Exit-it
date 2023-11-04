using UnityEngine;

// オブジェクトの配置z座標をランダムに決定する
public class Shuffler : MonoBehaviour
{
    public Transform[] obj, obj2, obj3;
    public float[] zs;

    private void Start()
    {
        // objの位置をランダムに決定
        int z1 = Random.Range(0, zs.Length);
        foreach (Transform t in obj)
        {
            t.position = new Vector3(t.position.x, t.position.y, zs[z1]);
        }

        if (obj2 != null)
        {
            // obj2の位置をランダムに決定
            int z2 = Random.Range(0, zs.Length - 1);
            if(z2 == z1)
            {
                // objと被らないようにする
                z2 = zs.Length - 1;
            }
            foreach (Transform t in obj2)
            {
                t.position = new Vector3(t.position.x, t.position.y, zs[z2]);
            }

            if (obj3 != null)
            {
                // obj3の位置をランダムに決定
                int z3 = Random.Range(0, zs.Length - 2);
                if (z3 == z1 || z3 == z2)
                {
                    // obj、obj2と被らないようにする
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
