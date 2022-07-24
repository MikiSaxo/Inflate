using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Manager : MonoBehaviour
{
    int randomNumber, choosenNumber, actualNumber, nbOfPlayers, whichTurn;
    int actualgrow = 10;
    bool someoneLose, hasChooseNbPlayers;
    [SerializeField] TextMeshProUGUI[] choosenNb;
    [SerializeField] GameObject balloon, textHPlayers, prefabPlayers, parentPlayers, validateButtonNb;
    [SerializeField] PlayerData[] playersData;

    [SerializeField] List<GameObject> stockPlayers = new List<GameObject>();

    [SerializeField] int nbDeMancheScoreMode = 5;

    [Header("Random number for Balloon")]
    [SerializeField] int minBalloon;
    [SerializeField] int maxBalloon;

    [Header("Buttons Colors")]
    [SerializeField] Color[] colorsButtons;
    [SerializeField] Image[] imgButtonsNotPress;
    [SerializeField] Image[] imgButtonsPress;
    const float timeToEnterHoverColorButtons = .2f;
    const float timeToExitHoverColorButtons = .1f;
    const float timeForAClickButton = .1f;

    [Header("Game Mode")]
    public GameMode gameMode;

    private void Start()
    {
        choosenNumber = 1;

        //for (int i = 0; i < imgButtonsNotPress.Length; i++)
        //{
        //    imgButtonsNotPress[i].color = colorsButtons[0];
        //}
    }

    void ChooseRandomNb()
    {
        randomNumber = Random.Range(minBalloon, maxBalloon * nbOfPlayers);
        print("randomNumber : " + randomNumber);
    }

    public enum GameMode
    {
        Score,
        Melee
    }

    public void OnClickPlus()
    {
        if (hasChooseNbPlayers)
        {
            choosenNumber++;
        }
        else
        {
            if (nbOfPlayers > 6)
                return;
            nbOfPlayers++;
        }
        ActualizeChoosenNb();
    }

    public void OnClickMinus()
    {
        if (hasChooseNbPlayers)
        {
            if (choosenNumber < 2)
                return;

            choosenNumber--;
        }
        else
        {
            if (nbOfPlayers < 1)
                return;

            nbOfPlayers--;
        }
        ActualizeChoosenNb();
    }

    public void OnValidateNb()
    {
        if (hasChooseNbPlayers)
        {
            actualNumber += choosenNumber;
            if (actualNumber > randomNumber)
            {
                EndGame();
                return;
            }

            if (gameMode == GameMode.Score)
                stockPlayers[whichTurn - 1].GetComponent<PlayerHimself>().ActualizeScore(choosenNumber);

            GrowBalloon(choosenNumber);
            choosenNumber = 1;
        }
        else
            SpawnPlayers(nbOfPlayers);

        ActualizeChoosenNb();
    }

    void ActualizeChoosenNb()
    {
        if (hasChooseNbPlayers)
        {
            choosenNb[0].text = choosenNumber.ToString();
            choosenNb[1].text = choosenNumber.ToString();
        }
        else
        {
            choosenNb[0].text = nbOfPlayers.ToString();
            choosenNb[1].text = nbOfPlayers.ToString();
        }
    }

    void GrowBalloon(int HowManyToGrow)
    {
        actualgrow += HowManyToGrow;
        float convertGrow = actualgrow;
        convertGrow /= 8;
        //print("convertGrow : " + convertGrow);
        Vector3 grow = new Vector3(convertGrow, convertGrow, convertGrow);
        balloon.transform.localScale = grow;
        ChangeTurn();
    }

    void ChangeTurn()
    {
        if (!someoneLose)
        {
            if (whichTurn > 0)
                stockPlayers[whichTurn - 1].GetComponent<PlayerHimself>().MyTurnEnd();
        }
        else
        {
            whichTurn--;
            someoneLose = false;
        }

        whichTurn++;
        if (whichTurn > nbOfPlayers)
            whichTurn = 1;

        stockPlayers[whichTurn - 1].GetComponent<PlayerHimself>().ItsMyTurn();
    }

    void ResetGame()
    {
        balloon.transform.localScale = Vector3.one;
        choosenNumber = 1;
        actualNumber = 0;
        ChooseRandomNb();
        ActualizeChoosenNb();
        ChangeTurn();
    }

    void EndGame()
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

            stockPlayers[whichTurn - 1].SetActive(false);
            stockPlayers.RemoveAt(whichTurn - 1);
            nbOfPlayers--;

            ResetGame();
        }
    }

    void SpawnPlayers(int nbOfPlayers)
    {
        ChooseRandomNb();
        for (int i = 0; i < nbOfPlayers; i++)
        {
            GameObject go = Instantiate(prefabPlayers, parentPlayers.transform);
            stockPlayers.Add(go);
            go.GetComponent<PlayerHimself>().Name.text = playersData[i].Name;
            go.GetComponent<PlayerHimself>().Name.color = playersData[i].Color;
            go.GetComponent<PlayerHimself>().Contour.color = playersData[i].Color;
            if (gameMode == GameMode.Score)
            {
                go.GetComponent<PlayerHimself>().ScoreMode.SetActive(true);
                go.GetComponent<PlayerHimself>().ActualizeScore(0);
            }
            go.GetComponent<PlayerHimself>().LaunchMovePlayer(i);
        }

        hasChooseNbPlayers = true;
        balloon.SetActive(true);
        textHPlayers.SetActive(false);

        ActualizeChoosenNb();
        StartCoroutine(LaunchTurnGame());
    }

    IEnumerator LaunchTurnGame()
    {
        validateButtonNb.SetActive(true);
        yield return new WaitForSeconds(2f);
        validateButtonNb.SetActive(false);
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
    }

    IEnumerator MakeAClick(int whichButton)
    {
        imgButtonsNotPress[whichButton].gameObject.SetActive(false);
        imgButtonsPress[whichButton].gameObject.SetActive(true);

        yield return new WaitForSeconds(timeForAClickButton);

        imgButtonsNotPress[whichButton].gameObject.SetActive(true);
        imgButtonsPress[whichButton].gameObject.SetActive(false);
    }
}
