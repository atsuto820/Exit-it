using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    // 初回起動時のみfalse
    public static bool noFirst;

    public Animator anim;
    public AudioSource music;
    public AudioClip[] clips;

    // ランキング (高専祭限定使用)
    public static bool isKosensai;  // 高専祭モードか (スコア記録、ランキング機能あり)
    public RankingObj robj;         // RankingObjプレハブ
    public RankingObj[] robjs;      // 1位～3位のRankingObjプレハブ
    public Transform content;       // RankingObjの親
    public GameObject NoRankText;   // 「プレイした人がいない」表示
    public static List<string> names;   // 記録された名前
    public static List<int> scores;     // 記録されたスコア

    public void Awake()
    {
        // 初回起動時は場面切り替えアニメーションなし
        anim.SetBool("NoFirst", noFirst);
    }

    public void Start()
    {
        isKosensai = false;
        if (isKosensai)
        {
            if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.L))
            {
                // スコアデータ削除コマンド
                names = new List<string>();
                scores = new List<int>();
            }
            else
            {
                if (names == null)
                {
                    // スコアデータファイルの読み込み (初回起動時)
                    names = new List<string>();
                    scores = new List<int>();
                    StreamReader sr = new StreamReader(Application.persistentDataPath + "/score.csv");
                    while (!sr.EndOfStream)
                    {
                        string[] strs = sr.ReadLine().Split(",");
                        names.Add(strs[0]);
                        scores.Add(int.Parse(strs[1]));
                    }
                    sr.Close();
                }
            }

            if (names.Count > 0)
            {
                NoRankText.SetActive(false);
            }
            for (int i = 0; i < names.Count; i++)
            {
                // RankingObjの配置
                RankingObj obj = Instantiate(i < 3 ? robjs[i] : robj, Vector3.zero, Quaternion.identity, content);
                obj.rank.text = (i + 1) + "位";
                obj.playerName.text = names[i];
                obj.score.text = scores[i] + "点";
            }
        }
    }

    public void PlayMusic()
    {
        music.Play();
    }

    public void StopMusic()
    {
        music.Stop();
    }

    public void PlaySfx(int num)
    {
        music.PlayOneShot(clips[num]);
    }

    public void LoadGame()
    {
        noFirst = true;
        SceneManager.LoadScene(isKosensai ? "GameForKosensai" : "Game");
    }

    public void LoadHowToPlay()
    {
        noFirst = true;
        SceneManager.LoadScene("HowToPlay");
    }
}
