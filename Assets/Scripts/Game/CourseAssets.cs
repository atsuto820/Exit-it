using UnityEngine;

[CreateAssetMenu(fileName = "CourseAssets", menuName = "ScriptableObjects/CourseAssets���쐬")]
public class CourseAssets : ScriptableObject
{
    [SerializeField]
    public Course courseStart;

    [SerializeField]
    public Course[] courses;

    [SerializeField]
    public int debug;    // (�R�[�X�m�F�f�o�b�O�p) -1�ȊO�Ȃ炻�̗v�f��course�����o�Ȃ�
}
