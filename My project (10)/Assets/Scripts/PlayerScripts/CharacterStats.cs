using UnityEngine;

// 프로젝트 창 우클릭 > Create > Stats > CharacterStat 메뉴 생성
[CreateAssetMenu(fileName = "NewStat", menuName = "Stats/CharacterStat")]
public class CharacterStats : ScriptableObject
{
    [Header("기본 능력치")]
    public int maxHp = 100;
    public int damage = 20;
    public float moveSpeed = 5.0f;

    [Header("전투 설정")]
    public float attackSpeed = 1.0f; // 1초에 공격하는 횟수 (높을수록 빠름)
    public float attackRange = 2.0f;

    // 공격 쿨타임 계산 (공속이 2면 0.5초마다 공격)
    public float AttackCooldown => 1f / attackSpeed;
}