using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    // ����N�����̂�false
    public static bool noFirst;

    public Animator anim;
    public AudioSource music;
    public AudioClip[] clips;

    // �����L���O (����Ռ���g�p)
    public static bool isKosensai;  // ����Ճ��[�h�� (�X�R�A�L�^�A�����L���O�@�\����)
    public RankingObj robj;         // RankingObj�v���n�u
    public RankingObj[] robjs;      // 1�ʁ`3�ʂ�RankingObj�v���n�u
    public Transform content;       // RankingObj�̐e
    public GameObject NoRankText;   // �u�v���C�����l�����Ȃ��v�\��
    public static List<string> names;   // �L�^���ꂽ���O
    public static List<int> scores;     // �L�^���ꂽ�X�R�A

    public void Awake()
    {
        // ����N�����͏�ʐ؂�ւ��A�j���[�V�����Ȃ�
        anim.SetBool("NoFirst", noFirst);
    }

    public void Start()
    {
        isKosensai = false;
        if (isKosensai)
        {
            if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.L))
            {
                // �X�R�A�f�[�^�폜�R�}���h
                names = new List<string>();
                scores = new List<int>();
            }
            else
            {
                if (names == null)
                {
                    // �X�R�A�f�[�^�t�@�C���̓ǂݍ��� (����N����)
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
                // RankingObj�̔z�u
                RankingObj obj = Instantiate(i < 3 ? robjs[i] : robj, Vector3.zero, Quaternion.identity, content);
                obj.rank.text = (i + 1) + "��";
                obj.playerName.text = names[i];
                obj.score.text = scores[i] + "�_";
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
