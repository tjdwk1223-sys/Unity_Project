using UnityEngine;

public class RockInteraction : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject interactText;
    public GameObject settingsPanel;

    [Header("거리 설정")]
    public float interactDistance = 3.0f; // 이 숫자만큼 가까워지면 작동 (필요시 인스펙터에서 수정 가능)

    private Transform player;

    void Start()
    {
        // 시작할 때 UI 끄기
        if (interactText != null) interactText.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // 태그가 "Player"인 키키를 자동으로 찾아옵니다.
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // ★ 핵심: 물리 엔진을 무시하고, 돌과 키키의 실제 '거리'를 수학적으로 계산합니다.
        float distance = Vector3.Distance(transform.position, player.position);

        // 1. 키키가 설정한 거리(3.0f) 안으로 들어왔을 때
        if (distance <= interactDistance)
        {
            // 조작법 창이 안 열려있다면 E 문구 띄우기
            if (interactText != null && settingsPanel != null && !settingsPanel.activeSelf)
            {
                interactText.SetActive(true);
            }

            // E키를 눌렀을 때
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (settingsPanel != null)
                {
                    bool isPanelActive = settingsPanel.activeSelf;
                    settingsPanel.SetActive(!isPanelActive); // 창 켜고 끄기

                    // 창이 켜지면 E 문구는 숨기기
                    if (interactText != null) interactText.SetActive(isPanelActive);
                }
            }
        }
        // 2. 키키가 거리 밖으로 멀어졌을 때
        else
        {
            if (interactText != null) interactText.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }
    }
}