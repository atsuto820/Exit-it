using UnityEngine;

public class Player3D : Player
{
    public Rigidbody rb;
    private int conveyor;   // ベルトコンベアに乗っているか (0 : 乗っていない -1 : 左 1 : 右)

    /// <summary>
    /// 初期化処理。位置、速度などを変える
    /// </summary>
    public void Init(Vector3 position, Vector3 velocity)
    {
        transform.position = position;
        rb.velocity = velocity;
        anim.SetInteger("State", 1);
        conveyor = 0;
        isGround = false;
    }

    /// <summary>
    /// 対象のplayer2Dに位置、速度などを反映
    /// </summary>
    public void Reflect(Player2D player2D)
    {
        player2D.Init(new Vector3(transform.position.x, transform.position.y, 100), new Vector2(rb.velocity.x, rb.velocity.y));
    }

    private void Update()
    {
        if (GameMain.gameMain.IsStop)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (isRespawnPrepare)
        {
            // リスポーン準備
            transform.position = respawnPosition;
            rb.velocity = Vector3.zero;
            runDistance = (int)transform.position.x;
        }
        else
        {
            if (dashTime != -1 && Time.time - dashTime > 1.5f)
            {
                // ダッシュ終了
                dashTime = -1;
            }

            float xSpeed = rb.velocity.x;   // x方向 (前方向) への速度
            if (isFall)
            {
                // 落下判定後は前に進まない
                xSpeed = 0;
            }
            else
            {
                if (dashTime != -1)
                {
                    // ダッシュ中は定数
                    xSpeed = GameMain.gameMain.dashSpeed;
                }
                else
                {
                    // 前へ加速する
                    xSpeed += GameMain.gameMain.accelerationSpeed;
                    if (xSpeed > GameMain.gameMain.maxSpeed)
                    {
                        // 速度上限あり
                        xSpeed = GameMain.gameMain.maxSpeed;
                    }
                }
            }
            rb.velocity = new Vector3(xSpeed, rb.velocity.y, 0);

            // このフレーム走った距離をスコアに加算
            int runDistanceCurrent = (int)(transform.position.x - runDistance);
            GameMain.gameMain.AddScore(runDistanceCurrent * 0.1f);
            runDistance = (int)transform.position.x;

            if (isGround && Input.GetKeyDown(KeyCode.Space))
            {
                // ジャンプ
                rb.velocity += Vector3.up * GameMain.gameMain.jump;
                GameMain.gameMain.PlaySfx(0);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // 左移動
                rb.velocity += Vector3.forward * GameMain.gameMain.moveSpeed;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                // 右移動
                rb.velocity += Vector3.back * GameMain.gameMain.moveSpeed;
            }

            if (conveyor == 1)
            {
                // 左コンベア移動
                rb.velocity += Vector3.forward * 10;
            }
            else if (conveyor == -1)
            {
                // 右コンベア移動
                rb.velocity += Vector3.back * 10;
            }
        }
    }

    /// <summary>
    /// トリガーに触れた or トリガーが触れた
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Energy"))
        {
            // エネルギーを取得
            GameMain.gameMain.PlaySfx(1);
            GameMain.gameMain.AddScore(50);
            if(other.transform.parent.childCount == 1)
            {
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Fire"))
        {
            // 炎に触れた → ゲームオーバー
            if (!GameMain.gameMain.IsGameOver)
            {
                GameOver();
            }
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            // 爆弾に衝突
            GameMain.gameMain.PlaySfx(2);
            Animator anim = other.transform.parent.transform.parent.GetComponent<Animator>();
            anim.SetTrigger("Explosion");
            rb.velocity = new Vector3(-30 * anim.transform.localScale.x, rb.velocity.y, rb.velocity.z);
            dashTime = -1;
        }
        else
        {
            if (other.gameObject.CompareTag("DashBoard"))
            {
                // ダッシュボードに触れた → ダッシュ
                if (dashTime == -1)
                {
                    GameMain.gameMain.PlaySfx(3);
                }
                dashTime = Time.time;
            }
            else if (other.gameObject.CompareTag("LeftConveyor"))
            {
                conveyor = 1;
            }
            else if (other.gameObject.CompareTag("RightConveyor"))
            {
                conveyor = -1;
            }

            // GroundCheckerによる接地判定
            isGround = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("LeftConveyor"))
        {
            conveyor = 0;
        }
        else if (collision.gameObject.CompareTag("RightConveyor"))
        {
            conveyor = 0;
        }
        isGround = false;
    }

    public override void SetVelocityZero()
    {
        rb.velocity = Vector3.zero;
    }

    public override Vector3 GetSpeed()
    {
        return rb.velocity;
    }

    /// <summary>
    /// ゲーム開始前アニメーション終了 -> Reay Go!表示
    /// </summary>
    public void Ready()
    {
        GameMain.gameMain.Ready();
    }

    public void FireAnimation()
    {
        GameMain.gameMain.FireAnimation();
    }
}
