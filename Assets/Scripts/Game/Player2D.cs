using UnityEngine;

public class Player2D : Player
{
    public Rigidbody2D rb;

    /// <summary>
    /// �����������B�ʒu�A���x�Ȃǂ�ς���
    /// </summary>
    public void Init(Vector3 position, Vector2 velocity)
    {
        transform.position = position;
        rb.velocity = velocity;
        anim.SetInteger("State", 1);
        isGround = false;
    }

    /// <summary>
    /// �Ώۂ�player3D�Ɉʒu�A���x�Ȃǂ𔽉f
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
            // ������
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

            float xSpeed = rb.velocity.x;   // x���� (�O����) �ւ̑��x
            if (dashTime != -1)
            {
                // �_�b�V�����͒萔
                xSpeed = GameMain.gameMain.dashSpeed;
            }
            else
            {
                // �O�։�������
                if (isFall)
                {
                    // ���������͑O�ɐi�܂Ȃ�
                    xSpeed = 0;
                }
                else
                {
                    xSpeed += GameMain.gameMain.accelerationSpeed;
                    if (xSpeed > GameMain.gameMain.maxSpeed)
                    {
                        // ���x�������
                        xSpeed = GameMain.gameMain.maxSpeed;
                    }
                }
            }
            rb.velocity = new Vector3(xSpeed, rb.velocity.y, 0);

            // ���̃t���[���������������X�R�A�ɉ��Z
            int runDistanceCurrent = (int)(transform.position.x - runDistance);
            GameMain.gameMain.AddScore(runDistanceCurrent * 0.1f);
            runDistance = (int)transform.position.x;

            if (isGround && Input.GetKeyDown(KeyCode.Space))
            {
                // �W�����v
                rb.velocity += Vector2.up * GameMain.gameMain.jump;
                GameMain.gameMain.PlaySfx(0);
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Energy"))
        {
            // �G�l���M�[���擾
            GameMain.gameMain.PlaySfx(1);
            GameMain.gameMain.AddScore(50);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Fire"))
        {
            // ���ɐG�ꂽ �� �Q�[���I�[�o�[
            GameOver();
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            // ���e�ɏՓ�
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
