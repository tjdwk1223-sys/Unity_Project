using UnityEngine;
using Cinemachine;

public class CoasterSystem : MonoBehaviour
{
    [Header("--- 연결할 것들 ---")]
    public GameObject player;
    public Transform seatPoint;
    public Transform exitPoint;
    public GameObject uiPanel;
    public CinemachineDollyCart cart;

    [Header("--- 속도 제어 ---")]
    public float moveSpeed = 20f;      // 출발할 때 기본 속도
    public float targetSpeed = 0f;     // 목표 속도 (구간마다 바뀜)
    public float accel = 3f;           // 가속도 (수치가 클수록 확 변함)

    [Header("--- 카메라 & UI 설정 ---")]
    public GameObject rideCamera;
    public GameObject viewHintUI;

    private bool isRiding = false;
    private Animator playerAnim;
    private KikiController kikiScript;

    private Vector3 lastCartPos;
    private float stopTimer = 0f;

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        if (cart != null) cart.m_Speed = 0;
        if (rideCamera != null) rideCamera.SetActive(false);
        if (viewHintUI != null) viewHintUI.SetActive(false);

        if (player != null)
        {
            playerAnim = player.GetComponent<Animator>();
            kikiScript = player.GetComponent<KikiController>();
        }
    }

    void Update()
    {
        if (isRiding)
        {
            // ★ 1. 부드러운 가속/감속 시스템
            if (cart != null)
            {
                cart.m_Speed = Mathf.Lerp(cart.m_Speed, targetSpeed, Time.deltaTime * accel);
            }

            // ★ 2. 자동 정차 시스템
            if (targetSpeed > 0)
            {
                if (Vector3.Distance(cart.transform.position, lastCartPos) < 0.01f)
                {
                    stopTimer += Time.deltaTime;
                    if (stopTimer > 0.5f)
                    {
                        targetSpeed = 0;
                        cart.m_Speed = 0;
                        stopTimer = 0f;
                    }
                }
                else stopTimer = 0f;
            }
            lastCartPos = cart.transform.position;

            // 3. 완전히 멈췄을 때만 내리기 가능
            if (cart.m_Speed < 0.5f && targetSpeed == 0 && Input.GetKeyDown(KeyCode.E))
            {
                GetOut();
            }

            // 4. 시점 변경
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (rideCamera != null) rideCamera.SetActive(!rideCamera.activeSelf);
            }
        }
        else if (!isRiding && Input.GetKeyDown(KeyCode.E))
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 3.0f)
                Ride();
        }
    }

    public void Ride()
    {
        isRiding = true;

        if (kikiScript != null) kikiScript.enabled = false;
        if (player.GetComponent<CharacterController>())
            player.GetComponent<CharacterController>().enabled = false;

        player.transform.SetParent(seatPoint);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        if (playerAnim != null) playerAnim.SetBool("isSitting", true);
        if (rideCamera != null) rideCamera.SetActive(true);
        if (viewHintUI != null) viewHintUI.SetActive(true);

        uiPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void GetOut()
    {
        isRiding = false;

        if (playerAnim != null) playerAnim.SetBool("isSitting", false);

        player.transform.SetParent(null);
        if (exitPoint != null)
        {
            player.transform.position = exitPoint.position;
            player.transform.rotation = exitPoint.rotation;
        }

        if (player.GetComponent<CharacterController>())
            player.GetComponent<CharacterController>().enabled = true;
        if (kikiScript != null) kikiScript.enabled = true;

        if (cart != null)
        {
            targetSpeed = 0;
            cart.m_Speed = 0;
            cart.m_Position = 0;
        }

        uiPanel.SetActive(false);
        if (rideCamera != null) rideCamera.SetActive(false);
        if (viewHintUI != null) viewHintUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnClickYes()
    {
        uiPanel.SetActive(false);
        targetSpeed = moveSpeed; // 버튼 누르면 목표 속도로 설정!
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnClickNo()
    {
        GetOut();
    }

    // ★ 외부(투명 표지판)에서 이 함수를 불러서 속도를 바꿈 ★
    public void ChangeSpeed(float newSpeed, float newAccel)
    {
        targetSpeed = newSpeed;
        accel = newAccel;
    }
}