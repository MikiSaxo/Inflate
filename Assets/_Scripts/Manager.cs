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
    private bool hasChooseNbPlayers = false;
    private bool cannotPressInflate = false;

    [SerializeField] private TextMeshProUGUI[] choosenNb = null;
    [SerializeField] private GameObject balloon = null;
    [SerializeField] private GameObject aroundBalloon = null;
    [SerializeField] private GameObject textHPlayers = null;
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
        _choosenNumber = 0;
        textHPlayers.transform.DOScale(Vector3.one, .7f);
        DesacInflatOrNot(false);
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

            if (_nbOfPlayers > 0)
                validateGreyButtonNb.SetActive(false);
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
        if (_nbOfPlayers == 0 || (_choosenNumber == 0 && hasChooseNbPlayers))
            return;

        if (hasChooseNbPlayers)
        {
            if (gameMode == GameMode.Score)
                stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().ActualizeScore(_choosenNumber);

            ChangeTurn();
            _choosenNumber = 0;
        }
        else
            SpawnPlayers(_nbOfPlayers);

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

    }

    private IEnumerator DisplayWhosTurn()
    {
        textHPlayers.GetComponent<TextMeshProUGUI>().text = $"</color=blue>J{_whichTurn}</color> {textDisplayTurn}"; //la couleur marche pas
        textHPlayers.transform.DOComplete();
        textHPlayers.transform.DOScale(Vector3.one, .7f);
        yield return new WaitForSeconds(1.5f);
        textHPlayers.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.3f);
        textHPlayers.transform.DOScale(Vector3.zero, .25f);
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
        StartCoroutine(DisplayWhosTurn());
    }

    private IEnumerator ResetGame()
    {
        DesacInflatOrNot(true);
        
        yield return new WaitForSeconds(.1f);
        
        balloon.transform.localScale = Vector3.zero;
        ShakeAnim.Instance.StartShaking();
        
        yield return new WaitForSeconds(ShakeAnim.Instance.duration);
        
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

            StartCoroutine(ResetGame());
        }
        else if (gameMode == GameMode.Melee)
        {
            someoneLose = true;
            balloon.transform.localScale = Vector3.one;

            StartCoroutine(MakePlayerMeleeDisappear());
            _nbOfPlayers--;

            StartCoroutine(ResetGame());
        }
    }

    IEnumerator MakePlayerMeleeDisappear()
    {
        stockPlayers[_whichTurn - 1].transform.DOMoveY(stockPlayers[_whichTurn - 1].transform.position.y + 2, .3f);
        yield return new WaitForSeconds(.3f);
        stockPlayers[_whichTurn - 1].SetActive(false);
        stockPlayers.RemoveAt(_whichTurn - 1);
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
        //balloon.SetActive(true);
        balloon.transform.DOScale(Vector3.one, 2f);
        aroundBalloon.SetActive(true);
        //textHPlayers.SetActive(false);
        StartCoroutine(AnimDisappear());

        ActualizeChoosenNb();
        validateGreyButtonNb.SetActive(true);


        //StartCoroutine(DisplayWhosTurn());
    }

    IEnumerator AnimDisappear()
    {
        textHPlayers.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .3f);
        yield return new WaitForSeconds(.3f);
        textHPlayers.transform.DOScale(Vector3.zero, .25f);
        yield return new WaitForSeconds(.8f);
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
