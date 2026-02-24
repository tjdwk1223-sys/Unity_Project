using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalWarp : MonoBehaviour
{
    [Header("이동할 맵 이름")]
    // 인스펙터 창에서 원하는 맵 이름을 적을 수 있게 열어둡니다.
    public string targetSceneName = "Map2";

    private bool isPlayerInPortal = false; // 키키가 포탈 근처에 있는지 확인

    void Update()
    {
        // 키키가 포탈 반경 안에 있고(true), 키보드 E를 눌렀을 때만 씬 이동
        if (isPlayerInPortal && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(targetSceneName + " (으)로 이동합니다!");
            SceneManager.LoadScene(targetSceneName);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 닿은 물체가 키키(Player)인지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true; // 포탈 안에 들어왔다고 체크
            Debug.Log("[E] 상호작용 UI 켜기");
            // 나중에 여기에 "E를 눌러 이동" UI를 켜는 코드를 넣으면 됩니다!
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 키키가 포탈 밖으로 나가면
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false; // 포탈 밖으로 나갔다고 체크
            Debug.Log("[E] 상호작용 UI 끄기");
            // 나중에 여기에 UI를 다시 끄는 코드를 넣으면 됩니다!
        }
    }
}