using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Manager : MonoBehaviour
{
    private int _randomNumber = 0;
    private int _choosenNumber = 0;
    private int _actualNumber = 0;
    private int _nbOfPlayers = 0;
    private int _whichTurn = 0;
    private int _actualgrow = 10;
    private bool someoneLose = false;
    private bool hasChooseGameMode = false;
    private bool hasChooseNbPlayers = false;
    private bool cannotPressInflate = false;

    [SerializeField] private TextMeshProUGUI[] choosenNb = null;
    [SerializeField] private GameObject balloon = null;
    [SerializeField] private GameObject aroundBalloon = null;
    [SerializeField] private GameObject[] instructions = null;
    [SerializeField] private string textDisplayTurn = null;
    [SerializeField] private GameObject prefabPlayers = null;
    [SerializeField] private GameObject parentPlayers = null;
    [SerializeField] private GameObject validateGreyButtonNb = null;
    [SerializeField] private GameObject inflateGreyButton = null;

    [SerializeField] private PlayerData[] playersData;

    private List<GameObject> stockPlayers = new List<GameObject>();

    [SerializeField] int nbDeMancheScoreMode = 5;
    [SerializeField] int howManyPlayers = 0;

    [Header("Random number for Balloon")]
    [SerializeField] int minBalloon = 0;
    [SerializeField] int maxBalloon = 0;

    [Header("Buttons Colors")]
    [SerializeField] private Color[] colorsButtons;
    //[SerializeField] private Image[] imgButtonsNotPress;
    //[SerializeField] private Image[] imgButtonsPress;
    [SerializeField] private GameObject[] buttonsNotPress;
    [SerializeField] private GameObject[] buttonsPress;
    
    [Header("Beggining Game")]
    [SerializeField] private GameObject Fade;
    [SerializeField] private GameObject[] title;
    [SerializeField] private GameObject uiGame;

    const float timeForAClickButton = .1f;

    [Header("Game Mode")]
    public GameMode gameMode;
    public enum GameMode
    {
        Score,
        Melee
    }

    private void Start()
    {
        //StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        Fade.SetActive(true);
        TransiAnim.Instance.MakeTransiOn();
        yield return new WaitForSeconds(TransiAnim.Instance.TimeTransi + .15f);
        title[0].SetActive(false);
        title[1].SetActive(false);
        uiGame.SetActive(true);
        TransiAnim.Instance.MakeTransiOff();
        _choosenNumber = 0;
        _nbOfPlayers = 1;
        DesacInflatOrNot(false);
        yield return new WaitForSeconds(1f);
        instructions[2].transform.DOScale(Vector3.one, .7f);
    }

    public void ChooseScoreMode()
    {
        if (hasChooseGameMode)
            return;

        gameMode = GameMode.Score;
        StartCoroutine(StartGame());

        hasChooseGameMode = true;
        title[2].SetActive(false);
        title[3].SetActive(true);
    }
    public void ChooseMeleeMode()
    {
        if (hasChooseGameMode)
            return;

        gameMode = GameMode.Melee;
        StartCoroutine(StartGame());

        hasChooseGameMode = true;
        title[2].SetActive(false);
        title[3].SetActive(true);
    }

    private void ChooseRandomNb()
    {
        _randomNumber = Random.Range(minBalloon, maxBalloon * _nbOfPlayers);
        print("randomNumber : " + _randomNumber);
    }

    public void OnClickPlus()
    {
        if (cannotPressInflate)
            return;

        if (hasChooseNbPlayers)
        {
            _choosenNumber++;
            _actualNumber++;
            //print((float)_choosenNumber / 10);
            ShakeAnim.Instance.StartZoom((float)_choosenNumber /30);
            validateGreyButtonNb.SetActive(false);

            if (_actualNumber > _randomNumber)
            {
                EndGame();
                return;
            }
            GrowBalloon(1);
        }
        else
        {
            if (_nbOfPlayers >= howManyPlayers)
                _nbOfPlayers = 0;

            _nbOfPlayers++;

            if (_nbOfPlayers > 1)
                validateGreyButtonNb.SetActive(false);
            else
                validateGreyButtonNb.SetActive(true);
        }
        ActualizeChoosenNb();
    }

    //public void OnClickMinus()
    //{
    //    if (hasChooseNbPlayers)
    //    {
    //        if (choosenNumber < 2)
    //            return;

    //        choosenNumber--;
    //    }
    //    else
    //    {
    //        if (nbOfPlayers < 1)
    //            return;

    //        nbOfPlayers--;
    //    }
    //    ActualizeChoosenNb();
    //}

    public void OnValidateNb()
    {
        if (_nbOfPlayers <= 1 || (_choosenNumber == 0 && hasChooseNbPlayers))
            return;

        if (hasChooseNbPlayers)
        {
            if (gameMode == GameMode.Score)
                stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().ActualizeScore(_choosenNumber);

            ChangeTurn();
            _choosenNumber = 0;
        }
        else
            StartCoroutine(TransiBeforeSpawn(_nbOfPlayers));

        ActualizeChoosenNb();
    }

    private void ActualizeChoosenNb()
    {
        if (hasChooseNbPlayers)
        {
            choosenNb[0].text = _choosenNumber.ToString();
            choosenNb[1].text = _choosenNumber.ToString();
        }
        else
        {
            choosenNb[0].text = _nbOfPlayers.ToString();
            choosenNb[1].text = _nbOfPlayers.ToString();
        }
    }

    void GrowBalloon(int _howManyToGrow)
    {
        _actualgrow += _howManyToGrow;
        float _convertGrow = _actualgrow;
        _convertGrow /= 8;
        Vector3 _grow = new Vector3(_convertGrow, _convertGrow, _convertGrow);
        balloon.transform.DOScale(_grow, .5f);
        PunchingBagAnim.Instance.Punch();
    }

    private IEnumerator DisplayWhosTurn(Color color)
    {
        DesacInflatOrNot(true);
        instructions[1].SetActive(true);
        instructions[0].transform.localPosition = new Vector3(270, -13, -30);
        instructions[1].transform.localPosition = new Vector3(270, -13, -30);

        instructions[0].GetComponent<TextMeshProUGUI>().text = $"<wave>J{_whichTurn}</wave>{textDisplayTurn}";
        instructions[1].GetComponent<TextMeshProUGUI>().text = $"<wave><color=#{ColorUtility.ToHtmlStringRGBA(color)}>J{_whichTurn}</color></wave>";
        instructions[2].transform.DOComplete();
        instructions[2].transform.DOScale(Vector3.one, .7f);
        yield return new WaitForSeconds(1.5f);
        instructions[2].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.3f);
        instructions[2].transform.DOScale(Vector3.zero, .25f);
        //instructions[1].transform.DOScale(Vector3.zero, .25f);
        DesacInflatOrNot(false);
    }

    private void ChangeTurn()
    {
        if (!someoneLose)
        {
            if (_whichTurn > 0)
                stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().MyTurnEnd();
        }
        else
        {
            _whichTurn--;
            someoneLose = false;
        }

        _whichTurn++;
        if (_whichTurn > _nbOfPlayers)
            _whichTurn = 1;

        stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().ItsMyTurn();
        validateGreyButtonNb.SetActive(true);
        StartCoroutine(DisplayWhosTurn(playersData[_whichTurn-1].Color));
    }

    private IEnumerator ResetGame()
    {
        DesacInflatOrNot(true);
        balloon.transform.DOKill();
        PunchingBagAnim.Instance.ResetPunch();

        yield return new WaitForSeconds(.1f);

        balloon.transform.localScale = Vector3.zero;
        ShakeAnim.Instance.StartShaking();

        yield return new WaitForSeconds(ShakeAnim.Instance.durationZoom);

        DesacInflatOrNot(true);
        balloon.transform.DOScale(Vector3.one, .5f);


        _actualgrow = 10;
        _choosenNumber = 0;
        _actualNumber = 0;

        ChooseRandomNb();
        ActualizeChoosenNb();
        ChangeTurn();
    }

    private void EndGame()
    {
        print("Lose");
        if (gameMode == GameMode.Score)
        {
            nbDeMancheScoreMode--;

            if (nbDeMancheScoreMode <= 0)
                print("Fin dla game ScoreMode");
        }
        else if (gameMode == GameMode.Melee)
        {
            someoneLose = true;
            balloon.transform.localScale = Vector3.one;

            StartCoroutine(MakePlayerMeleeDisappear());
            _nbOfPlayers--;
        }
        StartCoroutine(ResetGame());
    }

    IEnumerator MakePlayerMeleeDisappear()
    {
        stockPlayers[_whichTurn - 1].transform.DOMoveY(stockPlayers[_whichTurn - 1].transform.position.y + 2, .3f);
        yield return new WaitForSeconds(.3f);
        stockPlayers[_whichTurn - 1].SetActive(false);
        stockPlayers.RemoveAt(_whichTurn - 1);
    }

    IEnumerator TransiBeforeSpawn(int nbOfPlayers)
    {
        instructions[2].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.15f);
        TransiAnim.Instance.MakeTransiOn();
        yield return new WaitForSeconds(.15f);
        instructions[2].transform.DOScale(Vector3.zero, .25f);

        yield return new WaitForSeconds(TransiAnim.Instance.TimeTransi);
        TransiAnim.Instance.MakeTransiOff();
        yield return new WaitForSeconds(TransiAnim.Instance.TimeTransi / 2);
        SpawnPlayers(nbOfPlayers);
    }

    private void SpawnPlayers(int nbOfPlayers)
    {
        ChooseRandomNb();
        DesacInflatOrNot(true);
        for (int i = 0; i < nbOfPlayers; i++)
        {
            GameObject go = Instantiate(prefabPlayers, parentPlayers.transform);
            stockPlayers.Add(go);

            go.GetComponent<PlayerHimself>().Init(playersData[i].Name, playersData[i].Color, playersData[i].BG, i);

            if (gameMode == GameMode.Score)
                go.GetComponent<PlayerHimself>().ActualizeScore(0);
        }

        hasChooseNbPlayers = true;
        balloon.transform.DOScale(Vector3.one, 2f);
        aroundBalloon.SetActive(true);

        StartCoroutine(AnimDisappear());
        ActualizeChoosenNb();
        validateGreyButtonNb.SetActive(true);
    }

    IEnumerator AnimDisappear()
    {

        yield return new WaitForSeconds(_nbOfPlayers * .3f + .3f * 2);
        ChangeTurn();
    }


    //public void OnPointerEnter(int whichButton)
    //{
    //    if (whichButton == 0)
    //        imgButtonsNotPress[whichButton].DOColor(colorsButtons[1], timeToEnterHoverColorButtons);
    //    else
    //        imgButtonsNotPress[whichButton].DOColor(colorsButtons[2], timeToEnterHoverColorButtons);
    //}

    //public void OnPointerExit(int whichButton)
    //{
    //    imgButtonsNotPress[whichButton].DOColor(colorsButtons[0], timeToExitHoverColorButtons);
    //}

    public void OnPointerClick(int whichButton)
    {
        StartCoroutine(MakeAClick(whichButton));
    } //Relied to Buttons in UI

    private IEnumerator MakeAClick(int whichButton) //Animation for a click
    {
        buttonsNotPress[whichButton].gameObject.transform.localScale = Vector3.zero;
        buttonsPress[whichButton].gameObject.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(timeForAClickButton);

        buttonsNotPress[whichButton].gameObject.transform.localScale = Vector3.one;
        buttonsPress[whichButton].gameObject.transform.localScale = Vector3.zero;
    }

    private void DesacInflatOrNot(bool _tellMe)
    {
        inflateGreyButton.SetActive(_tellMe);
        cannotPressInflate = _tellMe;
    }
}
