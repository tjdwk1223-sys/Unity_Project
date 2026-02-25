using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("게임 시작 시 자동 출력될 대사")]
    [TextArea(2, 5)]
    // ★ 유니티 인스펙터가 꼬이든 말든 무조건 이 대사가 먼저 뜨도록 코드에 고정했습니다!
    public string[] openingSentences = new string[] {
        "어느날 눈을떳는대 갑자기 이곳에 떨어졋다",
        "이목검은 뭐지?? 나는검은 처음써보는대",
        "어저기 빛이보이내 저쪽으로가볼까 ?",
        "아저기 돌도한번 봐야겠다 신기하게 생겻네"
    };

    private string[] currentSentences;
    private int currentSentenceIndex = 0;
    public float typingSpeed = 0.05f;
    private bool isTyping = false;
    private bool isDialogueActive = false;

    void Start()
    {
        // 게임 시작 시 코드로 박아둔 대사가 무조건 재생됩니다!
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
        // 마우스 클릭이나 스페이스바로 넘기기
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