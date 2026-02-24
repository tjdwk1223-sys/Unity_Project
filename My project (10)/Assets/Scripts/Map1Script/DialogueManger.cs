using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("게임 시작 시 자동 출력될 대사")]
    [TextArea(2, 5)]
    public string[] openingSentences; // 여기에 오프닝 대사를 적습니다.

    private string[] currentSentences;
    private int currentSentenceIndex = 0;
    public float typingSpeed = 0.05f;
    private bool isTyping = false;
    private bool isDialogueActive = false;

    void Start()
    {
        // 게임 시작 시 openingSentences에 적힌 내용이 있다면 바로 재생!
        if (openingSentences != null && openingSentences.Length > 0)
        {
            StartDialogue(openingSentences);
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isDialogueActive && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            NextSentence();
        }
    }

    public void StartDialogue(string[] newSentences)
    {
        currentSentences = newSentences;
        currentSentenceIndex = 0;
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        StartCoroutine(TypeSentence(currentSentences[currentSentenceIndex]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void NextSentence()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentSentences[currentSentenceIndex];
            isTyping = false;
            return;
        }

        currentSentenceIndex++;

        if (currentSentenceIndex < currentSentences.Length)
        {
            StartCoroutine(TypeSentence(currentSentences[currentSentenceIndex]));
        }
        else
        {
            dialoguePanel.SetActive(false);
            isDialogueActive = false;
        }
    }
}