using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUnlockRock : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject interactText;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("거리 설정")]
    public float interactDistance = 5.0f;

    private Transform player;
    private int clickCount = 0;
    private bool isUnlocked = false;
    private float nextClickTime = 0f;

    void Start()
    {
        if (interactText != null) interactText.SetActive(false);

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            Image panelImage = dialoguePanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = new Color(0f, 0f, 0f, 0.7f);
            }
        }

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance)
        {
            if (!dialoguePanel.activeSelf && !isUnlocked && interactText != null)
            {
                interactText.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                if (isUnlocked) return;

                if (Time.time < nextClickTime) return;
                nextClickTime = Time.time + 0.5f;

                if (interactText != null) interactText.SetActive(false);
                if (dialoguePanel != null) dialoguePanel.SetActive(true);

                clickCount++;

                if (clickCount == 1) dialogueText.text = "너는 선택받았다.";
                else if (clickCount == 2) dialogueText.text = "시험을 치르면 집으로 돌아갈 수 있다.";
                else if (clickCount == 3) dialogueText.text = "너 자신을 찾아라.";
                else if (clickCount == 4)
                {
                    dialogueText.text = "X 스킬이 강화되었습니다!!";
                    UnlockXSkill();
                }
                else if (clickCount >= 5)
                {
                    // ★ 5번째 클릭 시: 창을 먼저 끄고 나서 잠금 처리!
                    if (dialoguePanel != null) dialoguePanel.SetActive(false);
                    isUnlocked = true;
                }
            }
        }
        else
        {
            if (interactText != null) interactText.SetActive(false);
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (!isUnlocked) clickCount = 0;
        }
    }

    void UnlockXSkill()
    {
        if (player != null)
        {
            KikiController kc = player.GetComponent<KikiController>();
            if (kc != null)
            {
                kc.UpgradeXSkill();
            }
        }
    }
}