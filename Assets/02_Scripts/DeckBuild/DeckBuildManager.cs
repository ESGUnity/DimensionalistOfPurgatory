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
    [SerializeField] TextMeshProUGUI deckNameInputField;
    [SerializeField] TextMeshProUGUI deckNameText;
    [SerializeField] TextMeshProUGUI currentCardCountText;

    [SerializeField] GameObject viewMyAllCardsScreen;
    [SerializeField] Transform viewMyAllCardsContent;

    GraphicRaycaster deckBuildScreenRaycaster;

    HashSet<int> selectedDeckCardId; // 겹치는 카드가 없도록 미연에 방지하는 해쉬셋
    int deckNumber;
    GameObject repositionedCard;
    Coroutine CurrentAlramDeckSaved;
    Coroutine CurrentWarningAlreadyDeckMax;
    IDeck currentBuildingDeck;

    private static DeckBuildManager instance;
    public static DeckBuildManager Instance {  get { return instance; } }
    void Awake()
    {
        instance = this;

        deckBuildScreenRaycaster = mainCanvas.GetComponent<GraphicRaycaster>();
    }

    private void Update()
    {
        if (currentBuildingDeck != null)
        {
            currentBuildingDeck.DeckCount = selectedDeckCardId.Count;
            currentCardCountText.text = $"{currentBuildingDeck.DeckCount} / 25";
        }
    }
    public void StartViewMyCards()
    {
        viewMyAllCardsScreen.SetActive(true);

        foreach (CardData card in cardDataBase.CardDataList)
        {
            GameObject go = card.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab); // 카드가 영체인지 기도인지 확인하고 각 타입에 맞는 프리팹을 생성.
            go.GetComponent<DeckBuildInteractableCard>().SetupCardData(card);
            go.GetComponent<DeckBuildInteractableCard>().IsViewOnly = true;
            go.transform.SetParent(viewMyAllCardsContent, false);
        }

        SortWholeCardContent(viewMyAllCardsContent);
    }
    public void StartDeckBuild(IDeck deck) // 덱 빌드 화면 세팅
    {
        currentBuildingDeck = deck;

        deckBuildScreen.SetActive(true); // 덱 빌드 화면 띄우기
        FillWholeCardContent(); // 보유한 카드(현재는 카드 전체 다 줌) 목록 띄우기

        deckNumber = deck.DeckNumber; // 덱 넘버 설정
        selectedDeckCardId = LoadDeckInLocal(deckNumber); // 덱 불러오기
        FillDeckNamePlateContent(selectedDeckCardId); // 불러온 덱으로 덱 리스트 채우기

        deckNameInputField.text = deck.DeckName;
        deckNameText.text = deck.DeckName;
        warningMessage.gameObject.SetActive(false); // 알림 메시지 비활성화
    }
    void FillWholeCardContent() // 보유 중인 카드 목록
    {
        foreach (CardData card in cardDataBase.CardDataList)
        {
            GameObject go = card.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab); // 카드가 영체인지 기도인지 확인하고 각 타입에 맞는 프리팹을 생성.
            go.GetComponent<DeckBuildInteractableCard>().SetupCardData(card);
            go.transform.SetParent(wholeCardContent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);
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
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);
            }
        }
        SortDeckContent();
    } // 선택한 덱의 카드 목록
    public void StartRepositionCard(GameObject cardPrefab, CardData cardData)
    {
        wholeCardScrollRect.enabled = false;
        deckNamePlateScrollRect.enabled = false;

        repositionedCard = cardData.IsAstral ? Instantiate(astralPrefab) : Instantiate(prayPrefab);
        repositionedCard.GetComponent<DeckBuildInteractableCard>().SetupCardData(cardData);
        repositionedCard.transform.SetParent(deckBuildScreen.transform);
        repositionedCard.transform.localScale = Vector3.one;
        repositionedCard.transform.localPosition = new Vector3(repositionedCard.transform.localPosition.x, repositionedCard.transform.localPosition.y, 0f);
    } // 카드 이동 시작
    public void OnRepositionCard()
    {
        if (repositionedCard != null)
        {
            Vector3 worldPosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mainCanvas.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out worldPosition))
            {
                repositionedCard.transform.position = worldPosition;
            }
        }
    } // 카드 이동 중
    public void StopRepositionCard(GameObject prefab) // 카드 이동 종료. DeckContent 위에서 마우스를 뗐다면 덱에 저장
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
            if (selectedDeckCardId.Count >= 25) // 최대 장수를 넘겼다면
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
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);
                SortDeckContent();
            }
        }

        wholeCardScrollRect.enabled = true;
        deckNamePlateScrollRect.enabled = true;

        Destroy(repositionedCard);
    } 
    public bool CheckDuplicate(CardData cardData) // 카드가 덱에 존재하는지 중복 검사
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
    } // 덱 저장
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
            File.WriteAllText(path, json); // 첫 실행이어서 파일이 없다면 파일 생성.
        }

        return new HashSet<int>();
    } // 덱 불러오기
    void SortDeckContent() // 덱 목록 정렬
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
    void SortWholeCardContent(Transform content) // 전체 카드 정렬
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
    } // 커서가 DeckContent 위에 있는지 검사


    // 버튼에 대한 함수 모음
    public void OnClickGoBackSelectDeckMenu()
    {
        if (selectedDeckCardId == LoadDeckInLocal(deckNumber)) // 만약 이미 저장이 된 상태라면 (잘 작동안하네.. 이미 저장했다면)
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
        if (wholeCardContent.childCount > 0) // 전체 카드 삭제
        {
            for (int i = 0; i < wholeCardContent.childCount; i++)
            {
                Destroy(wholeCardContent.GetChild(i).gameObject);
            }
        }
        if (deckNamePlateContent.childCount > 0) // 덱 목록 삭제
        {
            for (int i = 0; i < deckNamePlateContent.childCount; i++)
            {
                Destroy(deckNamePlateContent.GetChild(i).gameObject);
            }
        }

        selectedDeckCardId.Clear(); // 덱 목록 깨끗히 지우기
        deckNumber = 0; // 덱넘버 초기화

        suggestSaveDeckMessageBox.SetActive(false);
        deckBuildScreen.SetActive(false); // 덱빌드 화면 끄기
    }
    public void SaveDeckAndTerminateDeckBuildScreen()
    {
        SaveDeckInLocal(deckNumber);
        currentBuildingDeck.SaveDeckInfo();
        currentBuildingDeck.SetDeckInfo();
        TerminateDeckBuildScreen();
    }
    public void OnClickSaveDeck()
    {
        SaveDeckInLocal(deckNumber);
        currentBuildingDeck.SaveDeckInfo();
        currentBuildingDeck.SetDeckInfo();

        if (CurrentAlramDeckSaved == null) // 덱이 저장됨을 알려주는 텍스트
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
        if (viewMyAllCardsContent.childCount > 0) // 전체 카드 삭제
        {
            for (int i = 0; i < viewMyAllCardsContent.childCount; i++)
            {
                Destroy(viewMyAllCardsContent.GetChild(i).gameObject);
            }
        }

        viewMyAllCardsScreen.SetActive(false);
    }
    public void OnClickDeckNameChange()
    {
        currentBuildingDeck.DeckName = deckNameInputField.text;
        currentBuildingDeck.SaveDeckInfo();
        currentBuildingDeck.SetDeckInfo();
        deckNameText.text = currentBuildingDeck.DeckName;
    }


    // 코루틴 모음
    IEnumerator WarningAlreadyDeckMax()
    {
        warningMessage.gameObject.SetActive(true);
        warningMessage.text = "덱의 카드가 이미 최대 장수인 25장에 도달했습니다";

        yield return new WaitForSeconds(3f);

        warningMessage.gameObject.SetActive(false);
    }

    IEnumerator AlramDeckSaved()
    {
        warningMessage.gameObject.SetActive(true);
        warningMessage.text = $"덱을 저장했습니다";

        yield return new WaitForSeconds(3f);

        warningMessage.gameObject.SetActive(false);
    }

}
