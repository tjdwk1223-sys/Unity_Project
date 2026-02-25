using UnityEngine;

// 이 줄이 있어야 'Create' 메뉴에 'Boss'가 나타납니다!
[CreateAssetMenu(fileName = "NewBossData", menuName = "Boss/BossData", order = 0)]
public class BossData : ScriptableObject
{
    [Header("기본 스탯")]
    public float maxHp = 1000f;
    public float phase2Threshold = 500f;

    [Header("거리 설정")]
    public float runDistance = 15f;
    public float attackDistance = 3.5f;
    public float detectDistance = 15f;

    [Header("전투 설정")]
    public float attackMaxCooldown = 10f;
    public float runSpeed = 8f;
    public float walkSpeed = 3.5f;

    [Header("경직 시간")]
    public float stunTimeSmall = 1.0f;
    public float stunTimeMedium = 1.5f;
    public float stunTimeLarge = 2.0f;
}