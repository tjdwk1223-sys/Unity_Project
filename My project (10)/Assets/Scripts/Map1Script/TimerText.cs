using UnityEngine;
using UnityEngine.UI;
using TMPro; // ★ 이게 있어야 TextMeshPro를 인식합니다!

public class GameTimer : MonoBehaviour
{
    // ★ 타입을 Text -> TextMeshProUGUI로 변경했습니다.
    public TextMeshProUGUI timerText;

    void Start()
    {
        // 연결 안 되어 있으면 자동으로 내 몸에 있는 TMP를 찾아서 넣음
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        float t = Time.time;

        string minutes = ((int)t / 60).ToString("00");
        string seconds = (t % 60).ToString("00.00");

        if (timerText != null)
        {
            timerText.text = "TIME: " + minutes + ":" + seconds;
        }
    }
}