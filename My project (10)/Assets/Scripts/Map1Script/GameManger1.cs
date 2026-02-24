using UnityEngine;
using VariableInventorySystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 인벤토리 데이터 금고
    public StandardStashViewData PlayerInventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 8x16 크기의 빈 인벤토리 데이터 생성
            PlayerInventory = new StandardStashViewData(8, 16);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}