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
            WinText.text = "�� ������ ���� ��������� PWGood!";
        else if (ScoreManager.pwgoodScore < ScoreManager.viewersScore)
            WinText.text = "�� ������ ���� ��������� �������!";
        else
            WinText.text = "����������! �� ������ ���� ��������� ������! pwgood3";
        PWGoodScoreText.text = "������� ��������� �� " + ScoreManager.pwgoodScore.ToString() + " ��������";
        ViewersScoreText.text = "�������� ��������� �� " + ScoreManager.viewersScore.ToString() + " ��������";
        yield return new WaitForSeconds(time);
        menuButton.SetActive(true);
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene(0);
    }
}
