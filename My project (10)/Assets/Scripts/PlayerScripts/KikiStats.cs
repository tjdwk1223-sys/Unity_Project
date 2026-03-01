using UnityEngine;

[CreateAssetMenu(fileName = "NewStat", menuName = "Stats/CharacterStat")]
public class CharacterStats : ScriptableObject
{
    [Header("기본 능력치")]
    public int maxHp = 100;
    public int damage = 20;
    public float moveSpeed = 5.0f;   // 걷기 속도
    public float runSpeed = 8.0f;    // ★ [추가] 달리기 속도 (기본값 설정)

    [Header("전투 설정")]
    public float attackSpeed = 1.0f;
    public float attackRange = 2.0f;

    public float AttackCooldown => 1f / attackSpeed;
}