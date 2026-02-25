using UnityEngine;

// 클래스 이름을 파일 이름(PortalController)과 똑같이 맞춰주는 게 유니티 국룰입니다!
public class PortalController : MonoBehaviour
{
    [Header("상호작용 UI 설정")]
    public GameObject interactUI; // "E를 눌러 이동" 문구 오브젝트를 여기에 드래그

    private bool isPlayerInPortal = false;

    void Start()
    {
        // 시작할 때 UI는 꺼둡니다.
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        // 포탈 안이고 E를 눌렀을 때
        if (isPlayerInPortal && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("인벤토리를 챙겨서 다음 스테이지로 이동합니다!");

            // ★ 중요: 그냥 SceneManager 쓰지 말고, 우리가 만든 금고(GameManager)를 통해 이동!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NextStage();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true;
            // 상호작용 문구 켜기
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false;
            // 상호작용 문구 끄기
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}