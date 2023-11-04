using UnityEngine;

public class UIAnim : MonoBehaviour
{
    public Animator anim;
    public AudioSource music;

    public void AnimationStart()
    {
        GameMain.gameMain.AnimationStart();
    }

    public void GameStart()
    {
        GameMain.gameMain.GameStart();
    }

    public void Retry()
    {
        GameMain.gameMain.Retry();
    }

    public void End()
    {
        GameMain.gameMain.End();
    }

    public void PlayMusic()
    {
        music.Play();
    }

    public void PauseMusic()
    {
        music.Pause();
    }

    public void UnPauseMusic()
    {
        music.UnPause();
    }

    public void StopMusic()
    {
        music.Stop();
        GameMain.gameMain.fire.fireSound.Stop();
    }

    public void PlaySfx(int num)
    {
        GameMain.gameMain.PlaySfx(num);
    }
}
