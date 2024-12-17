using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject AstralUI;
    public GameObject AstralCardPrefab;
    public GameObject PlayerAstralInfoNotifierPos; // 멀티를 고려하지 않은 부분!!!
    public GameObject OpponentAstralInfoNotifierPos;
    public GameObject BringerHealthUI;

    public GameObject ResultPanel;
    public TextMeshProUGUI ResultText;
    public GameObject StartPanel;

    GameObject astralCardPrefab;
    CardData cardData;

    static UIManager instance;
    public static UIManager Instance {  get { return instance; } }
    private void Awake()
    {
        instance = this;

        astralCardPrefab = Instantiate(AstralCardPrefab);
        astralCardPrefab.transform.SetParent(PlayerAstralInfoNotifierPos.transform, false);
        astralCardPrefab.transform.localPosition = Vector3.zero;
        astralCardPrefab.SetActive(false);
    }
    private void Start()
    {
        StartCoroutine(StartGame());
    }
    public void GenerateAstralUI(GameObject parentObject)
    {
        Instantiate(AstralUI).GetComponent<AstralUI>().SetParentObject(parentObject);
    }
    public void OnAstralInfoNotifier(GameObject astral)
    {
        astralCardPrefab.SetActive(true);
        this.cardData = astral.GetComponent<AstralBody>().cardData;
        astralCardPrefab.GetComponent<InteractableCard>().SetupCardData(cardData);
    }
    public void OffAstralInfoNotifier()
    {
        astralCardPrefab.SetActive(false);
    }
    public void GenerateBringerUI(GameObject bringerObject, BringerSystem bringerSystem)
    {
        GameObject go = Instantiate(BringerHealthUI);
        go.GetComponent<BringerUI>().SetParentObject(bringerObject, bringerSystem);
    }
    public void PlayerWin()
    {
        ResultPanel.SetActive(true);
        ResultText.text = "승리";
        Time.timeScale = 0.1f;
    }
    public void OpponentWin()
    {
        ResultPanel.SetActive(true);
        ResultText.text = "패배";
        Time.timeScale = 0.1f;
    }
    public void ClickedBackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    IEnumerator StartGame()
    {
        StartPanel.SetActive(true);
        yield return new WaitForSeconds(6f);
        StartPanel.SetActive(false);

    }
}
