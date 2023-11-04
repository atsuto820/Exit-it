using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public Animator anim;

    protected bool isGround;    // 接地判定
    protected static float dashTime;   // ダッシュし始めた時間

    protected static bool isFall;      // 落下 ～ リスポーンの状態か
    protected static bool isRespawnPrepare;
    protected static Vector3 respawnPosition;  // リスポーン位置
    protected static Vector3 fallPosition;     // 落下位置
    protected static int runDistance;    // 走った距離(切り捨て)

    private void Start()
    {
        dashTime = -1;
        isFall = false;
        isRespawnPrepare = false;
        runDistance = 0;
    }

    /// <summary>
    /// ダッシュ中か返す
    /// </summary>
    public bool IsDash()
    {
        return dashTime != -1;
    }

    /// <summary>
    /// 落下 ～ リスポーンの間の状態かを返す
    /// </summary>
    public bool IsFall()
    {
        return isFall;
    }

    /// <summary>
    /// 落下時処理
    /// </summary>
    public void Fall(Vector3 respawnPosition, Vector3 fallPosition)
    {
        isFall = true;
        isRespawnPrepare = false;
        Player.respawnPosition = respawnPosition;
        Player.fallPosition = fallPosition;
    }

    /// <summary>
    /// リスポーン準備
    /// </summary>
    public virtual void RespawnPrepare()
    {
        isRespawnPrepare = true;
        anim.SetInteger("State", 0);
    }

    /// <summary>
    /// リスポーン
    /// </summary>
    public virtual void Respawn()
    {
        isFall = false;
        isRespawnPrepare = false;
        anim.SetInteger("State", 1);
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    public virtual void GameOver()
    {
        GameMain.gameMain.GameOver(transform.position.x);
        SetVelocityZero();
        anim.SetInteger("State", 2);
    }

    public Vector3 GetFallPosition()
    {
        return fallPosition;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public abstract Vector3 GetSpeed();

    /// <summary>
    /// スピードをゼロにする
    /// </summary>
    public abstract void SetVelocityZero();
}
