using UnityEngine;
using UnityEngine.SceneManagement;
using VariableInventorySystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("인벤토리 데이터 금고")]
    public StandardStashViewData PlayerInventory;

    // --- [히든 퀘스트용 변수] ---
    public bool hasMagicItem = false;
    public bool isVSkillUpgraded = false;
    // ---------------------------------

    private void Awake()
    {
        if (Instance == null)
        {
            // 내가 최초의 매니저(맵1)라면 불사신이 된다!
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 인벤토리 초기화
            PlayerInventory = new StandardStashViewData(8, 16);
        }
        else
        {
            DialogueManager immortalDialogue = Instance.GetComponent<DialogueManager>();
            DialogueManager myDialogue = this.GetComponent<DialogueManager>();

            if (immortalDialogue != null && myDialogue != null)
            {
                // 1. NPC 대화(상호작용)를 위해 끊어진 UI 선(대화창, 텍스트)만 조용히 이어줍니다.
                immortalDialogue.dialoguePanel = myDialogue.dialoguePanel;
                immortalDialogue.dialogueText = myDialogue.dialogueText;

                // ★ [수정됨] 맵2, 맵3으로 넘어갈 때 오프닝 대사를 강제로 트는 코드를 싹 지웠습니다!
                // 이제 게임을 처음 켠 맵1에서만 오프닝이 나오고, 다음 맵부터는 절대 안 나옵니다.

                myDialogue.enabled = false;
            }

            // 헌납을 모두 마쳤으니, 중복된 나는 장렬하게 파괴됨!
            Destroy(gameObject);
        }
    }

    public void NextStage()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 스테이지입니다!");
        }
    }
}