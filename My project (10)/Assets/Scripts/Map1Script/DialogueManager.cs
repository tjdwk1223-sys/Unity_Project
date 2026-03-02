using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("게임 시작 시 자동 출력될 대사")]
    [TextArea(2, 5)]
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
    public bool isDialogueActive = false;

    // ★ [막타 방어] 대화가 시작된 '그 순간'의 프레임을 기억합니다.
    private int startFrame = -1;

    void Start()
    {
        // ★ [새로 추가] 대화창 패널이 안 채워져 있다면, 에러 내지 말고 그냥 아무것도 하지 마!
        if (dialoguePanel == null)
        {
            return;
        }

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
        // ★ [수정] 대화가 시작된 '그 프레임(startFrame)'에는 E키가 눌려도 넘기지 않습니다.
        // 이렇게 해야 매직스톤의 E와 매니저의 E가 겹치지 않습니다.
        if (isDialogueActive && Time.frameCount > startFrame)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                NextSentence();
            }
        }
    }

    public void StartDialogue(string[] newSentences)
    {
        // ★ [새로 추가] 여기서도 패널이 없으면 시작 자체를 막아버립니다.
        if (dialoguePanel == null) return;

        // 대화 시작 시 현재 프레임 번호를 딱 찍어둡니다.
        startFrame = Time.frameCount;

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