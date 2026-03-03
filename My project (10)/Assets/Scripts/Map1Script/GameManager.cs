using UnityEngine;
using UnityEngine.SceneManagement;
using VariableInventorySystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("인벤토리 데이터 금고")]
    public StandardStashViewData PlayerInventory;

    [Header("--- [스킬 해금 상태 관리] ---")]
    public bool isXSkillUpgraded = false;
    public bool isVSkillUpgraded = false;
    public bool isAuraUnlocked = false;

    [Header("--- [퀘스트 진행 아이템] ---")]
    public bool hasMagicItem = false;

    // ★★★ [신규 기능] 히든 스코어 시스템 ★★★
    [Header("--- [히든 스코어] ---")]
    public int totalScore = 0; // 플레이어는 모르는 점수

    public void AddScore(int score)
    {
        totalScore += score;
        Debug.Log($"[히든 스코어] {score}점 획득! (현재 총점: {totalScore})");
    }
    // ★★★★★★★★★★★★★★★★★★★★★★★★★

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            isXSkillUpgraded = false;
            isVSkillUpgraded = false;
            isAuraUnlocked = false;
            hasMagicItem = false;
            totalScore = 0; // 점수 초기화

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
    }
}