using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlay : MonoBehaviour
{
    public Animator cameraAnim;
    public AudioSource music, sfx;
    public AudioClip[] clips;

    public void SetTrigger(string name)
    {
        cameraAnim.SetTrigger(name);
    }

    public void SwitchBool(string name)
    {
        cameraAnim.SetBool(name, !cameraAnim.GetBool(name));
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
        sfx.PlayOneShot(clips[num]);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(Title.isKosensai ? "GameForKosensai" : "Game");
    }
}
