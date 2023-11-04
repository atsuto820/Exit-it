using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    public static GameMain gameMain;

    // �f�o�b�O�p
    public float timeScale;         // �Q�[���̍Đ����x

    // �v���C���[�e��p�����[�^
    public float maxSpeed;          // �ō����x
    public float accelerationSpeed; // �����x
    public float moveSpeed;         // ���E�ړ����x
    public float jump;              // �W�����v�����x
    public float dashSpeed;         // �_�b�V�������x

    // �q�G�����L�[�I�u�W�F�N�g
    public CourseAssets courseAssets;
    public Player3D player3D;
    public Player2D player2D;
    public CameraAnim cameraAnim;
    public UIAnim uiAnim;
    public PlayerChaser playerChaser;
    public Fire fire;
    public RectTransform canvasArea;    // UI�\���̈�

    // ����Ռ���g�p�q�G�����L�[�I�u�W�F�N�g
    public TMP_InputField inputField;   // ���O���͗�
    public RankingObj robj;             // RankingObj�v���n�u
    public RankingObj[] robjs;          // 1�ʁ`3�ʂ�RankingObj�v���n�u
    public Transform content;           // RankingObj�̐e
    public GameObject NoRankText;       // �u�v���C�����l�����Ȃ��v�\��
    public GameObject saveButton, saveButton_invalid;   // �X�R�A�L�^�{�^��

    public AudioSource sfx;
    public AudioClip[] clips;   // ���ʉ�

    public TextMeshPro scoreText;   // �X�R�A�e�L�X�g

    private Course previousCourse;          // �O�̃R�[�X
    private Course currentCourse;           // ���݂̃R�[�X
    private Course nextCourse;              // ���̃R�[�X

    private bool is3D;  // 3D���[�h��
    private float score;  // �X�R�A
    private int nextCamera;     // ����Course.cameraMovePlace�̃C���f�b�N�X (-1 : null)
    private int nextBack;       // ����Cource.Back3DPlace�̃C���f�b�N�X (-1 : null)
    private float baseX;        // ���݂̃R�[�X�̎n�_x���W
    private int previousCourseNum;  // �O�̃R�[�X�̔ԍ�

    public bool IsStop { get; private set; }        // �Q�[���J�n�Oor�Q�[���I�[�o�[
    public bool IsGameOver { get; private set; }    // �Q�[���I�[�o�[��
    public float Speed { get; private set; }

    private float switchDimensionTime;  // �Ō�Ɏ����؂�ւ����s��������
    private float fallTime;         // �������� (-1 : �������Ă��Ȃ�)
    private bool isRespawnPrepare;  // ���X�|�[�������� (������ނ��� �` ���X�|�[��)
    private Vector3 fallPosition, respawnPosition;  // �����ʒu�A���X�|�[���ʒu

    /// <summary>
    /// ���݂̎����̃v���C���[��Ԃ�
    /// </summary>
    public Player GetPlayer()
    {
        return is3D ? player3D : player2D;
    }

    private void Awake()
    {
        gameMain = this;
        Time.timeScale = timeScale == 0 ? 1 : timeScale;
        Speed = 0.8f;
        IsStop = true;
        IsGameOver = false;
        canvasArea.sizeDelta = new Vector2(10 * ((float)Screen.width / Screen.height), 10);
    }

    private void Start()
    {
        is3D = true;

        // �X�^�[�g�̃R�[�X�𐶐�
        currentCourse = Instantiate(courseAssets.courseStart, Vector3.zero, Quaternion.identity);
        nextCamera = currentCourse.cameraMovePosition.Length == 0 ? -1 : 0;
        nextBack = currentCourse.Back3DPosition.Length == 0 ? -1 : 0;
        baseX = 0;

        // �X�^�[�g�̎��̃R�[�X������
        previousCourseNum = -1;
        nextCourse = GetNewCourse();

        fallTime = -1;

        if (Title.isKosensai)
        {
            if (Title.names.Count > 0)
            {
                NoRankText.SetActive(false);
            }
            for (int i = 0; i < Title.names.Count; i++)
            {
                RankingObj obj = Instantiate(i < 3 ? robjs[i] : robj, Vector3.zero, Quaternion.identity, content);
                obj.rank.text = (i + 1) + "��";
                obj.playerName.text = Title.names[i];
                obj.score.text = Title.scores[i] + "�_";
            }
        }
    }

    private void Update()
    {
        if (IsStop)
        {
            return;
        }

        // �Q�[���X�s�[�h���������Ă���
        if(Time.timeScale > 0)
        {
            Speed += 0.003f * Time.deltaTime;
            Time.timeScale = Speed * timeScale;
        }

        // ��������
        if (fallTime == -1 && GetPlayer().transform.position.y < -5)
        {
            fallTime = Time.time;
            isRespawnPrepare = false;

            // �������n�_����ł��߂����X�|�[���n�_��������
            fallPosition = GetPlayer().transform.position;
            respawnPosition = new Vector3(baseX + currentCourse.length, 3, 0);
            float x = fallPosition.x - baseX;   // �����n�_ (�R�[�X�)
            for (int i = 0; i < currentCourse.respawnX.Length; i++)
            {
                if(x < currentCourse.respawnX[i])
                {
                    // ���߂����X�|�[���n�_
                    respawnPosition = new Vector3(baseX + currentCourse.respawnX[i], 3, currentCourse.respawnZ[i]);
                    break;
                }
            }
            GetPlayer().Fall(respawnPosition, fallPosition);
            playerChaser.Fall(x + baseX, respawnPosition.x);
        }

        if(fallTime != -1)
        {
            // ������
            if (Time.time - fallTime > 1.5f + (respawnPosition.x - fallPosition.x) / 15f)
            {
                // ���X�|�[��
                fallTime = -1;
                GetPlayer().Respawn();
            }
            else
            {
                if (Time.time - fallTime > 1 && !isRespawnPrepare)
                {
                    // ���X�|�[������ (�ނ��Ă�����)
                    isRespawnPrepare = true;
                    fire.RespawnPrepare(respawnPosition.x - fallPosition.x);
                    GetPlayer().RespawnPrepare();
                }
            }
        }
        else
        {
            // ������łȂ�

            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && Time.time > switchDimensionTime + 1.5f)
            {
                // �����؂�ւ��A�j���[�V�����J�n
                switchDimensionTime = Time.time;
                PlaySfx(4);
                cameraAnim.anim.SetBool("is3D", !cameraAnim.anim.GetBool("is3D"));
            }

            // �R�[�X���X�V
            if (GetPlayer().transform.position.x > baseX + currentCourse.length)
            {
                if (previousCourse != null)
                {
                    // �O�̃R�[�X��j��
                    Destroy(previousCourse.gameObject);
                }
                previousCourse = currentCourse;
                currentCourse = nextCourse;

                nextCamera = currentCourse.cameraMovePosition.Length == 0 ? -1 : 0;
                nextBack = currentCourse.Back3DPosition.Length == 0 ? -1 : 0;
                baseX = currentCourse.transform.position.x;

                nextCourse = GetNewCourse();
            }

            // �J�����ړ�
            if (nextCamera != -1 && GetPlayer().transform.position.x > baseX + currentCourse.cameraMovePosition[nextCamera])
            {
                cameraAnim.anim.SetInteger("Position", currentCourse.cameraMovePlace[nextCamera]);

                nextCamera++;
                if (currentCourse.cameraMovePosition.Length == nextCamera)
                {
                    nextCamera = -1;
                }
            }

            // 2D -> 3D �̎���z���W
            if (nextBack != -1 && GetPlayer().transform.position.x > baseX + currentCourse.Back3DPosition[nextBack])
            {
                nextBack++;
                if (currentCourse.Back3DPosition.Length == nextBack)
                {
                    nextBack = -1;
                }
            }
        }
    }

    // �V�����R�[�X�𐶐����Ԃ�
    private Course GetNewCourse()
    {
        if (courseAssets.debug == -1)
        {
            int newCourseNum = Random.Range(0, courseAssets.courses.Length - 1);
            if(previousCourseNum <= newCourseNum)
            {
                newCourseNum++;
            }

            int[] junban = { -1, 0, 2, 12, 1, 7, 4, 14, 10, 11, 3, 13, 5, 5 };
            for (int i = 0; i < junban.Length; i++)
            {
                if(junban[i] == previousCourseNum)
                {
                    newCourseNum = junban[i + 1];
                    break;
                }
            }

            previousCourseNum = newCourseNum;
            return Instantiate(courseAssets.courses[newCourseNum],
                new Vector3(baseX + currentCourse.length, 0, 0), Quaternion.identity);
        }
        else
        {
            return Instantiate(courseAssets.courses[courseAssets.debug],
                new Vector3(baseX + currentCourse.length, 0, 0), Quaternion.identity);
        }
    }

    // �����؂�ւ�
    public void SwitchDimension()
    {
        is3D = !is3D;
        if (is3D)
        {
            // 2D -> 3D
            player2D.gameObject.SetActive(false);
            player3D.gameObject.SetActive(true);

            // �ʒu�A���x�𔽉f
            player2D.Reflect(player3D, nextBack < 1 ? 0 : currentCourse.Back3DPlace[nextBack - 1]);
        }
        else
        {
            // 3D -> 2D
            player2D.gameObject.SetActive(true);
            player3D.gameObject.SetActive(false);

            // �ʒu�A���x�𔽉f
            player3D.Reflect(player2D);
        }
        playerChaser.SwitchDimension();
    }

    // �X�R�A
    public void AddScore(float plus)
    {
        score += plus;
        scoreText.text = ((int)score).ToString();
    }

    /// <summary>
    /// �Q�[���I�[�o�[����
    /// </summary>
    /// <param name="deathPosition">���S�ʒux���W</param>
    public void GameOver(float deathPosition)
    {
        IsStop = true;
        IsGameOver = true;
        cameraAnim.anim.SetInteger("Position", 0);
        uiAnim.anim.SetTrigger("Gameover");
        playerChaser.GameOver(deathPosition);
        fire.GameOver(deathPosition);
    }

    /// <summary>
    /// �|�[�Y�������s���B�Q�[���̓������~�߁A���j���[��\��
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        fire.fireSound.Pause();
        uiAnim.anim.SetTrigger("Pause");
    }

    /// <summary>
    /// �|�[�Y���Ȃ�|�[�Y�����A�Q�[���I�[�o�[��Ȃ烊�g���C
    /// </summary>
    public void ContinueOrRetry()
    {
        if(Time.timeScale == 0)
        {
            // �|�[�Y�� -> �|�[�Y����
            fire.fireSound.UnPause();
            Time.timeScale = Speed;
            uiAnim.anim.SetTrigger("Pause");
        }
        else if (IsGameOver)
        {
            // �Q�[���I�[�o�[�� -> ���g���C
            uiAnim.anim.SetTrigger("Retry");
        }
    }

    /// <summary>
    /// �X�R�A��ۑ����A�����L���O�ɒǉ�����(����Ռ���g�p)
    /// </summary>
    public void SaveRanking()
    {
        if(inputField.text.Length == 0 || inputField.text.Length > 8)
        {
            // �s���ȓ���
            return;
        }

        saveButton.SetActive(false);
        saveButton_invalid.SetActive(true);
        int rank = 1;  // ����
        for (int i = 0; i < Title.names.Count; i++)
        {
            if(score < Title.scores[Title.scores.Count - i - 1])
            {
                rank = Title.scores.Count - i + 1;
                break;
            }
        }
        Title.names.Insert(rank - 1, inputField.text);
        Title.scores.Insert(rank - 1, (int)score);

        NoRankText.SetActive(false);
        // �����L���O���N���A���A�������Ȃ���
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < Title.names.Count; i++)
        {
            RankingObj obj = Instantiate(i < 3 ? robjs[i] : robj, Vector3.zero, Quaternion.identity, content);
            obj.rank.text = (i + 1) + "��";
            obj.playerName.text = Title.names[i];
            obj.score.text = Title.scores[i] + "�_";
        }

        // �O���e�L�X�g�t�@�C���ɕۑ�
        StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/score.csv");
        for (int i = 0; i < Title.names.Count; i++)
        {
            sw.WriteLine(Title.names[i] + "," + Title.scores[i]);
        }
        sw.Close();
    }

    /// <summary>
    /// ���O���͗��̕������𐧌�����(����Ռ���g�p)
    /// </summary>
    public void CheckTextCount()
    {
        if (inputField.text.Length > 8)
        {
            inputField.text = inputField.text[..8];
        }
    }

    public void AnimationStart()
    {
        cameraAnim.anim.SetTrigger("Start");
        player3D.anim.SetTrigger("Start");
    }

    public void FireAnimation()
    {
        fire.gameObject.SetActive(true);
        fire.fireSound.Play();
    }

    public void Ready()
    {
        uiAnim.anim.SetTrigger("Ready");
    }

    public void GameStart()
    {
        player3D.anim.SetInteger("State", 1);
        IsStop = false;
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(Title.isKosensai ? "GameForKosensai" : "Game");
    }

    public void End()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(Title.isKosensai ? "TitleForKosensai" : "Title");
    }

    /// <summary>
    /// ���ʉ��Đ�
    /// </summary>
    public void PlaySfx(int num)
    {
        sfx.PlayOneShot(clips[num]);
    }
}
