using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 씬 확인을 위해 필수!

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
    private int startFrame = -1;

    // ★★★ [여기서부터 중요!] 씬이 바뀔 때마다 감시합니다 ★★★
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로딩이 끝날 때마다 이 함수가 자동으로 실행됩니다!
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ★ 0번(오프닝)이나 엔딩씬이면 대화창 끄기
        if (scene.buildIndex == 0 || scene.name == "EndingScene")
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            return;
        }

        // ★ 1번(Map1)에 도착했을 때만 오프닝 대사 시작!
        if (scene.buildIndex == 1) // 빌드 세팅의 Map1 번호가 1번이어야 합니다
        {
            if (openingSentences != null && openingSentences.Length > 0)
            {
                StartDialogue(openingSentences);
            }
        }
    }

    void Start()
    {
        // Start에서는 아무것도 안 해도 됩니다. (OnSceneLoaded가 대신 해줌)
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive && Time.frameCount > startFrame)
        {
            // 마우스 클릭, 스페이스바, E키 모두 넘기기 가능
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                NextSentence();
            }
        }
    }

    public void StartDialogue(string[] newSentences)
    {
        if (newSentences == null || newSentences.Length == 0) return;
        if (dialoguePanel == null) return;

        startFrame = Time.frameCount;
        currentSentences = newSentences;
        currentSentenceIndex = 0;
        dialoguePanel.SetActive(true);
        isDialogueActive = true;

        StopAllCoroutines(); // 혹시 꼬일까봐 초기화
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