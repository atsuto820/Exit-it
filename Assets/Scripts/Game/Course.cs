using UnityEngine;

public class Course : MonoBehaviour
{
    public int length;  // コースの長さ

    public int[] cameraMovePosition;    // cameraMovePlaceの反映される地点のz座標
    public int[] cameraMovePlace;       // カメラがどこに動くか (0 : 中央 -1 : 左 1 : 右)

    public int[] Back3DPosition;        // Back3DPlaceの反映される地点のz座標
    public int[] Back3DPlace;           // 2D -> 3D の時のz座標

    public int[] respawnX;              // 落下後復活地点のx座標
    public int[] respawnZ;              // 落下後復活地点のz座標
}
