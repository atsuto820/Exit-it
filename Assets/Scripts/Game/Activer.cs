using UnityEngine;

// �v���C���|�ƐG�ꂽ��w��̃I�u�W�F�N�g���A�N�e�B�u�ɂ���
public class Activer : MonoBehaviour
{
    public GameObject obj;  // �A�N�e�B�u�ɂ���I�u�W�F�N�g

    /// <summary>
    /// �g���K�[�ɐG�ꂽ
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            obj.SetActive(true);
        }
    }
}
