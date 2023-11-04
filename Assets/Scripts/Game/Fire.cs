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

    public float speedDashTime; // �v���C���[���_�b�V�����ɋ������k�߂鑬�x
    public float speedRunTime;  // �v���C���[�������Ă���Ƃ��ɋ������k�߂鑬�x
    public float speedStopTime; // �v���C���[���~�܂��Ă���Ƃ��ɋ������k�߂鑬�x
    public float speedFallTime; // �v���C���[�������`���X�|�[�����Ă���Ƃ��ɋ������k�߂鑬�x
    public float playerBack;    // �v���C���[�𗣂����E

    private float distance;     // �v���C���[�Ƃ̋���
    private float deathPosition;// �Q�[���I�[�o�[x���W

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
                // �Q�[���I�[�o�[��
                // ���S�ʒu�܂ňړ����Ď~�܂�
                distance -= speedStopTime * Time.deltaTime;
                if (distance < -10)
                {
                    distance = -10;
                }

                fireText.text = Mathf.Max(distance, 0).ToString("F1") + "m";                            // ���܂ł̋����\�� (�����_���ʂ܂�)
                t.position = new Vector3(deathPosition - distance - 10, t.position.y, t.position.z);    // �ʒu�X�V
                vignette.intensity.value = Mathf.Max(0, 0.5f - distance / playerBack * 0.4f);           // �r�l�b�g�K�p
                fireSound.volume = Mathf.Max(0, 1f - distance / playerBack * 1f);                       // ���̉���
                return;
            }
            return;
        }

        Player player = GameMain.gameMain.GetPlayer();
        if (player.IsFall())
        {
            // ���� �` ���X�|�[��
            distance -= speedFallTime * Time.deltaTime;
        }
        else
        {
            if (player.IsDash())
            {
                // �_�b�V����
                distance -= speedDashTime * Time.deltaTime;
            }
            else
            {
                distance -= (speedStopTime + (speedRunTime - speedStopTime) * (player.GetSpeed().x / GameMain.gameMain.maxSpeed)) * Time.deltaTime;
            }
            // �v���C���[�𗣂����E�ȏ�ɗ���Ȃ�
            if (distance > playerBack)
            {
                distance = playerBack;
            }
        }

        if(distance < 0)
        {
            distance = 0;
        }
        fireText.text = distance.ToString("F1") + "m";                                                  // ���܂ł̋����\�� (�����_���ʂ܂�)
        t.position = new Vector3(player.GetPosition().x - distance - 10, t.position.y, t.position.z);   // �ʒu�X�V
        vignette.intensity.value = Mathf.Max(0, 0.5f - distance / playerBack * 0.4f);                   // �r�l�b�g�K�p
        fireSound.volume = Mathf.Max(0, 1f - distance / playerBack * 1f);                               // ���̉���

        if (player.IsFall() && distance == 0)
        {
            // ������ɋ�����0�Ȃ�Q�[���I�[�o�[
            player.GameOver();
        }
    }

    /// <summary>
    /// �v���C���[���Q�[���I�[�o�[<br />
    /// �΂̓��������S�n�_�Ŏ~�߂�
    /// </summary>
    /// <param name="deathPosition">���S�n�_x���W</param>
    public void GameOver(float deathPosition)
    {
        this.deathPosition = deathPosition;
    }

    /// <summary>
    /// �v���C���[�����X�|�[������<br />
    /// ���X�|�[�����v���C���[���O�ɏu�Ԉړ�����̂ňʒu�̕␳���s��
    /// </summary>
    /// <param name="move">�u�Ԉړ����鋗��</param>
    public void RespawnPrepare(float move)
    {
        distance += move;
    }
}
