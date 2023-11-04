using UnityEngine;

// プレイヤ－と触れたら指定のオブジェクトをアクティブにする
public class Activer : MonoBehaviour
{
    public GameObject obj;  // アクティブにするオブジェクト

    /// <summary>
    /// トリガーに触れた
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            obj.SetActive(true);
        }
    }
}
