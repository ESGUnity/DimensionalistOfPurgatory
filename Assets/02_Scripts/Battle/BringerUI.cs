using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BringerUI : MonoBehaviour
{
    BringerSystem bringerSystem;
    GameObject bringerObject;
    Image healthImage;
    TextMeshProUGUI healthText;

    void Awake()
    {
        healthImage = transform.GetChild(1).GetComponent<Image>(); // UI 이미지가 있는 자식 위치
        healthText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        healthImage.fillAmount = (float)bringerSystem.CurrentHealth / bringerSystem.MaxHealth;
        healthText.text = bringerSystem.CurrentHealth.ToString();
    }
    public void SetParentObject(GameObject bringerObject, BringerSystem bringerSystem)
    {
        this.bringerSystem = bringerSystem;
        this.bringerObject = bringerObject;
        transform.position = bringerObject.transform.position + new Vector3(0, 3.15f, 0);

        if (bringerSystem.thisPlayerTag == "Player") // 영체 태그에 따른 체력바의 색깔 변경
        {
            healthImage.color = new Color(0f, 0.8f, 0.4f);
        }
        else if (bringerSystem.thisPlayerTag == "Opponent")
        {
            healthImage.color = new Color(0.95f, 0.2f, 0f);
        }
    }
}
