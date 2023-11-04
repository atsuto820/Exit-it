using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public Animator anim;

    protected bool isGround;    // �ڒn����
    protected static float dashTime;   // �_�b�V�����n�߂�����

    protected static bool isFall;      // ���� �` ���X�|�[���̏�Ԃ�
    protected static bool isRespawnPrepare;
    protected static Vector3 respawnPosition;  // ���X�|�[���ʒu
    protected static Vector3 fallPosition;     // �����ʒu
    protected static int runDistance;    // ����������(�؂�̂�)

    private void Start()
    {
        dashTime = -1;
        isFall = false;
        isRespawnPrepare = false;
        runDistance = 0;
    }

    /// <summary>
    /// �_�b�V�������Ԃ�
    /// </summary>
    public bool IsDash()
    {
        return dashTime != -1;
    }

    /// <summary>
    /// ���� �` ���X�|�[���̊Ԃ̏�Ԃ���Ԃ�
    /// </summary>
    public bool IsFall()
    {
        return isFall;
    }

    /// <summary>
    /// ����������
    /// </summary>
    public void Fall(Vector3 respawnPosition, Vector3 fallPosition)
    {
        isFall = true;
        isRespawnPrepare = false;
        Player.respawnPosition = respawnPosition;
        Player.fallPosition = fallPosition;
    }

    /// <summary>
    /// ���X�|�[������
    /// </summary>
    public virtual void RespawnPrepare()
    {
        isRespawnPrepare = true;
        anim.SetInteger("State", 0);
    }

    /// <summary>
    /// ���X�|�[��
    /// </summary>
    public virtual void Respawn()
    {
        isFall = false;
        isRespawnPrepare = false;
        anim.SetInteger("State", 1);
    }

    /// <summary>
    /// �Q�[���I�[�o�[
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
    /// �X�s�[�h���[���ɂ���
    /// </summary>
    public abstract void SetVelocityZero();
}
