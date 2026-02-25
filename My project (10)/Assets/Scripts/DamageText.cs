using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float alphaSpeed = 2.0f;
    public float destroyTime = 1.0f;

    private TextMeshPro textMesh;
    private Color textColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        // 이벤트 카메라 자동 연결
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null) { canvas.worldCamera = Camera.main; }
    }

    public void Setup(int damage, bool isCritical)
    {
        textMesh.text = damage.ToString();
        textColor = textMesh.color;
        if (isCritical) { textMesh.color = Color.red; textMesh.fontSize *= 1.5f; }
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        textColor.a -= alphaSpeed * Time.deltaTime;
        textMesh.color = textColor;

        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}