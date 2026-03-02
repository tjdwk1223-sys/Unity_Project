using UnityEngine;

public class CooltimeStone : MonoBehaviour
{
    [Header("설정")]
    public float detectRange = 5.0f; // 감지 거리 (안 뜨면 이 숫자를 더 키우세요)
    public GameObject interactUI;    // "E 상호작용" 글자 오브젝트

    private KikiController kiki;
    private DialogueManager dialogue;

    void Start()
    {
        // 씬에서 플레이어와 대화창 매니저를 찾습니다.
        kiki = FindObjectOfType<KikiController>();
        dialogue = FindObjectOfType<DialogueManager>();

        // 처음에는 상호작용 글자를 꺼둡니다.
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        if (kiki == null || dialogue == null) return;

        // 플레이어와의 거리를 계산합니다.
        float dist = Vector3.Distance(transform.position, kiki.transform.position);

        if (dist <= detectRange)
        {
            // 거리 안이면 글자를 보여줍니다.
            if (interactUI != null) interactUI.SetActive(true);

            // E키를 누르면 각 스킬의 기본 쿨타임을 대화창에 띄웁니다.
            if (Input.GetKeyDown(KeyCode.E))
            {
                string info = "[ 스킬별 기본 쿨타임 정보 ]\n" +
                    "X 스킬 (땅 후려찍기): " + GetCool(kiki.xSkillData) + "초\n" +
                    "C 스킬: " + GetCool(kiki.cSkillData) + "초\n" +
                    "Q 스킬: " + GetCool(kiki.qSkillData) + "초\n" +
                    "F 스킬: " + GetCool(kiki.fSkillData) + "초\n" +
                    "V 스킬 (Shiny Slash): " + GetCool(kiki.vSkillData) + "초";

                dialogue.StartDialogue(new string[] { info });
            }
        }
        else
        {
            // 멀어지면 글자를 숨깁니다.
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    // SkillData에서 설정된 쿨타임 값을 가져오는 함수입니다.
    float GetCool(SkillData data)
    {
        return data != null ? data.cooldownTime : 5.0f;
    }
}