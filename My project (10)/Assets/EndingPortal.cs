using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동용

public class EndingPortal : MonoBehaviour
{
    public GameObject returnHomeUI; // "집으로 돌아가기 [E]" 텍스트 UI
    private bool isNearby = false;

    void Update()
    {
        if (isNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("게임 클리어! 엔딩 씬으로 이동합니다.");
            // "EndingScene"이라는 이름의 씬을 만들어두셔야 합니다!
            // SceneManager.LoadScene("EndingScene"); 
            // 일단은 로그만 띄우거나 게임 종료
            Application.Quit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearby = true;
            if (returnHomeUI != null) returnHomeUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearby = false;
            if (returnHomeUI != null) returnHomeUI.SetActive(false);
        }
    }
}