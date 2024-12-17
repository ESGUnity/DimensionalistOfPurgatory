using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AstralUI : MonoBehaviour
{
    [SerializeField] GameObject stunIcon;
    [SerializeField] GameObject declainIcon;
    [SerializeField] GameObject sealIcon;
    [SerializeField] GameObject punishIcon;
    [SerializeField] GameObject encroachmentIcon;
    [SerializeField] GameObject invincibilityIcon;
    [SerializeField] GameObject statusEffectContent;

    GameObject stun;
    GameObject declain;
    GameObject seal;
    GameObject punish;
    GameObject encroachment;
    GameObject invincibility;

    bool isStun = false;
    bool isDeclain = false;
    bool isSeal = false;
    bool isPunish = false;
    bool isEncroachment = false;
    bool isInvincibility = false;

    GameObject parentObject;
    Image healthImage;
    Image manaAndConditionImage;
    TextMeshProUGUI healthText;
    TextMeshProUGUI manaAndConditionText;
    TextMeshProUGUI DamageText;

    void Awake()
    {
        healthImage = transform.GetChild(1).gameObject.GetComponent<Image>();
        manaAndConditionImage = transform.GetChild(2).gameObject.GetComponent<Image>();
        healthText = transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        manaAndConditionText = transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>();
        DamageText = transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (parentObject != null)
        {
            transform.position = parentObject.transform.GetChild(2).position + new Vector3(0, 1.3f, 0); // UI가 StunPosition보다 1.3만큼 위에 위치하도록 설정
            DamageText.text = parentObject.GetComponent<AstralBody>().astralStats.Damage.ToString();
            healthImage.fillAmount = (float)parentObject.GetComponent<AstralBody>().astralStats.CurrentHealth / (float)parentObject.GetComponent<AstralBody>().astralStats.MaxHealth;
            healthText.text = $"{parentObject.GetComponent<AstralBody>().astralStats.CurrentHealth} / {parentObject.GetComponent<AstralBody>().astralStats.MaxHealth}";

            if (parentObject.GetComponent<AstralBody>().astralStats.MaxMana != 0)
            {
                manaAndConditionImage.fillAmount = (float)parentObject.GetComponent<AstralBody>().astralStats.CurrentMana / (float)parentObject.GetComponent<AstralBody>().astralStats.MaxMana;
                manaAndConditionText.text = $"{parentObject.GetComponent<AstralBody>().astralStats.CurrentMana} / {parentObject.GetComponent<AstralBody>().astralStats.MaxMana}";
            }
            else if (parentObject.GetComponent<AstralBody>().astralStats.MaxCondition != 0)
            {
                manaAndConditionImage.fillAmount = (float)parentObject.GetComponent<AstralBody>().astralStats.CurrentCondition / (float)parentObject.GetComponent<AstralBody>().astralStats.MaxCondition;
                manaAndConditionText.text = $"{parentObject.GetComponent<AstralBody>().astralStats.CurrentCondition} / {parentObject.GetComponent<AstralBody>().astralStats.MaxCondition}";
            }
            else
            {
                manaAndConditionImage.fillAmount = 0;
                manaAndConditionText.text = " ";
            }

            if (parentObject.GetComponent<AstralBody>().astralStats.Damage > parentObject.GetComponent<AstralBody>().astralStats.cardData.Damage)
            {
                DamageText.color = new Color(0f, 0.7f, 0.4f);
            }
            else if (parentObject.GetComponent<AstralBody>().astralStats.Damage < parentObject.GetComponent<AstralBody>().astralStats.cardData.Damage)
            {
                DamageText.color = new Color(0.95f, 0.2f, 0f);
            }
            else
            {
                DamageText.color = Color.black;
            }

            ManageStatusEffectIcon();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetParentObject(GameObject parentObject)
    {
        this.parentObject = parentObject;
        transform.position = parentObject.transform.GetChild(2).position + new Vector3(0, 1.3f, 0);

        if (parentObject.GetComponent<AstralBody>().masterPlayerTag == "Player") // 영체 태그에 따른 체력바의 색깔 변경
        {
            healthImage.color = new Color(0f, 0.8f, 0.4f);
        }
        else if (parentObject.GetComponent<AstralBody>().masterPlayerTag == "Opponent")
        {
            healthImage.color = new Color(0.95f, 0.2f, 0f);
        }

        if (parentObject.GetComponent<AstralBody>().astralStats.MaxMana != 0) // 영체가 충만, 조건인지에 따른 마나 및 조건바 색깔 변경
        {
            manaAndConditionImage.color = new Color(0.3f, 0.7f, 1f);
        }
        else if (parentObject.GetComponent<AstralBody>().astralStats.MaxCondition != 0)
        {
            manaAndConditionImage.color = new Color(0.6f, 0.3f, 1f);
        }
        else
        {
            manaAndConditionImage.color = new Color(0f, 0f, 0f);
            manaAndConditionText.text = " ";
        }
    }
    public void ManageStatusEffectIcon()
    {
        if (parentObject.GetComponent<AstralBody>().astralStatusEffect.stunDuration > 0 && !isStun)
        {
            isStun = true;
            stun = Instantiate(stunIcon);
        }
        else if (parentObject.GetComponent<AstralBody>().astralStatusEffect.stunDuration <= 0)
        {
            isStun = false;
            Destroy(stun);
        }
        if (isStun)
        {
            stun.transform.SetParent(statusEffectContent.transform, false);
            stun.GetComponentInChildren<TextMeshProUGUI>().text = parentObject.GetComponent<AstralBody>().astralStatusEffect.stunDuration.ToString();
        }

        if (parentObject.GetComponent<AstralBody>().astralStatusEffect.declainDuration > 0 && !isDeclain)
        {
            isDeclain = true;
            declain = Instantiate(declainIcon);
        }
        else if (parentObject.GetComponent<AstralBody>().astralStatusEffect.declainDuration <= 0)
        {
            isDeclain = false;
            Destroy(declain);
        }
        if (isDeclain)
        {
            declain.transform.SetParent(statusEffectContent.transform, false);
            declain.GetComponentInChildren<TextMeshProUGUI>().text = parentObject.GetComponent<AstralBody>().astralStatusEffect.declainDuration.ToString();
        }

        if (parentObject.GetComponent<AstralBody>().astralStatusEffect.sealDuration > 0 && !isSeal)
        {
            isSeal = true;
            seal = Instantiate(sealIcon);
        }
        else if (parentObject.GetComponent<AstralBody>().astralStatusEffect.sealDuration <= 0)
        {
            isSeal = false;
            Destroy(seal);
        }
        if (isSeal)
        {
            seal.transform.SetParent(statusEffectContent.transform, false);
            seal.GetComponentInChildren<TextMeshProUGUI>().text = parentObject.GetComponent<AstralBody>().astralStatusEffect.sealDuration.ToString();
        }

        if (parentObject.GetComponent<AstralBody>().astralStatusEffect.punishDuration > 0 && !isPunish)
        {
            isPunish = true;
            punish = Instantiate(punishIcon);
        }
        else if (parentObject.GetComponent<AstralBody>().astralStatusEffect.punishDuration <= 0)
        {
            isPunish = false;
            Destroy(punish);
        }
        if (isPunish)
        {
            punish.transform.SetParent(statusEffectContent.transform, false);
            punish.GetComponentInChildren<TextMeshProUGUI>().text = parentObject.GetComponent<AstralBody>().astralStatusEffect.punishDuration.ToString();
        }

        if (parentObject.GetComponent<AstralBody>().astralStatusEffect.encroachmentDuration > 0 && !isEncroachment)
        {
            isEncroachment = true;
            encroachment = Instantiate(encroachmentIcon);
        }
        else if (parentObject.GetComponent<AstralBody>().astralStatusEffect.encroachmentDuration <= 0)
        {
            isEncroachment = false;
            Destroy(encroachment);
        }
        if (isEncroachment)
        {
            encroachment.transform.SetParent(statusEffectContent.transform, false);
            encroachment.GetComponentInChildren<TextMeshProUGUI>().text = parentObject.GetComponent<AstralBody>().astralStatusEffect.encroachmentDuration.ToString();
        }

        if (parentObject.GetComponent<AstralBody>().astralStatusEffect.invincibilityDuration > 0 && !isInvincibility)
        {
            isInvincibility = true;
            invincibility = Instantiate(invincibilityIcon);
        }
        else if (parentObject.GetComponent<AstralBody>().astralStatusEffect.invincibilityDuration <= 0)
        {
            isInvincibility = false;
            Destroy(invincibility);
        }
        if (isInvincibility)
        {
            invincibility.transform.SetParent(statusEffectContent.transform, false);
            invincibility.GetComponentInChildren<TextMeshProUGUI>().text = parentObject.GetComponent<AstralBody>().astralStatusEffect.invincibilityDuration.ToString();
        }
    }
}
