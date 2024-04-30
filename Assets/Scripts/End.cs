using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    [SerializeField] private TMP_Text WinText;
    [SerializeField] private TMP_Text PWGoodScoreText;
    [SerializeField] private TMP_Text ViewersScoreText;
    [SerializeField] private GameObject menuButton;

    void Start()
    {
        StartCoroutine(Wait(7));
    }

    IEnumerator Wait(float time)
    {
        menuButton.SetActive(false);
        yield return new WaitForSeconds(time);
        if (ScoreManager.pwgoodScore > ScoreManager.viewersScore)
            WinText.text = "По итогам игры побеждает PWGood!";
        else if (ScoreManager.pwgoodScore < ScoreManager.viewersScore)
            WinText.text = "По итогам игры побеждают зрители!";
        else
            WinText.text = "Невероятно! По итогам игры побеждает дружба! pwgood3";
        PWGoodScoreText.text = "Ответил правильно на " + ScoreManager.pwgoodScore.ToString() + " вопросов";
        ViewersScoreText.text = "Ответили правильно на " + ScoreManager.viewersScore.ToString() + " вопросов";
        yield return new WaitForSeconds(time);
        menuButton.SetActive(true);
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene(0);
    }
}
