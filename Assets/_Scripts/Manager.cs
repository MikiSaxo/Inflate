using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    private int _randomNumber = 0;
    private int _choosenNumber = 0;
    private int _actualNumber = 0;
    private int _nbOfPlayers = 0;
    private int _whichTurn = 0;
    private int _actualgrow = 10;
    private int _lastScore = 0;
    private int _lastKing = -1;
    private int _maxScore = 0;
    private int _whichMaxScore = 0;
    private bool someoneLose = false;
    private bool hasChooseGameMode = false;
    private bool hasChooseNbPlayers = false;
    private bool hasChooseNamePlayers = false;
    private bool hasGameEnded = false;
    private bool hasChooseNbOfTurn = false;
    private bool cannotPressInflate = false;
    private bool cannotPressValidate = false;

    [SerializeField] private TextMeshProUGUI[] choosenNb = null;
    [SerializeField] private GameObject[] balloon = null;
    [SerializeField] private GameObject[] licorne = null;
    [SerializeField] private GameObject aroundBalloon = null;
    [SerializeField] private GameObject backButton = null;
    [SerializeField] private GameObject[] instructions = null;
    [SerializeField] private string textHowManyTurn = null;
    [SerializeField] private string textDisplayTurn = null;
    [SerializeField] private string textDisplayTurnLeft = null;
    [SerializeField] private string textDisplayWon = null;
    [SerializeField] private GameObject prefabInput = null;
    [SerializeField] private GameObject prefabPlayers = null;
    [SerializeField] private GameObject[] parentInput = null;
    [SerializeField] private GameObject parentPlayers = null;
    [SerializeField] private GameObject parentFX = null;
    [SerializeField] private GameObject validateGreyButtonNb = null;
    [SerializeField] private GameObject inflateGreyButton = null;
    [SerializeField] private GameObject exploBalloonFX = null;
    [SerializeField] private Color[] colorsBalloon;

    [SerializeField] private PlayerData[] playersData;

    private List<GameObject> stockPlayers = new List<GameObject>();

    [SerializeField] private int nbOfTurn = 5;
    [SerializeField] private int howManyPlayers = 0;

    [Header("Random number for Balloon")]
    [SerializeField] private int minBalloon = 0;
    [SerializeField] private int maxBalloon = 0;

    [Header("Buttons Colors")]
    [SerializeField] private Color[] colorsButtons;
    [SerializeField] private GameObject[] buttonsNotPress;
    [SerializeField] private GameObject[] buttonsPress;
    private Color invisibleText = Color.black;


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

    public static Manager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        invisibleText.a = 0;
        TransiAnim.Instance.MakeTransiOff();
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
        title[5].SetActive(false);
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
        title[4].SetActive(false);
    }
    private void ChooseRandomNb()
    {
        _randomNumber = Random.Range(minBalloon, maxBalloon * _nbOfPlayers);
        minBalloon++;
        maxBalloon++;
        print("randomNumber : " + _randomNumber);
    }

    public void OnClickPlus()
    {
        if (cannotPressInflate)
            return;

        FX_Inflate.Instance.Start_FX_Inflate();
        if (hasChooseNbPlayers && hasChooseNbOfTurn)
        {
            _choosenNumber++;
            _actualNumber++;
            ShakeAnim.Instance.StartZoom((float)_choosenNumber / 30);
            DesacValidateOrNot(false);

            if (_actualNumber > _randomNumber)
            {
                EndGame();
                return;
            }
            GrowBalloon(1);
        }
        else
        {
            if (!hasChooseNbOfTurn)
            {
                if (_nbOfPlayers >= howManyPlayers)
                    _nbOfPlayers = 0;

                _nbOfPlayers++;

                if (_nbOfPlayers > 1)
                    DesacValidateOrNot(false);
                else
                {
                    validateGreyButtonNb.transform.localScale = Vector3.one;
                    cannotPressValidate = true;
                }
            }
            else
            {
                if (nbOfTurn >= 10)
                    nbOfTurn = 1;

                nbOfTurn++;
            }
        }
        ActualizeChoosenNb();
    }

    public void OnValidateNb()
    {
        if (_nbOfPlayers <= 1 || (_choosenNumber == 0 && hasChooseNbPlayers) || cannotPressValidate)
            return;

        if (hasChooseNbPlayers && hasChooseNbOfTurn)
        {
            if (gameMode == GameMode.Score)
                stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().ActualizeScore(_choosenNumber);

            if (stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().actualScore > _lastScore && gameMode == GameMode.Score)
            {
                if (_lastKing >= 0)
                    stockPlayers[_lastKing].GetComponent<PlayerHimself>().ActiCrownOrNot(false);

                stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().ActiCrownOrNot(true);

                _lastKing = _whichTurn - 1;
                _lastScore = stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().actualScore;
            }

            DesacValidateOrNot(true);
            ChangeTurn();
            _choosenNumber = 0;
        }
        else
        {
            if (!hasChooseNbOfTurn)
            {
                if(gameMode == GameMode.Score)
                    StartCoroutine(ChooseNBOfTurn());
                else
                    StartCoroutine(ChoosePlayerName(_nbOfPlayers));

                hasChooseNbOfTurn = true;
                DesacInflatOrNot(true);
                return;
            }
            else
            {
                StartCoroutine(ChoosePlayerName(_nbOfPlayers));
            }
        }

        ActualizeChoosenNb();
    }

    private void ActualizeChoosenNb()
    {
        if (hasChooseNbPlayers && hasChooseNbOfTurn)
        {
            choosenNb[0].text = _choosenNumber.ToString();
            choosenNb[1].text = _choosenNumber.ToString();
        }
        else
        {
            if (!hasChooseNbOfTurn)
            {
                choosenNb[0].text = _nbOfPlayers.ToString();
                choosenNb[1].text = _nbOfPlayers.ToString();
            }
            else
            {
                choosenNb[0].text = nbOfTurn.ToString();
                choosenNb[1].text = nbOfTurn.ToString();
            }
        }
    }

    void GrowBalloon(int _howManyToGrow)
    {
        _actualgrow += _howManyToGrow;
        float _convertGrow = _actualgrow;
        _convertGrow /= 8;
        Vector3 _grow = new Vector3(_convertGrow, _convertGrow, _convertGrow);
        balloon[0].transform.DOScale(_grow, .5f);
        PunchingBagAnim.Instance.Punch();
    }

    private IEnumerator DisplayWhosTurn(Color color)
    {
        DesacInflatOrNot(true);
        instructions[1].SetActive(true);
        instructions[1].transform.localScale = Vector3.one;

        instructions[0].GetComponent<TextMeshProUGUI>().text = $"<wave>{stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().Namee}</wave>{textDisplayTurn}";
        instructions[1].GetComponent<TextMeshProUGUI>().text = $"<wave><color=#{ColorUtility.ToHtmlStringRGBA(color)}>{stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().Namee}</color></wave><size=86.5f%><color=#{ColorUtility.ToHtmlStringRGBA(invisibleText)}>{textDisplayTurn}";
        instructions[2].transform.DOComplete();
        instructions[2].transform.DOScale(Vector3.one, .7f);

        yield return new WaitForSeconds(1.5f);
        instructions[2].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.3f);
        instructions[2].transform.DOScale(Vector3.zero, .25f);
        DesacInflatOrNot(false);
    }

    private IEnumerator DisplayHowManyTurnLeft()
    {
        instructions[0].GetComponent<TextMeshProUGUI>().text = $"<bounce><u>{nbOfTurn}</u> {textDisplayTurnLeft}";
        instructions[1].transform.localScale = Vector3.zero;
        instructions[2].transform.DOScale(Vector3.one, .7f);

        yield return new WaitForSeconds(1.5f);
        instructions[2].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.3f);
        instructions[2].transform.DOScale(Vector3.zero, .25f);
        yield return new WaitForSeconds(.35f);

        ChangeTurn();
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
        StartCoroutine(DisplayWhosTurn(stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().Colorr));
    }

    private IEnumerator ResetGame()
    {
        DesacInflatOrNot(true);

        validateGreyButtonNb.transform.localScale = Vector3.one;
        cannotPressValidate = true;

        balloon[0].transform.DOKill();
        PunchingBagAnim.Instance.ResetPunch();

        yield return new WaitForSeconds(.1f);

        balloon[0].transform.localScale = Vector3.zero;
        var _randomColor = Random.Range(0, colorsBalloon.Length);
        balloon[1].gameObject.GetComponent<Image>().color = colorsBalloon[_randomColor];
        GameObject go = Instantiate(exploBalloonFX, parentFX.transform);

        float _convertGrow = _actualgrow;
        _convertGrow /= 25;
        Vector3 _grow = new Vector3(_convertGrow, _convertGrow, _convertGrow);
        go.transform.localScale = _grow;

        ShakeAnim.Instance.StartShakingCam(_actualNumber);

        yield return new WaitForSeconds((ShakeAnim.Instance.durationShaking + _actualgrow) / 25);

        if (!hasGameEnded)
        {
            DesacInflatOrNot(true);
            balloon[0].transform.DOScale(Vector3.one, .5f);


            _actualgrow = 10;
            _choosenNumber = 0;
            _actualNumber = 0;

            ChooseRandomNb();
            ActualizeChoosenNb();
            if (gameMode == GameMode.Score)
                StartCoroutine(DisplayHowManyTurnLeft());
            else
                ChangeTurn();
        }
    }

    private void EndGame()
    {
        if (gameMode == GameMode.Score)
        {
            nbOfTurn--;

            if (nbOfTurn <= 0)
            {
                hasGameEnded = true;
                StartCoroutine(RestartWholeGame());
            }
        }
        else if (gameMode == GameMode.Melee)
        {
            someoneLose = true;
            balloon[0].transform.localScale = Vector3.one;

            StartCoroutine(MakePlayerMeleeDisappear());
            _nbOfPlayers--;
            if (_nbOfPlayers == 1)
            {
                hasGameEnded = true;
                StartCoroutine(RestartWholeGame());
            }
        }
        StartCoroutine(ResetGame());
    }

    IEnumerator RestartWholeGame()
    {
        instructions[1].SetActive(true);
        instructions[1].transform.localScale = Vector3.one;

        if (gameMode == GameMode.Score)
        {
            for (int i = 0; i < _nbOfPlayers; i++)
            {
                if (stockPlayers[i].GetComponent<PlayerHimself>().actualScore > _maxScore)
                {
                    _maxScore = stockPlayers[i].GetComponent<PlayerHimself>().actualScore;
                    _whichMaxScore = i;
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
            _whichMaxScore = 0;
        }

        instructions[0].GetComponent<TextMeshProUGUI>().text = $"<wave>{stockPlayers[_whichMaxScore].GetComponent<PlayerHimself>().Namee}</wave> {textDisplayWon}";
        instructions[1].GetComponent<TextMeshProUGUI>().text = $"<wave><color=#{ColorUtility.ToHtmlStringRGBA(stockPlayers[_whichMaxScore].GetComponent<PlayerHimself>().Colorr)}>{stockPlayers[_whichMaxScore].GetComponent<PlayerHimself>().Namee}</color></wave><size=86.5f%><color=#{ColorUtility.ToHtmlStringRGBA(invisibleText)}>{textDisplayWon}";
        instructions[2].transform.DOComplete();
        instructions[2].transform.DOScale(Vector3.one, .7f);
    }

    public void LaunchReStartGame()
    {
        StartCoroutine(ReStartGame());
    }
    IEnumerator ReStartGame()
    {
        instructions[2].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        TransiAnim.Instance.MakeTransiOn();
        yield return new WaitForSeconds(.3f);
        instructions[2].transform.DOScale(Vector3.zero, .25f);
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(0);
    }

    IEnumerator MakePlayerMeleeDisappear()
    {
        stockPlayers[_whichTurn - 1].transform.DOMoveY(stockPlayers[_whichTurn - 1].transform.position.y + 2, .3f);
        yield return new WaitForSeconds(.3f);
        stockPlayers[_whichTurn - 1].SetActive(false);
        stockPlayers.RemoveAt(_whichTurn - 1);
    }

    public void ChangePlayerName(int _index, string _name)
    {
        playersData[_index].Name = _name;
    }

    IEnumerator ChoosePlayerName(int nbOfPlayers)
    {
        DesacValidateOrNot(true);

        instructions[2].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.15f);
        TransiAnim.Instance.MakeTransiOn();
        yield return new WaitForSeconds(.15f);
        instructions[2].transform.DOScale(Vector3.zero, .25f);
        yield return new WaitForSeconds(TransiAnim.Instance.TimeTransi);

        uiGame.SetActive(false);
        parentInput[0].SetActive(true);
        for (int i = 0; i < nbOfPlayers; i++)
        {
            GameObject go = Instantiate(prefabInput, parentInput[1].transform);

            go.GetComponent<InputInit>().Init(playersData[i].BG, i);
        }

        TransiAnim.Instance.MakeTransiOff();
        yield return new WaitForSeconds(TransiAnim.Instance.TimeTransi / 2);

        DesacValidateOrNot(false);
    }

    IEnumerator ChooseNBOfTurn()
    {
        DesacValidateOrNot(true);

        instructions[0].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);

        yield return new WaitForSeconds(.3f);

        instructions[0].transform.DOScale(Vector3.zero, .25f);

        yield return new WaitForSeconds(.35f);

        backButton.transform.DOScale(Vector3.one, .7f);

        instructions[0].GetComponent<TextMeshProUGUI>().text = textHowManyTurn;
        instructions[0].transform.DOScale(Vector3.one, .7f);

        yield return new WaitForSeconds(.35f);
        ActualizeChoosenNb();
        yield return new WaitForSeconds(.35f);

        DesacValidateOrNot(false);
        DesacInflatOrNot(false);
    }

    public void LaunchTransiSpawn()
    {
        if (hasChooseNamePlayers)
            return;
        hasChooseNamePlayers = true;

        StartCoroutine(MakeAClick(2));
        StartCoroutine(TransiBeforeSpawn(_nbOfPlayers));
        //DesacValidateOrNot(true);
    }

    IEnumerator TransiBeforeSpawn(int nbOfPlayers)
    {
        instructions[2].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.15f);
        TransiAnim.Instance.MakeTransiOn();
        yield return new WaitForSeconds(.15f);
        instructions[2].transform.DOScale(Vector3.zero, .25f);

        yield return new WaitForSeconds(TransiAnim.Instance.TimeTransi);
        uiGame.SetActive(true);
        parentInput[0].SetActive(false);
        yield return new WaitForSeconds(TransiAnim.Instance.TimeTransi / 2);
        TransiAnim.Instance.MakeTransiOff();
        SpawnPlayers(nbOfPlayers);
    }

    private void SpawnPlayers(int nbOfPlayers)
    {
        DesacInflatOrNot(true);
        ChooseRandomNb();
        for (int i = 0; i < nbOfPlayers; i++)
        {
            GameObject go = Instantiate(prefabPlayers, parentPlayers.transform);
            stockPlayers.Add(go);

            go.GetComponent<PlayerHimself>().Init(playersData[i].Name, playersData[i].Color, playersData[i].BG, i);

            if (gameMode == GameMode.Score)
                go.GetComponent<PlayerHimself>().ActualizeScore(0);
        }

        hasChooseNbPlayers = true;
        balloon[0].transform.DOScale(Vector3.one, 2f);
        balloon[1].GetComponent<Image>().color = colorsBalloon[0];
        aroundBalloon.SetActive(true);

        StartCoroutine(AnimDisappear());
        ActualizeChoosenNb();

        validateGreyButtonNb.transform.localScale = Vector3.one;
        cannotPressValidate = true;
    }

    IEnumerator AnimDisappear()
    {
        yield return new WaitForSeconds(_nbOfPlayers * .3f * 3);
        if (gameMode == GameMode.Score)
            StartCoroutine(DisplayHowManyTurnLeft());
        else
            ChangeTurn();
    }

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

    private void DesacValidateOrNot(bool _tellMe)
    {
        //validateGreyButtonNb.SetActive(_tellMe);
        if (_tellMe)
        {
            title[6].SetActive(false);
            //validateGreyButtonNb.transform.localScale = Vector3.one;
            StartCoroutine(MakeAClick(1));
        }
        else
        {
            title[6].SetActive(true);
            validateGreyButtonNb.transform.localScale = Vector3.zero;
        }

        cannotPressValidate = _tellMe;
    }

    public void HasDiscoverEasterEgg()
    {
        GameObject go = Instantiate(exploBalloonFX, parentFX.transform);
        go.transform.localScale = Vector3.one * 2;

        licorne[0].SetActive(true);
        licorne[1].SetActive(true);
    }
}
