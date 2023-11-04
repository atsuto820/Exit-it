using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    public static GameMain gameMain;

    // デバッグ用
    public float timeScale;         // ゲームの再生速度

    // プレイヤー各種パラメータ
    public float maxSpeed;          // 最高速度
    public float accelerationSpeed; // 加速度
    public float moveSpeed;         // 左右移動速度
    public float jump;              // ジャンプ加速度
    public float dashSpeed;         // ダッシュ時速度

    // ヒエラルキーオブジェクト
    public CourseAssets courseAssets;
    public Player3D player3D;
    public Player2D player2D;
    public CameraAnim cameraAnim;
    public UIAnim uiAnim;
    public PlayerChaser playerChaser;
    public Fire fire;
    public RectTransform canvasArea;    // UI表示領域

    // 高専祭限定使用ヒエラルキーオブジェクト
    public TMP_InputField inputField;   // 名前入力欄
    public RankingObj robj;             // RankingObjプレハブ
    public RankingObj[] robjs;          // 1位～3位のRankingObjプレハブ
    public Transform content;           // RankingObjの親
    public GameObject NoRankText;       // 「プレイした人がいない」表示
    public GameObject saveButton, saveButton_invalid;   // スコア記録ボタン

    public AudioSource sfx;
    public AudioClip[] clips;   // 効果音

    public TextMeshPro scoreText;   // スコアテキスト

    private Course previousCourse;          // 前のコース
    private Course currentCourse;           // 現在のコース
    private Course nextCourse;              // 次のコース

    private bool is3D;  // 3Dモードか
    private float score;  // スコア
    private int nextCamera;     // 次のCourse.cameraMovePlaceのインデックス (-1 : null)
    private int nextBack;       // 次のCource.Back3DPlaceのインデックス (-1 : null)
    private float baseX;        // 現在のコースの始点x座標
    private int previousCourseNum;  // 前のコースの番号

    public bool IsStop { get; private set; }        // ゲーム開始前orゲームオーバー
    public bool IsGameOver { get; private set; }    // ゲームオーバーか
    public float Speed { get; private set; }

    private float switchDimensionTime;  // 最後に次元切り替えを行った時間
    private float fallTime;         // 落下時間 (-1 : 落下していない)
    private bool isRespawnPrepare;  // リスポーン準備中 (落下後釣られる ～ リスポーン)
    private Vector3 fallPosition, respawnPosition;  // 落下位置、リスポーン位置

    /// <summary>
    /// 現在の次元のプレイヤーを返す
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

        // スタートのコースを生成
        currentCourse = Instantiate(courseAssets.courseStart, Vector3.zero, Quaternion.identity);
        nextCamera = currentCourse.cameraMovePosition.Length == 0 ? -1 : 0;
        nextBack = currentCourse.Back3DPosition.Length == 0 ? -1 : 0;
        baseX = 0;

        // スタートの次のコースも生成
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
                obj.rank.text = (i + 1) + "位";
                obj.playerName.text = Title.names[i];
                obj.score.text = Title.scores[i] + "点";
            }
        }
    }

    private void Update()
    {
        if (IsStop)
        {
            return;
        }

        // ゲームスピードが加速していく
        if(Time.timeScale > 0)
        {
            Speed += 0.003f * Time.deltaTime;
            Time.timeScale = Speed * timeScale;
        }

        // 落下判定
        if (fallTime == -1 && GetPlayer().transform.position.y < -5)
        {
            fallTime = Time.time;
            isRespawnPrepare = false;

            // 落ちた地点から最も近いリスポーン地点を見つける
            fallPosition = GetPlayer().transform.position;
            respawnPosition = new Vector3(baseX + currentCourse.length, 3, 0);
            float x = fallPosition.x - baseX;   // 落下地点 (コース基準)
            for (int i = 0; i < currentCourse.respawnX.Length; i++)
            {
                if(x < currentCourse.respawnX[i])
                {
                    // より近いリスポーン地点
                    respawnPosition = new Vector3(baseX + currentCourse.respawnX[i], 3, currentCourse.respawnZ[i]);
                    break;
                }
            }
            GetPlayer().Fall(respawnPosition, fallPosition);
            playerChaser.Fall(x + baseX, respawnPosition.x);
        }

        if(fallTime != -1)
        {
            // 落下後
            if (Time.time - fallTime > 1.5f + (respawnPosition.x - fallPosition.x) / 15f)
            {
                // リスポーン
                fallTime = -1;
                GetPlayer().Respawn();
            }
            else
            {
                if (Time.time - fallTime > 1 && !isRespawnPrepare)
                {
                    // リスポーン準備 (釣られている状態)
                    isRespawnPrepare = true;
                    fire.RespawnPrepare(respawnPosition.x - fallPosition.x);
                    GetPlayer().RespawnPrepare();
                }
            }
        }
        else
        {
            // 落下後でない

            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && Time.time > switchDimensionTime + 1.5f)
            {
                // 次元切り替えアニメーション開始
                switchDimensionTime = Time.time;
                PlaySfx(4);
                cameraAnim.anim.SetBool("is3D", !cameraAnim.anim.GetBool("is3D"));
            }

            // コースを更新
            if (GetPlayer().transform.position.x > baseX + currentCourse.length)
            {
                if (previousCourse != null)
                {
                    // 前のコースを破棄
                    Destroy(previousCourse.gameObject);
                }
                previousCourse = currentCourse;
                currentCourse = nextCourse;

                nextCamera = currentCourse.cameraMovePosition.Length == 0 ? -1 : 0;
                nextBack = currentCourse.Back3DPosition.Length == 0 ? -1 : 0;
                baseX = currentCourse.transform.position.x;

                nextCourse = GetNewCourse();
            }

            // カメラ移動
            if (nextCamera != -1 && GetPlayer().transform.position.x > baseX + currentCourse.cameraMovePosition[nextCamera])
            {
                cameraAnim.anim.SetInteger("Position", currentCourse.cameraMovePlace[nextCamera]);

                nextCamera++;
                if (currentCourse.cameraMovePosition.Length == nextCamera)
                {
                    nextCamera = -1;
                }
            }

            // 2D -> 3D の時のz座標
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

    // 新しいコースを生成し返す
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

    // 次元切り替え
    public void SwitchDimension()
    {
        is3D = !is3D;
        if (is3D)
        {
            // 2D -> 3D
            player2D.gameObject.SetActive(false);
            player3D.gameObject.SetActive(true);

            // 位置、速度を反映
            player2D.Reflect(player3D, nextBack < 1 ? 0 : currentCourse.Back3DPlace[nextBack - 1]);
        }
        else
        {
            // 3D -> 2D
            player2D.gameObject.SetActive(true);
            player3D.gameObject.SetActive(false);

            // 位置、速度を反映
            player3D.Reflect(player2D);
        }
        playerChaser.SwitchDimension();
    }

    // スコア
    public void AddScore(float plus)
    {
        score += plus;
        scoreText.text = ((int)score).ToString();
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    /// <param name="deathPosition">死亡位置x座標</param>
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
    /// ポーズ処理を行う。ゲームの動きを止め、メニューを表示
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        fire.fireSound.Pause();
        uiAnim.anim.SetTrigger("Pause");
    }

    /// <summary>
    /// ポーズ中ならポーズ解除、ゲームオーバー後ならリトライ
    /// </summary>
    public void ContinueOrRetry()
    {
        if(Time.timeScale == 0)
        {
            // ポーズ中 -> ポーズ解除
            fire.fireSound.UnPause();
            Time.timeScale = Speed;
            uiAnim.anim.SetTrigger("Pause");
        }
        else if (IsGameOver)
        {
            // ゲームオーバー後 -> リトライ
            uiAnim.anim.SetTrigger("Retry");
        }
    }

    /// <summary>
    /// スコアを保存し、ランキングに追加する(高専祭限定使用)
    /// </summary>
    public void SaveRanking()
    {
        if(inputField.text.Length == 0 || inputField.text.Length > 8)
        {
            // 不正な入力
            return;
        }

        saveButton.SetActive(false);
        saveButton_invalid.SetActive(true);
        int rank = 1;  // 順位
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
        // ランキングをクリアし、生成しなおす
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < Title.names.Count; i++)
        {
            RankingObj obj = Instantiate(i < 3 ? robjs[i] : robj, Vector3.zero, Quaternion.identity, content);
            obj.rank.text = (i + 1) + "位";
            obj.playerName.text = Title.names[i];
            obj.score.text = Title.scores[i] + "点";
        }

        // 外部テキストファイルに保存
        StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/score.csv");
        for (int i = 0; i < Title.names.Count; i++)
        {
            sw.WriteLine(Title.names[i] + "," + Title.scores[i]);
        }
        sw.Close();
    }

    /// <summary>
    /// 名前入力欄の文字数を制限する(高専祭限定使用)
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
    /// 効果音再生
    /// </summary>
    public void PlaySfx(int num)
    {
        sfx.PlayOneShot(clips[num]);
    }
}
