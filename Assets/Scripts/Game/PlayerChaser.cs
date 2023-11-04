using UnityEngine;

// プレイヤーを追尾する
public class PlayerChaser : MonoBehaviour
{
    public Transform t;

    public AnimationCurve respawnMove;  // 落下後のカメラの動き

    private bool is3D;          // 3Dか

    private float fallTime;     // 落下時間
    private float fallPosition, respawnPosition;    // 落下座標、リスポーン座標
    private float respawnTime;  // リスポーン時間

    public void Start()
    {
        is3D = true;
        fallTime = -1;
    }

    public void SwitchDimension()
    {
        is3D = !is3D;
    }

    private void Update()
    {
        if (GameMain.gameMain.IsGameOver)
        {
            return;
        }

        if (fallTime == -1)
        {
            // プレイヤーに追従
            t.position = new Vector3(GameMain.gameMain.GetPlayer().GetPosition().x, 0, 0);
        }
        else
        {
            // 落下後
            if(Time.time - fallTime > 1f + respawnTime)
            {
                fallTime = -1;
            }
            else
            {
                if (Time.time - fallTime > 1f)
                {
                    t.position = new Vector3(fallPosition + (respawnPosition - fallPosition) * respawnMove.Evaluate((Time.time - fallTime - 1f) / respawnTime), 0, 0);
                }
            }
        }
    }

    // プレイヤーが落下
    public void Fall(float fallPosition, float respawnPosition)
    {
        fallTime = Time.time;
        this.fallPosition = fallPosition;
        this.respawnPosition = respawnPosition;
        respawnTime = (respawnPosition - fallPosition) / 15f + 0.5f;
    }

    // プレイヤーがゲームオーバー
    public void GameOver(float deathPosition)
    {
        t.position = new Vector3(deathPosition, 0, 0);
    }
}
