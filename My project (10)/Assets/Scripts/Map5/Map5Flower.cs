using UnityEngine;

public class Map5Flower : MonoBehaviour
{
    public Map5QuestNPC npc;       // 모모 연결
    public GameObject interactUI;  // [E] 줍기 UI

    private bool isNearby = false;

    void Update()
    {
        if (isNearby && Input.GetKeyDown(KeyCode.E))
        {
            // NPC에게 "나 버섯 주웠어!" 라고 알림
            if (npc != null) npc.GetMushroom();

            // 내 할 일 끝났으니 사라짐
            gameObject.SetActive(false);
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearby = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearby = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}