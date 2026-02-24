using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5f;
    public GameObject interactUI;

    void Update()
    {
        // [수정] 레이저 시작 위치를 몸 중심에서 '앞으로 0.5m' 이동시킵니다.
        // 이렇게 하면 내 몸(Collider)에 레이저가 막히지 않습니다.
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f + transform.forward * 0.5f;
        Ray ray = new Ray(rayOrigin, transform.forward);
        RaycastHit hit;

        // 디버그용: 씬 뷰에서 초록색 선으로 레이저가 보입니다.
        Debug.DrawRay(rayOrigin, transform.forward * interactDistance, Color.green);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // [디버깅] 레이저가 무엇에 맞았는지 콘솔에 출력합니다 (테스트용)
            // Debug.Log("레이저가 맞은 물체: " + hit.collider.name);

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (interactUI != null) interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
            else
            {
                if (interactUI != null) interactUI.SetActive(false);
            }
        }
        else
        {
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}