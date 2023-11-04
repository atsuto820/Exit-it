using UnityEngine;

[CreateAssetMenu(fileName = "CourseAssets", menuName = "ScriptableObjects/CourseAssetsを作成")]
public class CourseAssets : ScriptableObject
{
    [SerializeField]
    public Course courseStart;

    [SerializeField]
    public Course[] courses;

    [SerializeField]
    public int debug;    // (コース確認デバッグ用) -1以外ならその要素のcourseしか出ない
}
