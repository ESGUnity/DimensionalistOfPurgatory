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

            if (IsStun) // ���� ����Ʈ��� ������Ʈ�� 3��° �ڽ��� StunPosition�� ��ġ�� �������� ����
            {
                transform.position = parentObject.transform.GetChild(2).position;
            }
            else
            {
                transform.position = parentObject.transform.position;
            }
        }
        else // ������ġ
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
