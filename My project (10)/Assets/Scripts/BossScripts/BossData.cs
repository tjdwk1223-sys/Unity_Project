using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "Boss/BossData", order = 0)]
public class BossData : ScriptableObject
{
    [Header("기본 스탯")]
    public float maxHp = 1000f;
    public float phase2Threshold = 500f;

    [Header("거리 및 속도 설정")]
    public float attackDistance = 2.0f;
    public float runSpeed = 8f;
    public float walkSpeed = 3.5f;

    [Header("경직 설정 (대미지 기준 및 시간)")]
    public float stunThresholdSmall = 10f;
    public float stunTimeSmall = 1.0f;

    public float stunThresholdMedium = 50f;
    public float stunTimeMedium = 1.5f;

    public float stunThresholdLarge = 80f;
    public float stunTimeLarge = 2.0f;

    [Header("공격 스킬 설정 (쿨타임 / 데미지)")]
    public float skill1Cooldown = 3f; public float skill1Damage = 10f;
    public float skill2Cooldown = 5f; public float skill2Damage = 20f;
    public float skill3Cooldown = 8f; public float skill3Damage = 35f;
    public float skill4Cooldown = 12f; public float skill4Damage = 50f;

    [Header("이동/회피 쿨타임")]
    public float strafeLeftCooldown = 4f;
    public float strafeRightCooldown = 4f;
    public float walkBackCooldown = 6f;

    // ▼▼▼ [새로 추가된 페이즈 2 (광폭화) 설정] ▼▼▼
    [Header("페이즈 2 (광폭화) 설정")]
    public float phase2SizeMultiplier = 1.3f;   // 크기 (1.3배 ~ 1.5배 조절)
    public float phase2DamageMultiplier = 1.5f; // 대미지 (1.5배 뻥튀기)
    public float phase2SpeedMultiplier = 1.2f;  // 이동속도 (1.2배 빨라짐)
}