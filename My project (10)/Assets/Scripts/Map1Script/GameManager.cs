using UnityEngine;
using UnityEngine.SceneManagement;
using VariableInventorySystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("인벤토리 데이터 금고")]
    public StandardStashViewData PlayerInventory;

    // ▼▼▼ [이 부분이 반드시 있어야 합니다!] ▼▼▼
    [Header("--- [스킬 해금 상태 관리] ---")]
    public bool isXSkillUpgraded = false; // 맵 2: 비석 완료 시 true
    public bool isVSkillUpgraded = false; // 맵 2: 미미 & 매직스톤 완료 시 true
    public bool isAuraUnlocked = false;   // 맵 3: 소녀 NPC 퀘스트 완료 시 true (검기)

    [Header("--- [퀘스트 진행 아이템] ---")]
    public bool hasMagicItem = false;     // 미미에게 받은 마법의 파편 보유 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ★ [핵심] 맵1에서 시작할 때 모든 스킬 강제 잠금! (검기 버그 원천 차단)
            isXSkillUpgraded = false;
            isVSkillUpgraded = false;
            isAuraUnlocked = false;
            hasMagicItem = false;

            PlayerPrefs.DeleteKey("XSkillUpgraded");
            PlayerInventory = new StandardStashViewData(8, 16);
        }
        else
        {
            DialogueManager immortalDialogue = Instance.GetComponent<DialogueManager>();
            DialogueManager myDialogue = this.GetComponent<DialogueManager>();

            if (immortalDialogue != null && myDialogue != null)
            {
                immortalDialogue.dialoguePanel = myDialogue.dialoguePanel;
                immortalDialogue.dialogueText = myDialogue.dialogueText;
                myDialogue.enabled = false;
            }
            Destroy(gameObject);
        }
    }

    public void NextStage()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(nextSceneIndex);
        else Debug.Log("마지막 스테이지입니다!");
    }
}