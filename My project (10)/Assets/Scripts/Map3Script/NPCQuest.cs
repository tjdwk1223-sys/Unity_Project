using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour
{
    [Header("--- UI 연결 ---")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI talkText;
    public GameObject choiceGroup;
    public Button btn1;
    public TextMeshProUGUI btn1Text;
    public Button btn2;
    public TextMeshProUGUI btn2Text;

    [Header("--- 게임 시스템 연결 ---")]
    public Transform player;
    public CoasterSystem coaster;
    public GameObject portalEffect;
    public PlayerAttack playerAttack;

    private int questState = 0;
    private int storyStep = 0;
    private bool isTalking = false;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (portalEffect != null) portalEffect.SetActive(false);
    }

    void Update()
    {
        if (!isTalking && Vector3.Distance(transform.position, player.position) < 3.0f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
        }

        if (isTalking && questState == 2 && !choiceGroup.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                NextStory();
            }
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        dialoguePanel.SetActive(true);
        choiceGroup.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (questState == 0) // 첫 번째 권유
        {
            nameText.text = "소녀 NPC";
            talkText.text = "나는 장사 중이야. 몇 년째 손님이 없네...\n롤러코스터 한번 타볼래? 공짜로 태워줄게!";

            btn1Text.text = "1. 그래 재밌겠다!";
            btn2Text.text = "2. 아니야 됐어.";

            // ★★★ [점수 반영] ★★★
            btn1.onClick.RemoveAllListeners();
            btn1.onClick.AddListener(() => {
                if (GameManager.Instance != null) GameManager.Instance.AddScore(2); // 탄다: +2점
                GoRideCoaster();
            });

            btn2.onClick.RemoveAllListeners();
            btn2.onClick.AddListener(() => {
                if (GameManager.Instance != null) GameManager.Instance.AddScore(10); // 안 탄다: +10점
                StartMainStory();
            });
        }
        else if (questState == 1) // 타고 난 뒤 (한 번 더?)
        {
            nameText.text = "키키";
            talkText.text = "와 너무 재밌다! 한번 더 타도 돼?";
            btn1Text.text = "응 (한번 더 타기)";
            btn2Text.text = "나쁘지 않네 (그만 타고 대화하기)";

            // ★★★ [점수 반영] ★★★
            btn1.onClick.RemoveAllListeners();
            btn1.onClick.AddListener(() => {
                if (GameManager.Instance != null) GameManager.Instance.AddScore(-1); // 또 탄다: -1점 (감점)
                GoRideCoaster();
            });

            btn2.onClick.RemoveAllListeners();
            btn2.onClick.AddListener(() => {
                if (GameManager.Instance != null) GameManager.Instance.AddScore(3); // 그만 탄다: +3점
                StartMainStory();
            });
        }
        else if (questState == 3) // 이미 완료했을 때
        {
            choiceGroup.SetActive(false);
            nameText.text = "소녀 NPC";
            talkText.text = "포탈을 타고 가서 꼭 마왕을 물리쳐줘!";
            Invoke("EndDialogue", 2.0f);
        }
    }

    void GoRideCoaster()
    {
        EndDialogue();
        questState = 1; // 다음엔 '한번 더?' 대사가 나오게 변경
        if (coaster != null) coaster.Ride();
    }

    void StartMainStory()
    {
        questState = 2;
        storyStep = 0;
        NextStory();
    }

    void NextStory()
    {
        choiceGroup.SetActive(false);

        switch (storyStep)
        {
            case 0:
                nameText.text = "키키";
                talkText.text = "그런데 너는 집에 언제 가?";
                break;
            case 1:
                nameText.text = "소녀 NPC";
                talkText.text = "사실 집에 가고 싶어도 못 가...";
                break;
            case 2:
                nameText.text = "키키";
                talkText.text = "왜?";
                break;
            case 3:
                nameText.text = "소녀 NPC";
                talkText.text = "마왕이 마법으로 이 공간을 통째로 가둬버렸어.";
                break;
            case 4:
                nameText.text = "키키";
                talkText.text = "마왕을 처치하면 돌아갈 수 있어?";
                break;
            case 5:
                nameText.text = "소녀 NPC";
                talkText.text = "네가? 미안해... 마왕은 무지하게 강해. 처치해준다면 나도 돌아갈 수 있긴 해.";
                break;
            case 6:
                nameText.text = "키키";
                talkText.text = "내가 처치해줄게!";
                break;
            case 7:
                nameText.text = "소녀 NPC";
                talkText.text = "진짜 고마워! 여기 내가 오래전에 발견한 주문서인데 이거 읽어봐. 대단한 마법이 들어있어.";
                break;
            case 8:
                nameText.text = "시스템";
                talkText.text = "[ 숨겨진 스킬 '검기'를 획득했습니다! ]\n[ 다음 구역으로 가는 차원문이 열렸습니다. ]";
                QuestClear();
                break;
            default:
                questState = 3;  // 완료 상태로 변경
                EndDialogue();
                break;
        }
        storyStep++;
    }

    void QuestClear()
    {
        if (portalEffect != null) portalEffect.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.isAuraUnlocked = true;
        }

        if (playerAttack != null) playerAttack.UnlockAura();
    }

    void EndDialogue()
    {
        isTalking = false;
        dialoguePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}