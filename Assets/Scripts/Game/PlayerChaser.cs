using UnityEngine;

// �v���C���[��ǔ�����
public class PlayerChaser : MonoBehaviour
{
    public Transform t;

    public AnimationCurve respawnMove;  // ������̃J�����̓���

    private bool is3D;          // 3D��

    private float fallTime;     // ��������
    private float fallPosition, respawnPosition;    // �������W�A���X�|�[�����W
    private float respawnTime;  // ���X�|�[������

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
            // �v���C���[�ɒǏ]
            t.position = new Vector3(GameMain.gameMain.GetPlayer().GetPosition().x, 0, 0);
        }
        else
        {
            // ������
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

    // �v���C���[������
    public void Fall(float fallPosition, float respawnPosition)
    {
        fallTime = Time.time;
        this.fallPosition = fallPosition;
        this.respawnPosition = respawnPosition;
        respawnTime = (respawnPosition - fallPosition) / 15f + 0.5f;
    }

    // �v���C���[���Q�[���I�[�o�[
    public void GameOver(float deathPosition)
    {
        t.position = new Vector3(deathPosition, 0, 0);
    }
}
