using UnityEngine;

// 유니티 우클릭 메뉴에서 스킬 데이터를 생성할 수 있게 해줍니다.
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("스킬 기본 정보")]
    public string skillName;        // 스킬 이름 (예: 평타, 검기, 파이어볼)

    [Header("대미지 설정")]
    public int minDamage = 10;      // 최소 대미지
    public int maxDamage = 20;      // 최대 대미지

    [Header("크리티컬 설정")]
    [Range(0, 100)]
    public int critChance = 20;     // 크리티컬 확률 (20%)
    public float critMultiplier = 2.0f; // 크리티컬 배율 (2배)

    // 이 스킬을 썼을 때 최종 대미지와 크리티컬 여부를 계산해주는 함수
    public void CalculateDamage(out int finalDamage, out bool isCrit)
    {
        int baseDamage = Random.Range(minDamage, maxDamage + 1);
        isCrit = Random.Range(0, 100) < critChance;

        if (isCrit)
        {
            finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);
        }
        else
        {
            finalDamage = baseDamage;
        }
    }
}