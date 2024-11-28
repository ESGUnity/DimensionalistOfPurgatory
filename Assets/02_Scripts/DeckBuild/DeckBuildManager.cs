using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeckBuildManager : MonoBehaviour
{
    [SerializeField] Canvas mainCanvas;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] CardDataBase cardDataBase;
    [SerializeField] GameObject deckBuildScreen;
    [SerializeField] ScrollRect wholeCardScrollRect;
    [SerializeField] ScrollRect deckNamePlateScrollRect;
    [SerializeField] Transform wholeCardContent;
    [SerializeField] Transform deckNamePlateContent;
    [SerializeField] GameObject astralPrefab;
    [SerializeField] GameObject prayPrefab;
    [SerializeField] GameObject namePlatePrefab;
    [SerializeField] TextMeshProUGUI warningMessage;
    [SerializeField] GameObject suggestSaveDeckMessageBox;
    [SerializeField] TextMeshProUGUI deckName;

    [SerializeField] GameObject viewMyAllCardsScreen;
    [SerializeField] Transform viewMyAllCardsContent;

    GraphicRaycaster deckBuildScreenRaycaster;

    HashSet<int> selectedDeckCardId; // ��ġ�� ī�尡 ������ �̿��� �����ϴ� �ؽ���
    int deckNumber;
    GameObject repositionedCard;
    Coroutine CurrentAlramDeckSaved;
    Coroutine CurrentWarningAlreadyDeckMax;

    private static DeckBuildManager instance;
    public static DeckBuildManager Instance {  get { return instance; } }
    void Awake()
    {
        instance = this;

        deckBuildScreenRaycaster = mainCanvas.GetComponent<GraphicRaycaster>();
    }

    public void StartViewMyCards()
    {
        viewMyAllCardsScreen.SetActive(true);

        foreach (CardData card in cardDataBase.CardDataList)
        {
            GameObject go = card.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab); // ī�尡 ��ü���� �⵵���� Ȯ���ϰ� �� Ÿ�Կ� �´� �������� ����.
            go.GetComponent<DeckBuildInteractableCard>().SetupCardData(card);
            go.GetComponent<DeckBuildInteractableCard>().IsViewOnly = true;
            go.transform.SetParent(viewMyAllCardsContent);
        }

        SortWholeCardContent(viewMyAllCardsContent);
    }
    public void StartDeckBuild(int deckNumber) // �� ���� ȭ�� ����
    {
        deckBuildScreen.SetActive(true); // �� ���� ȭ�� ����
        FillWholeCardContent(); // ������ ī��(����� ī�� ��ü �� ��) ��� ����

        this.deckNumber = deckNumber; // �� �ѹ� ����
        selectedDeckCardId = LoadDeckInLocal(deckNumber); // �� �ҷ�����
        FillDeckNamePlateContent(selectedDeckCardId); // �ҷ��� ������ �� ����Ʈ ä���

        deckName.text = $"{deckNumber}�� ��";
        warningMessage.gameObject.SetActive(false); // �˸� �޽��� ��Ȱ��ȭ
    }
    void FillWholeCardContent() // ���� ���� ī�� ���
    {
        foreach (CardData card in cardDataBase.CardDataList)
        {
            GameObject go = card.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab); // ī�尡 ��ü���� �⵵���� Ȯ���ϰ� �� Ÿ�Կ� �´� �������� ����.
            go.GetComponent<DeckBuildInteractableCard>().SetupCardData(card);
            go.transform.SetParent(wholeCardContent);
        }

        SortWholeCardContent(wholeCardContent);
    } 
    void FillDeckNamePlateContent(HashSet<int> selectedDeck)
    {
        if (selectedDeck != null)
        {
            foreach (int cardId in selectedDeck)
            {
                CardData selectedCardData = (CardData)(cardDataBase.CardDataList.Find(card => card.Id == cardId)).Clone();
                GameObject go = Instantiate(namePlatePrefab);
                go.GetComponent<DeckBuildNamePlate>().SetCardInfo(selectedCardData);
                go.transform.SetParent(deckNamePlateContent);
            }
        }
        SortDeckContent();
    } // ������ ���� ī�� ���
    public void StartRepositionCard(GameObject cardPrefab, CardData cardData)
    {
        wholeCardScrollRect.enabled = false;
        deckNamePlateScrollRect.enabled = false;

        repositionedCard = cardData.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab);
        repositionedCard.GetComponent<DeckBuildInteractableCard>().SetupCardData(cardData);
        repositionedCard.transform.SetParent(deckBuildScreen.transform);
    } // ī�� �̵� ����
    public void OnRepositionCard()
    {
        if (repositionedCard != null)
        {
            repositionedCard.transform.position = Input.mousePosition;
        }
    } // ī�� �̵� ��
    public void StopRepositionCard(GameObject prefab) // ī�� �̵� ����. DeckContent ������ ���콺�� �ôٸ� ���� ����
    {
        if (CheckDuplicate(repositionedCard.GetComponent<DeckBuildInteractableCard>().cardData) && CheckCursorOverNamePlateContent())
        {
            wholeCardScrollRect.enabled = true;
            deckNamePlateScrollRect.enabled = true;
            Destroy(repositionedCard);

            return;
        }

        if (CheckDuplicate(repositionedCard.GetComponent<DeckBuildInteractableCard>().cardData) && !CheckCursorOverNamePlateContent())
        {
            if (selectedDeckCardId.Contains(repositionedCard.GetComponent<DeckBuildInteractableCard>().cardData.Id))
            {
                selectedDeckCardId.Remove(repositionedCard.GetComponent<DeckBuildInteractableCard>().cardData.Id);
                Destroy(prefab);
            }

            wholeCardScrollRect.enabled = true;
            deckNamePlateScrollRect.enabled = true;
            Destroy(repositionedCard);

            return;
        }

        if (CheckCursorOverNamePlateContent())
        {
            if (selectedDeckCardId.Count >= 25) // �ִ� ����� �Ѱ�ٸ�
            {
                if (CurrentWarningAlreadyDeckMax == null)
                {
                    CurrentWarningAlreadyDeckMax = StartCoroutine(WarningAlreadyDeckMax());
                }
                else
                {
                    StopCoroutine(CurrentWarningAlreadyDeckMax);
                    CurrentWarningAlreadyDeckMax = StartCoroutine(WarningAlreadyDeckMax());
                }
            }
            else
            {
                selectedDeckCardId.Add(repositionedCard.GetComponent<DeckBuildInteractableCard>().cardData.Id);
                GameObject go = Instantiate(namePlatePrefab);
                go.GetComponent<DeckBuildNamePlate>().SetCardInfo(repositionedCard.GetComponent<DeckBuildInteractableCard>().cardData);
                go.transform.SetParent(deckNamePlateContent);
                SortDeckContent();
            }
        }

        wholeCardScrollRect.enabled = true;
        deckNamePlateScrollRect.enabled = true;

        Destroy(repositionedCard);
    } 
    public bool CheckDuplicate(CardData cardData) // ī�尡 ���� �����ϴ��� �ߺ� �˻�
    {
        int id = cardData.Id;

        return selectedDeckCardId.Contains(id);
    } 
    void SaveDeckInLocal(int deckNumber)
    {
        string fileName = $"MyDeck{deckNumber}";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = JsonConvert.SerializeObject(selectedDeckCardId);
            File.WriteAllText(path, json);
        }
    } // �� ����
    public HashSet<int> LoadDeckInLocal(int deckNumber)
    {
        string fileName = $"MyDeck{deckNumber}";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<HashSet<int>>(json);
        }
        else
        {
            string json = JsonConvert.SerializeObject(new HashSet<int>());
            File.WriteAllText(path, json); // ù �����̾ ������ ���ٸ� ���� ����.
        }

        return new HashSet<int>();
    } // �� �ҷ�����
    void SortDeckContent() // �� ��� ����
    {
        int totalChildren = deckNamePlateContent.transform.childCount;

        for (int i = 0; i < totalChildren - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < totalChildren; j++)
            {
                if (deckNamePlateContent.GetChild(j).gameObject.GetComponent<DeckBuildNamePlate>().cardData.Id < deckNamePlateContent.GetChild(minIndex).gameObject.GetComponent<DeckBuildNamePlate>().cardData.Id)
                {
                    minIndex = j;
                }
            }

            if (minIndex != i)
            {
                deckNamePlateContent.GetChild(minIndex).SetSiblingIndex(i);
                deckNamePlateContent.GetChild(i + 1).SetSiblingIndex(minIndex + 1);
            }
        }
    } 
    void SortWholeCardContent(Transform content) // ��ü ī�� ����
    {
        int totalChildren = content.transform.childCount;

        for (int i = 0; i < totalChildren - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < totalChildren; j++)
            {
                if (content.GetChild(j).gameObject.GetComponent<DeckBuildInteractableCard>().cardData.Id < content.GetChild(minIndex).gameObject.GetComponent<DeckBuildInteractableCard>().cardData.Id)
                {
                    minIndex = j;
                }
            }

            if (minIndex != i)
            {
                content.GetChild(minIndex).SetSiblingIndex(i);
                content.GetChild(i + 1).SetSiblingIndex(minIndex + 1);
            }
        }
    } 
    bool CheckCursorOverNamePlateContent()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        deckBuildScreenRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("DeckNamePlate"))
            {
                return true;
            }
        }

        return false;
    } // Ŀ���� DeckContent ���� �ִ��� �˻�


    // ��ư�� ���� �Լ� ����
    public void OnClickGoBackSelectDeckMenu()
    {
        if (selectedDeckCardId == LoadDeckInLocal(deckNumber)) // ���� �̹� ������ �� ���¶�� (�� �۵����ϳ�.. �̹� �����ߴٸ�)
        {
            TerminateDeckBuildScreen();
        }
        else
        {
            suggestSaveDeckMessageBox.SetActive(true);
        }
    }
    public void TerminateDeckBuildScreen()
    {
        if (wholeCardContent.childCount > 0) // ��ü ī�� ����
        {
            for (int i = 0; i < wholeCardContent.childCount; i++)
            {
                Destroy(wholeCardContent.GetChild(i).gameObject);
            }
        }
        if (deckNamePlateContent.childCount > 0) // �� ��� ����
        {
            for (int i = 0; i < deckNamePlateContent.childCount; i++)
            {
                Destroy(deckNamePlateContent.GetChild(i).gameObject);
            }
        }

        selectedDeckCardId.Clear(); // �� ��� ������ �����
        deckNumber = 0; // ���ѹ� �ʱ�ȭ

        suggestSaveDeckMessageBox.SetActive(false);
        deckBuildScreen.SetActive(false); // ������ ȭ�� ����
    }
    public void SaveDeckAndTerminateDeckBuildScreen()
    {
        SaveDeckInLocal(deckNumber);
        TerminateDeckBuildScreen();
    }
    public void OnClickSaveDeck()
    {
        SaveDeckInLocal(deckNumber);

        if (CurrentAlramDeckSaved == null)
        {
            CurrentAlramDeckSaved = StartCoroutine(AlramDeckSaved());
        }
        else
        {
            StopCoroutine(CurrentAlramDeckSaved);
            CurrentAlramDeckSaved = StartCoroutine(AlramDeckSaved());
        }
    }
    public void OnClickGoBackMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void OnClickGoBackViewMyAllCardsToMainMenu()
    {
        if (viewMyAllCardsContent.childCount > 0) // ��ü ī�� ����
        {
            for (int i = 0; i < viewMyAllCardsContent.childCount; i++)
            {
                Destroy(viewMyAllCardsContent.GetChild(i).gameObject);
            }
        }

        viewMyAllCardsScreen.SetActive(false);
    }



    // �ڷ�ƾ ����
    IEnumerator WarningAlreadyDeckMax()
    {
        warningMessage.gameObject.SetActive(true);
        warningMessage.text = "���� ī�尡 �̹� �ִ� ����� 25�忡 �����߽��ϴ�.";

        yield return new WaitForSeconds(3f);

        warningMessage.gameObject.SetActive(false);
    }

    IEnumerator AlramDeckSaved()
    {
        warningMessage.gameObject.SetActive(true);
        warningMessage.text = $"���� �����߽��ϴ�.\n�� �̸� : {deckNumber}�� ��";

        yield return new WaitForSeconds(3f);

        warningMessage.gameObject.SetActive(false);
    }

}
