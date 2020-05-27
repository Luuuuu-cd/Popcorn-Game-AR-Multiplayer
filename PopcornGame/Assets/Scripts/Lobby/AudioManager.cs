using UnityEngine;

public class AudioManager : MonoBehaviour
{

    //Manage all the sound effects in the game

    public AudioSource bgm_audioSource;
    public AudioSource buttonClicked_audioSource;
    public AudioSource popcornCollectionSource;
    public AudioSource gameOverSource;
    public AudioSource countDownSource;

    //Make this a singleton and make sure it's never destroyed
    public static AudioManager _instance{ get; set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(gameObject);
        }
    }

    //Methods to play or stop specific sound effects
    #region Sound Effects Methods

    public void PlayBGM()
    {
        if (bgm_audioSource.isPlaying) return;
        bgm_audioSource.Play();
    }

    public void StopBGM()
    {
        bgm_audioSource.Stop();
    }

    public void PlayButtonClickedSound()
    {
        if (buttonClicked_audioSource.isPlaying) return;
        buttonClicked_audioSource.Play();
    }

    public void StopButtonClickedSound()
    {
        buttonClicked_audioSource.Stop();
    }

    public void PlayCollectPopcornSound()
    {
        //if (popcornCollectionSource.isPlaying) return;
        popcornCollectionSource.Play();
    }
    public void StopCollectPopcornSound()
    {
        popcornCollectionSource.Stop();
    }

    public void PlayGameOverSound()
    {
        if (gameOverSource.isPlaying) return;
        gameOverSource.Play();
    }
    public void StopGameOverSound()
    {
        gameOverSource.Stop();
    }
    public void PlayCountDownSound()
    {
        if (countDownSource.isPlaying) return;
        countDownSource.Play();
    }
    public void StopCountDownSound()
    {
        countDownSource.Stop();
    }
    #endregion
}
