using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject musicManager;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Music") == null)
        {
            volumeSlider.value = 0.5f;
            musicManager = Instantiate(musicManager);
            musicManager.GetComponent<AudioSource>().volume = volumeSlider.value;
            DontDestroyOnLoad(musicManager);
        }
        else
        {
            musicManager = GameObject.FindGameObjectWithTag("Music");
            volumeSlider.value = musicManager.GetComponent<AudioSource>().volume;
        }
    }

    public void SetMusicVolume()
    {
        musicManager.GetComponent<AudioSource>().volume = volumeSlider.value;
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
