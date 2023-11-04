using UnityEngine;

public class Player2D : Player
{
    public Rigidbody2D rb;

    /// <summary>
    /// 初期化処理。位置、速度などを変える
    /// </summary>
    public void Init(Vector3 position, Vector2 velocity)
    {
        transform.position = position;
        rb.velocity = velocity;
        anim.SetInteger("State", 1);
        isGround = false;
    }

    /// <summary>
    /// 対象のplayer3Dに位置、速度などを反映
    /// </summary>
    public void Reflect(Player3D player3D, float z)
    {
        player3D.Init(new Vector3(transform.position.x, transform.position.y, z), new Vector3(rb.velocity.x, rb.velocity.y, 0));
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
            // 落下後
            transform.position = new Vector3(respawnPosition.x, respawnPosition.y, 100);
            rb.velocity = Vector3.zero;
            runDistance = (int)transform.position.x;
        }
        else
        {
            if (Time.time - dashTime > 1.5f)
            {
                dashTime = -1;
            }

            float xSpeed = rb.velocity.x;   // x方向 (前方向) への速度
            if (dashTime != -1)
            {
                // ダッシュ中は定数
                xSpeed = GameMain.gameMain.dashSpeed;
            }
            else
            {
                // 前へ加速する
                if (isFall)
                {
                    // 落下判定後は前に進まない
                    xSpeed = 0;
                }
                else
                {
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
                rb.velocity += Vector2.up * GameMain.gameMain.jump;
                GameMain.gameMain.PlaySfx(0);
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Energy"))
        {
            // エネルギーを取得
            GameMain.gameMain.PlaySfx(1);
            GameMain.gameMain.AddScore(50);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Fire"))
        {
            // 炎に触れた → ゲームオーバー
            GameOver();
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            // 爆弾に衝突
            GameMain.gameMain.PlaySfx(2);
            Animator anim = other.transform.parent.GetComponent<Animator>();
            rb.velocity = new Vector2(-30 * anim.transform.localScale.x, rb.velocity.y);
            anim.SetTrigger("Explosion");
            dashTime = -1;
        }
        else
        {
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGround = false;
    }

    public override Vector3 GetSpeed()
    {
        return rb.velocity;
    }

    public override void SetVelocityZero()
    {
        rb.velocity = Vector3.zero;
    }
}
