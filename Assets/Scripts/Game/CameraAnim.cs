using UnityEngine;

public class CameraAnim : MonoBehaviour
{
    public Animator anim;

    public void SwitchDimension()
    {
        // 次元の切り替え
        GameMain.gameMain.SwitchDimension();
    }
}
