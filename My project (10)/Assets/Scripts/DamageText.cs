using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float alphaSpeed = 2.0f;
    public float destroyTime = 1.0f;

    public TextMeshPro textMesh;
    private Color textColor;

    public void Setup(int damage, bool isCritical)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();

        textMesh.text = damage.ToString();
        textColor = textMesh.color;

        if (isCritical)
        {
            textMesh.color = Color.red;
            textMesh.fontSize *= 1.5f;
        }
        Destroy(gameObject, destroyTime);
    }

    void LateUpdate()
    {
        // 1. 위로 둥실 떠오르기
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 2. 투명해지기
        if (textMesh != null)
        {
            textColor.a -= alphaSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }

        // 3. [진짜 찐 최종] 뒷면이 보이는 현상(180도 뒤집힘) 완벽 해결!
        if (Camera.main != null)
        {
            // 카메라의 Y축 각도를 가져온 다음, 우리를 마주보게 180도를 휙 돌려줍니다!
            float camY = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, camY + 180f, 0f);
        }
    }
}