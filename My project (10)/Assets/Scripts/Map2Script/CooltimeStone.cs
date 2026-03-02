using UnityEngine;

public class CooltimeStone : MonoBehaviour
{
    [Header("설정")]
    public float detectRange = 5.0f;
    public GameObject interactUI;

    private KikiController kiki;
    private DialogueManager dialogue;

    void Start()
    {
        // 처음에는 상호작용 글자를 꺼둡니다.
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        // [핵심 해결법] 맵 이동 때문에 늦게 도착할 수 있으니, kiki나 dialogue가 없으면 계속 다시 찾습니다!
        if (kiki == null) kiki = FindObjectOfType<KikiController>();
        if (dialogue == null) dialogue = FindObjectOfType<DialogueManager>();

        // 찾고 있는 중(로딩 중)이면 아무것도 안 하고 대기
        if (kiki == null || dialogue == null) return;

        // 플레이어와의 거리를 계산합니다.
        float dist = Vector3.Distance(transform.position, kiki.transform.position);

        if (dist <= detectRange)
        {
            // 거리 안이면 글자를 보여줍니다.
            if (interactUI != null && !interactUI.activeSelf) interactUI.SetActive(true);

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
            if (interactUI != null && interactUI.activeSelf) interactUI.SetActive(false);
        }
    }

    // SkillData에서 설정된 쿨타임 값을 가져오는 함수입니다.
    float GetCool(SkillData data)
    {
        return data != null ? data.cooldownTime : 5.0f;
    }
}