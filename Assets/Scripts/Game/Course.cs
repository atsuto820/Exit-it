using UnityEngine;

public class Course : MonoBehaviour
{
    public int length;  // �R�[�X�̒���

    public int[] cameraMovePosition;    // cameraMovePlace�̔��f�����n�_��z���W
    public int[] cameraMovePlace;       // �J�������ǂ��ɓ����� (0 : ���� -1 : �� 1 : �E)

    public int[] Back3DPosition;        // Back3DPlace�̔��f�����n�_��z���W
    public int[] Back3DPlace;           // 2D -> 3D �̎���z���W

    public int[] respawnX;              // �����㕜���n�_��x���W
    public int[] respawnZ;              // �����㕜���n�_��z���W
}
