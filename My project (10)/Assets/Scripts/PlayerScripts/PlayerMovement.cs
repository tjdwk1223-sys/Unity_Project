using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KikiController : MonoBehaviour
{
    [Header("--- 광역 스킬(AoE) 설정 ---")]
    public float aoeRadius = 5.0f;

    public void AnimEvent_AoEAttack(float multiplier)
    {
        int finalDamage = Mathf.RoundToInt(currentBaseDamage * multiplier);
        Debug.Log($"쾅! 광역기 폭발! (반경: {aoeRadius}, 대미지: {finalDamage})");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRadius);

        foreach (Collider col in hitColliders)
        {
            ZombieController zombie = col.GetComponentInParent<ZombieController>();
            if (zombie != null)
            {
                Debug.Log($"-> {zombie.name} 명중! 대미지 들어감!");
                zombie.OnHit();
                zombie.TakeDamage(finalDamage);
            }

            MimiNPC mimi = col.GetComponentInParent<MimiNPC>();
            if (mimi != null && mimi.isAttacking == true)
            {
                mimi.TakeDamage(finalDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }

    [Header("★ 능력치 데이터 연결")]
    public CharacterStats stats;
    private float lastAttackTime;

    private int currentBaseDamage;

    [Header("실시간 상태")]
    public float currentHp;

    [Header("--- HP 바 UI ---")]
    public Image hpBarImage;

    [Header("점프 및 중력")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    [Header("마우스 시선 회전 설정")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    private float xRotation = 0f;

    [Header("필수 연결 요소")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("--- X 스킬 (땅 후려찍기) 설정 ---")]
    public bool isXSkillUpgraded = false;
    public GameObject xSkillEffectPrefab;
    public Transform groundImpactPoint;

    [Header("--- V 스킬 (Shiny Slash) 설정 ---")]
    public GameObject vSkillEffectPrefab;
    public Transform vSkillImpactPoint;

    [Header("--- 무기 & 타격 부위 연결 ---")]
    public WeaponController swordWeapon;
    public WeaponController footWeapon;
    public float attackActiveTime = 0.5f;

    // ▼▼▼▼▼ [새로 추가된 부분: 스킬 데이터 및 타이머] ▼▼▼▼▼
    [Header("--- 스킬 데이터 (쿨타임 관리용) ---")]
    public SkillData xSkillData;
    public SkillData cSkillData;
    public SkillData qSkillData;
    public SkillData fSkillData;
    public SkillData vSkillData;

    // 실제 쿨타임이 줄어들 타이머 변수들 (인스펙터에 안보이게 숨김)
    [HideInInspector] public float xSkillTimer = 0f;
    [HideInInspector] public float cSkillTimer = 0f;
    [HideInInspector] public float qSkillTimer = 0f;
    [HideInInspector] public float fSkillTimer = 0f;
    [HideInInspector] public float vSkillTimer = 0f;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        if (stats != null) currentHp = stats.maxHp;

        if (PlayerPrefs.GetInt("XSkillUpgraded", 0) == 1)
        {
            isXSkillUpgraded = true;
        }
    }

    void Update()
    {
        // ▼▼▼ [새로 추가된 부분: 매 프레임마다 스킬 쿨타임 타이머 줄이기] ▼▼▼
        if (xSkillTimer > 0) xSkillTimer -= Time.deltaTime;
        if (cSkillTimer > 0) cSkillTimer -= Time.deltaTime;
        if (qSkillTimer > 0) qSkillTimer -= Time.deltaTime;
        if (fSkillTimer > 0) fSkillTimer -= Time.deltaTime;
        if (vSkillTimer > 0) vSkillTimer -= Time.deltaTime;
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        isGrounded = controller.isGrounded;

        if (anim != null) anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float baseSpeed = (stats != null) ? stats.moveSpeed : 5.0f;
        float runSpeedVal = (stats != null) ? stats.runSpeed : 8.0f;

        bool isRunningInput = Input.GetKey(KeyCode.LeftShift) && isGrounded;
        float currentSpeed = isRunningInput ? runSpeedVal : baseSpeed;

        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        if (anim != null)
        {
            anim.SetFloat("Horizontal", x, 0.1f, Time.deltaTime);
            anim.SetFloat("Vertical", z, 0.1f, Time.deltaTime);

            float inputMagnitude = new Vector2(x, z).magnitude;
            float animSpeedValue = isRunningInput ? 2.0f : 1.0f;
            anim.SetFloat("Speed", inputMagnitude * animSpeedValue);

            if (inputMagnitude > 0.1f && isRunningInput)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (anim != null) anim.SetBool("isGrounded", false);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HandleCombatInput();

        if (hpBarImage != null && stats != null)
        {
            hpBarImage.fillAmount = currentHp / stats.maxHp;
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        Debug.Log($"앗! 키키가 {damage}만큼 맞았다! 남은 피: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("키키가 쓰러졌습니다... 게임 오버!");
        if (anim != null) anim.SetTrigger("Die");

        if (controller != null) controller.enabled = false;
        this.enabled = false;
    }

    void HandleCombatInput()
    {
        if (anim == null || stats == null) return;

        if (Time.time >= lastAttackTime + stats.AttackCooldown)
        {
            bool isAttack = false;
            currentBaseDamage = (stats != null) ? stats.damage : 20;

            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("doSword");
                isAttack = true;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Z))
            {
                anim.SetTrigger("doKick");
                isAttack = true;
            }

            // ▼▼▼ [수정된 부분: 각 스킬 입력 시 쿨타임(Timer) 체크 및 초기화] ▼▼▼
            if (Input.GetKeyDown(KeyCode.X) && xSkillTimer <= 0)
            {
                anim.SetTrigger("doSkill");
                isAttack = true;
                xSkillTimer = xSkillData != null ? xSkillData.cooldownTime : 5f;
            }

            if (Input.GetKeyDown(KeyCode.C) && cSkillTimer <= 0)
            {
                anim.SetTrigger("doSkill2");
                isAttack = true;
                cSkillTimer = cSkillData != null ? cSkillData.cooldownTime : 5f;
            }

            if (Input.GetKeyDown(KeyCode.Q) && qSkillTimer <= 0)
            {
                anim.SetTrigger("doSkill3");
                isAttack = true;
                qSkillTimer = qSkillData != null ? qSkillData.cooldownTime : 5f;
            }

            if (Input.GetKeyDown(KeyCode.F) && fSkillTimer <= 0)
            {
                anim.SetTrigger("doSkill4");
                isAttack = true;
                fSkillTimer = fSkillData != null ? fSkillData.cooldownTime : 5f;
            }

            if (Input.GetKeyDown(KeyCode.V) && vSkillTimer <= 0)
            {
                anim.SetTrigger("doSkill5");
                isAttack = true;
                vSkillTimer = vSkillData != null ? vSkillData.cooldownTime : 5f;

                if (GameManager.Instance != null && GameManager.Instance.isVSkillUpgraded && vSkillEffectPrefab != null)
                {
                    Vector3 spawnPos = vSkillImpactPoint != null ? vSkillImpactPoint.position : transform.position;
                    Instantiate(vSkillEffectPrefab, spawnPos, transform.rotation);
                }
            }
            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            if (isAttack)
            {
                lastAttackTime = Time.time;
            }
        }
    }

    public void AnimEvent_EnableSword(float multiplier)
    {
        if (swordWeapon != null)
        {
            swordWeapon.currentDamage = Mathf.RoundToInt(currentBaseDamage * multiplier);
            swordWeapon.EnableCollider();
        }
    }

    public void AnimEvent_DisableSword()
    {
        if (swordWeapon != null) swordWeapon.DisableCollider();
    }

    public void AnimEvent_EnableFoot(float multiplier)
    {
        if (footWeapon != null)
        {
            footWeapon.currentDamage = Mathf.RoundToInt(currentBaseDamage * multiplier);
            footWeapon.EnableCollider();
        }
    }

    public void AnimEvent_DisableFoot()
    {
        if (footWeapon != null) footWeapon.DisableCollider();
    }

    public void UpgradeXSkill()
    {
        isXSkillUpgraded = true;
        PlayerPrefs.SetInt("XSkillUpgraded", 1);
        Debug.Log("땅 후려찍기(X) 스킬 강화 완료!! 거대 이펙트 장전!");
    }

    public void SpawnXSkillEffect()
    {
        if (isXSkillUpgraded && xSkillEffectPrefab != null)
        {
            Vector3 spawnPos = groundImpactPoint != null ? groundImpactPoint.position : transform.position;
            Instantiate(xSkillEffectPrefab, spawnPos, Quaternion.identity);
        }
    }
}
