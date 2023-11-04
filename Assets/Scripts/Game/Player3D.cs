using UnityEngine;

public class Player3D : Player
{
    public Rigidbody rb;
    private int conveyor;   // �x���g�R���x�A�ɏ���Ă��邩 (0 : ����Ă��Ȃ� -1 : �� 1 : �E)

    /// <summary>
    /// �����������B�ʒu�A���x�Ȃǂ�ς���
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
    /// �Ώۂ�player2D�Ɉʒu�A���x�Ȃǂ𔽉f
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
            // ���X�|�[������
            transform.position = respawnPosition;
            rb.velocity = Vector3.zero;
            runDistance = (int)transform.position.x;
        }
        else
        {
            if (dashTime != -1 && Time.time - dashTime > 1.5f)
            {
                // �_�b�V���I��
                dashTime = -1;
            }

            float xSpeed = rb.velocity.x;   // x���� (�O����) �ւ̑��x
            if (isFall)
            {
                // ���������͑O�ɐi�܂Ȃ�
                xSpeed = 0;
            }
            else
            {
                if (dashTime != -1)
                {
                    // �_�b�V�����͒萔
                    xSpeed = GameMain.gameMain.dashSpeed;
                }
                else
                {
                    // �O�։�������
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
                rb.velocity += Vector3.up * GameMain.gameMain.jump;
                GameMain.gameMain.PlaySfx(0);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // ���ړ�
                rb.velocity += Vector3.forward * GameMain.gameMain.moveSpeed;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                // �E�ړ�
                rb.velocity += Vector3.back * GameMain.gameMain.moveSpeed;
            }

            if (conveyor == 1)
            {
                // ���R���x�A�ړ�
                rb.velocity += Vector3.forward * 10;
            }
            else if (conveyor == -1)
            {
                // �E�R���x�A�ړ�
                rb.velocity += Vector3.back * 10;
            }
        }
    }

    /// <summary>
    /// �g���K�[�ɐG�ꂽ or �g���K�[���G�ꂽ
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Energy"))
        {
            // �G�l���M�[���擾
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
            // ���ɐG�ꂽ �� �Q�[���I�[�o�[
            if (!GameMain.gameMain.IsGameOver)
            {
                GameOver();
            }
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            // ���e�ɏՓ�
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
                // �_�b�V���{�[�h�ɐG�ꂽ �� �_�b�V��
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

            // GroundChecker�ɂ��ڒn����
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
    /// �Q�[���J�n�O�A�j���[�V�����I�� -> Reay Go!�\��
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
