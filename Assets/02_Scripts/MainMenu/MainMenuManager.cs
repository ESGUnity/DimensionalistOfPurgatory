using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    //public TextMeshProUGUI NickNameInput;
    //public GameObject LoginPanel;
    public GameObject SelectDeckPanel;
    public GameObject StartBattlePanel;
    public TextMeshProUGUI CurrentSelectedDeckNameText;
    public Image Deck1Btn;
    public Image Deck2Btn;
    public Image Deck3Btn;
    public Image Deck4Btn;
    public Image Deck5Btn;
    public Image Deck6Btn;
    public GameObject SelectedBtnEdge;
    public TextMeshProUGUI ErrorMessage;
    public GameObject SelectDifficultyPanel;

    int deckNumber = 0;
    Coroutine errorMessageCoroutine;
    GameObject selectedBtnEdge;

    static MainMenuManager instance;
    public static MainMenuManager Instance {  get { return instance; } }
    private void Awake()
    {
        instance = this;

        selectedBtnEdge = Instantiate(SelectedBtnEdge);
        selectedBtnEdge.transform.SetParent(SelectDeckPanel.transform);
        selectedBtnEdge.transform.SetAsFirstSibling();
        selectedBtnEdge.transform.localScale = Vector3.one;
        selectedBtnEdge.SetActive(false);
    }

    private void Update()
    {
        switch (deckNumber)
        {
            case 0:
                StartBattlePanel.SetActive(false);
                selectedBtnEdge.SetActive(false);
                Deck1Btn.color = new Color(0.8f, 0.8f, 0.8f, 1f); // 임시. 제대로 UI 만들 때 수정할 것
                Deck2Btn.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                Deck3Btn.color = new Color(0.8f, 0.8f, 0.8f, 1f); 
                Deck4Btn.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                Deck5Btn.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                Deck6Btn.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                break;
            case 1:
                selectedBtnEdge.transform.position = Deck1Btn.transform.position;
                Deck1Btn.color = Color.white; // 임시. 제대로 UI 만들 때 수정할 것
                break;
            case 2:
                selectedBtnEdge.transform.position = Deck2Btn.transform.position;
                Deck2Btn.color = Color.white;
                break;
            case 3:
                selectedBtnEdge.transform.position = Deck3Btn.transform.position;
                Deck3Btn.color = Color.white;
                break;
            case 4:
                selectedBtnEdge.transform.position = Deck4Btn.transform.position;
                Deck4Btn.color = Color.white;
                break;
            case 5:
                selectedBtnEdge.transform.position = Deck5Btn.transform.position;
                Deck5Btn.color = Color.white;
                break;
            case 6:
                selectedBtnEdge.transform.position = Deck6Btn.transform.position;
                Deck6Btn.color = Color.white;
                break;
        }
    }
    public void OnClickStartGame()
    {
        SelectDeckPanel.SetActive(true);
    }
    public void ActivateStartGameButton(IDeck deck)
    {
        if (deck.DeckCount < 25)
        {
            deckNumber = 0;

            if (errorMessageCoroutine == null)
            {
                errorMessageCoroutine = StartCoroutine(CantSelectDeck());
            }
            else
            {
                StopCoroutine(errorMessageCoroutine);
                errorMessageCoroutine = StartCoroutine(CantSelectDeck());
            }
        }
        else
        {
            deckNumber = deck.DeckNumber;
            DeckManager.Instance.SetPlayerDeckNumber(deckNumber);

            StartBattlePanel.SetActive(true);
            SelectedBtnEdge.SetActive(true);
            CurrentSelectedDeckNameText.text = $"{deck.DeckName}으로 대전하기";
        }
        //deckNumber = deck.DeckNumber;
        //DeckManager.Instance.SetPlayerDeckNumber(deckNumber);

        //StartBattlePanel.SetActive(true);
        //selectedBtnEdge.SetActive(true);
        //CurrentSelectedDeckNameText.text = $"{deck.DeckName}으로 대전하기";
    }
    public void OnClickStartBattle()
    {
        SelectDifficultyPanel.SetActive(true);
    }
    public void OnClickBackToMainMenu()
    {
        deckNumber = 0; // 메인 메뉴로 갈 시 선택한 덱 초기화
        SelectDeckPanel.SetActive(false);
    }
    public void OnClickBasementToInitialSelectDeck()
    {
        deckNumber = 0;
    }
    public void OnClickBackToSelectDeck()
    {
        SelectDifficultyPanel.SetActive(false);
    }
    public void OnClickDifficulty(int difficulty)
    {
        DeckManager.Instance.SetOpponentDeckNumber(difficulty);
        SceneManager.LoadScene("BattleScene");
    }
    public void OnClickDeckBuild()
    {
        SceneManager.LoadScene("DeckBuildScene");
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
    IEnumerator CantSelectDeck()
    {
        ErrorMessage.gameObject.SetActive(true);
        ErrorMessage.text = "25장으로 이뤄지지 않은 덱은 사용할 수 없습니다.";

        yield return new WaitForSeconds(3f);

        ErrorMessage.gameObject.SetActive(false);
    }
}
