using UnityEngine;

public class VFXFollowParent : MonoBehaviour
{
    GameObject parentObject;
    public bool IsStun = false;
    float remainTime = 3;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (parentObject != null)
        {
            remainTime = 3;

            if (IsStun) // 기절 이펙트라면 오브젝트의 3번째 자식인 StunPosition의 위치를 따르도록 설정
            {
                transform.position = parentObject.transform.GetChild(2).position;
            }
            else
            {
                transform.position = parentObject.transform.position;
            }
        }
        else // 안전장치
        {
            remainTime -= Time.deltaTime; 
            if (remainTime < 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void SetParentObject(GameObject parentObject)
    {
        this.parentObject = parentObject;
        transform.position = parentObject.transform.position;
    }
}
