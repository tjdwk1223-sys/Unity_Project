using UnityEngine;

public class RockInteraction : MonoBehaviour
{
    [Header("UI 연결 (인스펙터 빈칸 꼭 확인!)")]
    public GameObject interactText;
    public GameObject settingsPanel;

    [Header("상호작용 레이더 반경")]
    public float interactDistance = 5.0f;

    private bool isPlayerNearby = false;

    void Start()
    {
        if (interactText != null) interactText.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void Update()
    {
        // [완전히 새로운 방식] 돌맹이 주변(반경 5)에 있는 모든 콜라이더를 스캔합니다!
        // (키키의 무기뿐만 아니라 몸통 콜라이더도 무조건 잡아냅니다)
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactDistance);
        bool foundPlayerThisFrame = false;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                foundPlayerThisFrame = true;
                break; // 키키를 찾았으니 스캔 종료!
            }
        }

        // 1. 키키가 방금 내 레이더망에 들어왔을 때 (딱 1번만 켬 = 남의 돌맹이 훼방 원천 차단)
        if (foundPlayerThisFrame && !isPlayerNearby)
        {
            isPlayerNearby = true;
            if (interactText != null && settingsPanel != null && !settingsPanel.activeSelf)
            {
                interactText.SetActive(true);
            }
        }
        // 2. 키키가 내 레이더망 밖으로 나갔을 때 (딱 1번만 끔)
        else if (!foundPlayerThisFrame && isPlayerNearby)
        {
            isPlayerNearby = false;
            if (interactText != null) interactText.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        // 3. 레이더 안에 있을 때 E키 작동
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (settingsPanel != null)
            {
                bool isPanelActive = settingsPanel.activeSelf;
                settingsPanel.SetActive(!isPanelActive);

                if (interactText != null) interactText.SetActive(isPanelActive);
            }
        }
    }

    // ★ 보너스: 씬 뷰에서 설정한 거리를 '노란색 원'으로 직접 볼 수 있게 해줍니다!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}