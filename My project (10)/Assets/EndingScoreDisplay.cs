using UnityEngine;
using UnityEngine.UI;

public class EndingScoreDisplay : MonoBehaviour
{
    public Text scoreText; // 화면 중앙에 띄울 텍스트

    void Start()
    {
        int finalScore = 0;
        if (GameManager.Instance != null)
        {
            finalScore = GameManager.Instance.totalScore;
        }

        if (scoreText != null)
        {
            scoreText.text = "당신의 모험 점수는?\n" + finalScore + " 점";

            // 점수에 따른 멘트 추가 가능
            if (finalScore >= 100) scoreText.text += "\n(완벽한 영웅!)";
            else if (finalScore < 0) scoreText.text += "\n(악동 그 자체...)";
        }
    }
}