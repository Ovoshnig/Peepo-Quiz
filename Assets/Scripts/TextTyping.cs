using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextTyping : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private TMP_Text fullText;
    [SerializeField] private GameObject nextButton;
    private string currentText = "";


    void Start()
    {
        nextButton.SetActive(false);
        StartCoroutine(TypeText());
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(2);
    }
#endif

    IEnumerator TypeText()
    {
        foreach (var letter in fullText.text)
        {
            currentText += letter;
            fullText.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }
        nextButton.SetActive(true);
    }

    public void OnClickContinue()
    {
        SceneManager.LoadScene(2);
    }
}
