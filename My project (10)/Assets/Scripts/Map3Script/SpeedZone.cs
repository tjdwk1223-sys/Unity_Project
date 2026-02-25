using UnityEngine;

public class SpeedZone : MonoBehaviour
{
    [Header("이 구간을 지날 때 목표 속도")]
    public float newSpeed = 40f;

    [Header("얼마나 확 변할지 (가속도)")]
    public float acceleration = 2f; // 숫자가 클수록 급브레이크/급발진

    private void OnTriggerEnter(Collider other)
    {
        // 닿은 물체(카트)에서 CoasterSystem을 찾습니다.
        CoasterSystem coaster = other.GetComponentInParent<CoasterSystem>();
        if (coaster != null)
        {
            // 속도 변경 명령 내리기!
            coaster.ChangeSpeed(newSpeed, acceleration);
        }
    }
}