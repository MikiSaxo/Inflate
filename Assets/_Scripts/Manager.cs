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

    [SerializeField] private TextMeshProUGUI[] choosenNb = null;
    [SerializeField] private GameObject balloon = null;
    [SerializeField] private GameObject textHPlayers = null;
    [SerializeField] private GameObject prefabPlayers = null;
    [SerializeField] private GameObject parentPlayers = null;
    [SerializeField] private GameObject validateGreyButtonNb = null;

    [SerializeField] private PlayerData[] playersData;

    [SerializeField] private List<GameObject> stockPlayers = new List<GameObject>();

    [SerializeField] int nbDeMancheScoreMode = 5;

    [Header("Random number for Balloon")]
    [SerializeField] int minBalloon = 0;
    [SerializeField] int maxBalloon = 0;

    [Header("Buttons Colors")]
    [SerializeField] private Color[] colorsButtons;
    [SerializeField] private Image[] imgButtonsNotPress;
    [SerializeField] private Image[] imgButtonsPress;
    [SerializeField] private GameObject[] buttonsNotPress;
    [SerializeField] private GameObject[] buttonsPress;

    const float timeToEnterHoverColorButtons = .2f;
    const float timeToExitHoverColorButtons = .1f;
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
    }

    private void ChooseRandomNb()
    {
        _randomNumber = Random.Range(minBalloon, maxBalloon * _nbOfPlayers);
        print("randomNumber : " + _randomNumber);
    }

    public void OnClickPlus()
    {
        if (hasChooseNbPlayers)
        {
            _choosenNumber++;
            _actualNumber ++;
            if (_actualNumber > _randomNumber)
            {
                EndGame();
                return;
            }
            GrowBalloon(1);
        }
        else
        {
            if (_nbOfPlayers > 6)
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
        if (_nbOfPlayers == 0)
            return;

        if (hasChooseNbPlayers)
        {
            //_actualNumber += _choosenNumber;
            //if (_actualNumber > _randomNumber)
            //{
            //    EndGame();
            //    return;
            //}

            if (gameMode == GameMode.Score)
                stockPlayers[_whichTurn - 1].GetComponent<PlayerHimself>().ActualizeScore(_choosenNumber);

            //GrowBalloon(_choosenNumber);
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
        //print("convertGrow : " + convertGrow);
        Vector3 _grow = new Vector3(_convertGrow, _convertGrow, _convertGrow);
        //balloon.transform.localScale = _grow;
        balloon.transform.DOScale(_grow, .5f);
        
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
    }

    private void ResetGame()
    {
        balloon.transform.localScale = Vector3.zero;
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

            ResetGame();
        }
        else if (gameMode == GameMode.Melee)
        {
            someoneLose = true;
            balloon.transform.localScale = Vector3.one;

            stockPlayers[_whichTurn - 1].SetActive(false);
            stockPlayers.RemoveAt(_whichTurn - 1);
            _nbOfPlayers--;

            ResetGame();
        }
    }

    private void SpawnPlayers(int nbOfPlayers)
    {
        ChooseRandomNb();
        for (int i = 0; i < nbOfPlayers; i++)
        {
            GameObject go = Instantiate(prefabPlayers, parentPlayers.transform);
            stockPlayers.Add(go);

            go.GetComponent<PlayerHimself>().Init(playersData[i].Name, playersData[i].Color, i);

            if (gameMode == GameMode.Score)
                go.GetComponent<PlayerHimself>().ActualizeScore(0);
        }

        hasChooseNbPlayers = true;
        balloon.SetActive(true);
        textHPlayers.SetActive(false);

        ActualizeChoosenNb();
        StartCoroutine(LaunchTurnGame());
    }

    private IEnumerator LaunchTurnGame()
    {
        validateGreyButtonNb.SetActive(true);
        yield return new WaitForSeconds(2f);
        validateGreyButtonNb.SetActive(false);
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
}
