using UnityEngine;
using VariableInventorySystem;

public class PlayerInventoryController : MonoBehaviour
{
    [Header("인벤토리 시스템 연결")]
    public StandardCore standardCore;
    public StandardStashView standardStashView;

    [Header("인벤토리 UI 창 (껐다 켤 대상)")]
    public GameObject inventoryUIWindow;

    private bool isInventoryOpen = false;

    void Start()
    {
        // 1. 코어(뇌) 초기화 및 뷰(눈) 연결
        standardCore.Initialize();
        standardCore.AddInventoryView(standardStashView);

        // 2. 게임 시작 시 인벤토리 창은 닫아두기
        inventoryUIWindow.SetActive(false);
    }

    void Update()
    {
        // 'I' 키를 누르면 인벤토리 창 열고 닫기
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // 인벤토리가 열려있을 때 'R' 키로 잡고 있는 아이템 회전
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.R))
        {
            standardCore.SwitchRotate();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        // UI 오브젝트 끄고 켜기
        inventoryUIWindow.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            // [추가된 부분] 인벤토리가 열렸을 때: 마우스 포인터를 보이게 하고 잠금 해제
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // GameManager(금고)에서 데이터를 가져와서 화면에 그리기
            if (GameManager.Instance != null && GameManager.Instance.PlayerInventory != null)
            {
                standardStashView.Apply(GameManager.Instance.PlayerInventory);
            }
        }
        else
        {
            // [추가된 부분] 인벤토리가 닫혔을 때: 마우스 포인터를 숨기고 화면 중앙에 고정
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}