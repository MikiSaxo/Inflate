using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerHimself : MonoBehaviour
{
    int actualScore;

    public GameObject Visu, ScoreMode;
    public TextMeshProUGUI Name;
    public Image Contour;

    [SerializeField] int turnDecalage;
    [SerializeField] float timeTurnDecalage;

    public void LaunchMovePlayer(float decalage)
    {
        StartCoroutine(MoveSpawnPlayer(decalage));
    }

    IEnumerator MoveSpawnPlayer(float decalage)
    {
        //print(decalage);
        yield return new WaitForSeconds(.3f + decalage /5);
        Visu.transform.DOMoveY(Visu.transform.position.y - 160, 1f);
    }

    public void ItsMyTurn()
    {
        Visu.transform.DOMoveY(Visu.transform.position.y - turnDecalage, timeTurnDecalage);
    }

    public void MyTurnEnd()
    {
        Visu.transform.DOMoveY(Visu.transform.position.y + turnDecalage, timeTurnDecalage);
    }

    public void ActualizeScore(int score)
    {
        actualScore += score;
        ScoreMode.GetComponent<TextMeshProUGUI>().text = actualScore.ToString();
    }
}