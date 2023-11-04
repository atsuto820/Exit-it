using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Fire : MonoBehaviour
{
    public Transform t;
    public TextMeshPro fireText;
    public Volume volume;
    public AudioSource fireSound;

    private Vignette vignette;

    public float speedDashTime; // プレイヤーがダッシュ中に距離を縮める速度
    public float speedRunTime;  // プレイヤーが走っているときに距離を縮める速度
    public float speedStopTime; // プレイヤーが止まっているときに距離を縮める速度
    public float speedFallTime; // プレイヤーが落下～リスポーンしているときに距離を縮める速度
    public float playerBack;    // プレイヤーを離れる限界

    private float distance;     // プレイヤーとの距離
    private float deathPosition;// ゲームオーバーx座標

    private void Start()
    {
        distance = -10 - t.position.x;
        volume.profile.TryGet(out vignette);
    }

    private void Update()
    {
        if (GameMain.gameMain.IsStop)
        {
            if (GameMain.gameMain.IsGameOver)
            {
                // ゲームオーバー後
                // 死亡位置まで移動して止まる
                distance -= speedStopTime * Time.deltaTime;
                if (distance < -10)
                {
                    distance = -10;
                }

                fireText.text = Mathf.Max(distance, 0).ToString("F1") + "m";                            // 炎までの距離表示 (小数点第一位まで)
                t.position = new Vector3(deathPosition - distance - 10, t.position.y, t.position.z);    // 位置更新
                vignette.intensity.value = Mathf.Max(0, 0.5f - distance / playerBack * 0.4f);           // ビネット適用
                fireSound.volume = Mathf.Max(0, 1f - distance / playerBack * 1f);                       // 炎の音量
                return;
            }
            return;
        }

        Player player = GameMain.gameMain.GetPlayer();
        if (player.IsFall())
        {
            // 落下 ～ リスポーン
            distance -= speedFallTime * Time.deltaTime;
        }
        else
        {
            if (player.IsDash())
            {
                // ダッシュ中
                distance -= speedDashTime * Time.deltaTime;
            }
            else
            {
                distance -= (speedStopTime + (speedRunTime - speedStopTime) * (player.GetSpeed().x / GameMain.gameMain.maxSpeed)) * Time.deltaTime;
            }
            // プレイヤーを離れる限界以上に離れない
            if (distance > playerBack)
            {
                distance = playerBack;
            }
        }

        if(distance < 0)
        {
            distance = 0;
        }
        fireText.text = distance.ToString("F1") + "m";                                                  // 炎までの距離表示 (小数点第一位まで)
        t.position = new Vector3(player.GetPosition().x - distance - 10, t.position.y, t.position.z);   // 位置更新
        vignette.intensity.value = Mathf.Max(0, 0.5f - distance / playerBack * 0.4f);                   // ビネット適用
        fireSound.volume = Mathf.Max(0, 1f - distance / playerBack * 1f);                               // 炎の音量

        if (player.IsFall() && distance == 0)
        {
            // 落下後に距離が0ならゲームオーバー
            player.GameOver();
        }
    }

    /// <summary>
    /// プレイヤーがゲームオーバー<br />
    /// 火の動きを死亡地点で止める
    /// </summary>
    /// <param name="deathPosition">死亡地点x座標</param>
    public void GameOver(float deathPosition)
    {
        this.deathPosition = deathPosition;
    }

    /// <summary>
    /// プレイヤーがリスポーン準備<br />
    /// リスポーン時プレイヤーが前に瞬間移動するので位置の補正を行う
    /// </summary>
    /// <param name="move">瞬間移動する距離</param>
    public void RespawnPrepare(float move)
    {
        distance += move;
    }
}
