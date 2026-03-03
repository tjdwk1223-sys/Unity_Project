using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("UI 연결 (인스펙터 빈칸 꼭 확인!)")]
    public GameObject interactUI; // 아까 만든 [E] 텍스트를 넣을 칸
    public DialogueManager dialogueManager; // 대사창 매니저를 넣을 칸

    [Header("이 오브젝트의 대사")]
    [TextArea(3, 10)] // 인스펙터 창에서 여러 줄을 편하게 입력할 수 있게 해줍니다.
    public string[] lines;

    private bool isPlayerNearby = false;

    void Start()
    {
        // 게임 시작 시 [E] UI가 켜져 있다면 안전하게 꺼줍니다.
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        // 1. 플레이어가 근처에 없거나 E키를 누르지 않았다면 무시!
        if (!isPlayerNearby || !Input.GetKeyDown(KeyCode.E)) return;

        // 2. 대화창이 이미 열려있다면 중복으로 실행되지 않게 막기!
        if (dialogueManager != null && dialogueManager.isDialogueActive) return;

        // 3. E키를 정상적으로 눌렀을 때
        if (interactUI != null) interactUI.SetActive(false); // E 글자는 끄고
        if (dialogueManager != null) dialogueManager.StartDialogue(lines); // 대사 시작!
    }

    void OnTriggerEnter(Collider other)
    {
        // 키키(플레이어)가 다가오면
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactUI != null) interactUI.SetActive(true); // E UI 켜기
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 키키(플레이어)가 멀어지면
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactUI != null) interactUI.SetActive(false); // E UI 끄기
        }
    }
}