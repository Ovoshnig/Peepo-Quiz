using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject[] TitlePanels;
    [SerializeField] private TMP_Text QuestionNumberText;
    [SerializeField] private TMP_Text QuestionText;
    [SerializeField] private TMP_Text PWGoodText;
    [SerializeField] private TMP_Text ViewersText;
    [SerializeField] private TMP_Text[] AnswerTexts;
    [SerializeField] private TMP_Text StatusText;
    [SerializeField] private TMP_Text PWGoodAnswerText;
    [SerializeField] private TMP_Text ViewersAnswerText;
    [SerializeField] private Image WaitingImage;

    [SerializeField] private float panelWaitingTime;
    [SerializeField] private float thinkWaitingTime;
    [SerializeField] private float answerWaitingTime;
    [SerializeField] private float resultWaitingTime;

    [SerializeField] private Color green;
    [SerializeField] private Color red;

    private List<Question> questions;

    private int categoryIndex;
    private int questionIndex;

    private readonly KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    private int pwgoodAnswer = 5;
    private int pwgoodScore = 0;
    private int viewersAnswersCount = 0;
    private int viewersCommonAnswer = 5;
    private int viewersScore = 0;

    private readonly Dictionary<int, int> frequencyDictionary = new();

    [System.Serializable]
    public class Question
    {
        public string question;
        public string[] answers;
        public int correctIndex;
    }

    [System.Serializable]
    public class QuizData
    {
        public List<Question> questions;
    }

    private void Start()
    {
        StartCoroutine(Quiz());
    }

    private void Update()
    {
        if (ChatManager.IsReadingEnabled)
        {
            for (int i = 0; i < keyCodes.Length; i++) 
            {
                if (Input.GetKey(keyCodes[i]))
                {
                    PWGoodAnswerText.text = "Ответ получен";
                    PWGoodAnswerText.color = Color.gray;
                    pwgoodAnswer = i + 1;
                    break;
                }
            }

            if (ChatManager.ChatDictionary.Count >= 1)
            {
                viewersAnswersCount = ChatManager.ChatDictionary.Count;
                ViewersAnswerText.text = "Получено " + viewersAnswersCount.ToString() + " ответов";
                ViewersAnswerText.color = Color.gray;
            }
        }
    }

    IEnumerator Quiz()
    {
        for (int i = 1; i < TitlePanels.Length; i++)
            TitlePanels[i].SetActive(false);

        for (categoryIndex = 0; categoryIndex < 3; categoryIndex++)
        {
            TitlePanels[categoryIndex].SetActive(true);
            StartCoroutine(QuestionsLoad());
            yield return new WaitForSeconds(panelWaitingTime);
            TitlePanels[categoryIndex].SetActive(false);

            for (questionIndex = 0; questionIndex < 10; questionIndex++) 
            {
                QuestionNumberText.text = $"Вопрос № {questionIndex + 1}";
                QuestionText.text = questions[questionIndex].question;

                for (int i = 0; i < 4; i++)
                {
                    AnswerTexts[i].text = $"{i + 1}. {questions[questionIndex].answers[i]}";
                    AnswerTexts[i].color = Color.black;
                }

                PWGoodText.enabled = false;
                ViewersText.enabled = false;
                StatusText.text = "Время на размышление";
                PWGoodAnswerText.text = "";
                PWGoodAnswerText.color = Color.white;
                ViewersAnswerText.text = "";
                ViewersAnswerText.color = Color.white;
                WaitingImage.fillAmount = 0;

                pwgoodAnswer = 5;
                ChatManager.ChatDictionary.Clear();
                frequencyDictionary.Clear();
                viewersAnswersCount = 0;
                viewersCommonAnswer = 5;

                yield return new WaitForSeconds(thinkWaitingTime);
                StatusText.text = "";
                PWGoodText.enabled = true;
                ViewersText.enabled = true;
                PWGoodAnswerText.text = "Ожидание ответа";
                ViewersAnswerText.text = "Ожидание ответа";

                ChatManager.IsReadingEnabled = true;
                StartCoroutine(ImageFill(answerWaitingTime));
                yield return new WaitForSeconds(answerWaitingTime);
                ChatManager.IsReadingEnabled = false;

                StatusText.text = "Результаты";
                if (ChatManager.ChatDictionary.Count >= 1)
                {
                    foreach (var kvp in ChatManager.ChatDictionary)
                    {
                        string key = kvp.Key;
                        int value = kvp.Value;

                        if (!frequencyDictionary.ContainsKey(value))
                            frequencyDictionary.Add(value, 0);
                        frequencyDictionary[value]++;
                    }
                    viewersCommonAnswer = frequencyDictionary.OrderByDescending(x => x.Value).First().Key;
                }

                if (pwgoodAnswer == questions[questionIndex].correctIndex)
                {
                    PWGoodAnswerText.text = pwgoodAnswer.ToString();
                    pwgoodScore++;
                    PWGoodAnswerText.color = green;
                }
                else
                {
                    if (pwgoodAnswer == 5)
                        PWGoodAnswerText.text = "Нет ответа";
                    else
                        PWGoodAnswerText.text = pwgoodAnswer.ToString();
                    PWGoodAnswerText.color = red;
                }

                if (viewersCommonAnswer == questions[questionIndex].correctIndex)
                {
                    ViewersAnswerText.text = viewersCommonAnswer.ToString();
                    viewersScore++;
                    ViewersAnswerText.color = green;
                }
                else
                {
                    if (viewersCommonAnswer == 5)
                        ViewersAnswerText.text = "Нет ответа";
                    else
                        ViewersAnswerText.text = viewersCommonAnswer.ToString();
                    ViewersAnswerText.color = red;
                }

                AnswerTexts[questions[questionIndex].correctIndex - 1].color = green;

                yield return new WaitForSeconds(resultWaitingTime);
            }
        }
        ScoreManager.pwgoodScore = pwgoodScore;
        ScoreManager.viewersScore = viewersScore;
        SceneManager.LoadScene(3);
    }

    private IEnumerator QuestionsLoad()
    {
        string path = $"Assets/Resources/Questions/question{categoryIndex + 1}.json";
        string json = File.ReadAllText(path);

        // Десериализовать объект JSONData, содержащий список вопросов
        QuizData quizData = JsonUtility.FromJson<QuizData>(json);
        questions = quizData.questions;

        yield return null;
    }

    private IEnumerator ImageFill(float time)
    {
        float t = 0f;
        while (t < time)
        {
            float fillAmount = Mathf.Lerp(1f, 0f, t / time);
            WaitingImage.fillAmount = fillAmount;

            t += Time.deltaTime;
            yield return null;
        }
    }
}
