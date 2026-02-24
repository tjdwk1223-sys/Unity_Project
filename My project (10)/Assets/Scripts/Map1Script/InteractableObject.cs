using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject interactUI; // 아까 만든 [E] 텍스트를 넣을 칸
    public DialogueManager dialogueManager; // 대사창 매니저를 넣을 칸

    [Header("이 오브젝트의 대사")]
    [TextArea(3, 10)] // 인스펙터 창에서 여러 줄을 편하게 입력할 수 있게 해줍니다.
    public string[] lines;

    private bool isPlayerNearby = false;

    void Update()
    {
        // 플레이어가 근처에 있고 E키를 누르면
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            interactUI.SetActive(false); // E 글자는 끄고
            dialogueManager.StartDialogue(lines); // 매니저에게 대사를 넘겨주고 출력 시작!
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            interactUI.SetActive(true); // 근처에 가면 UI 켜기
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactUI.SetActive(false); // 멀어지면 UI 끄기
        }
    }
}