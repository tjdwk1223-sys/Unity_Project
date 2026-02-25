using UnityEngine;
using UnityEngine.SceneManagement;
using VariableInventorySystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("인벤토리 데이터 금고")]
    public StandardStashViewData PlayerInventory;

    // 변수로 숫자를 세는 방식은 꼬이기 쉬우므로 제거했습니다.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 인벤토리 초기화
            PlayerInventory = new StandardStashViewData(8, 16);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextStage()
    {
        // 현재 활성화된 씬의 빌드 인덱스 번호에서 +1을 하여 다음 장면을 불러옵니다.
        // File > Build Settings 창에 등록된 순서대로 정확히 이동합니다.
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // 전체 씬 개수를 넘지 않는지 확인 후 이동
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 스테이지입니다!");
        }
    }
}