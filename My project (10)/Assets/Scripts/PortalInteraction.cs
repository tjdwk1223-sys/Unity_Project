using UnityEngine;

public class PortalInteraction : MonoBehaviour
{
    public GameObject interactUI; // "E를 눌러 이동" 텍스트 오브젝트
    private bool isReadyToNext = false; // 플레이어가 범위 안에 있는지 확인

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false); // 처음엔 끄기
    }

    void Update()
    {
        // 1. 플레이어가 범위 안에 있고 + 2. E키를 누르면
        if (isReadyToNext && Input.GetKeyDown(KeyCode.E))
        {
            // GameManager에 짜둔 다음 스테이지 이동 함수 실행!
            GameManager.Instance.NextStage();
        }
    }

    // 센서 범위 안으로 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isReadyToNext = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    // 센서 범위 밖으로 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isReadyToNext = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}