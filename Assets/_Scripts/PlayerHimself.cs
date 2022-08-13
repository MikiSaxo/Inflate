using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerHimself : MonoBehaviour
{
    [SerializeField] private GameObject Visu = null;
    [SerializeField] private TextMeshProUGUI Score = null;
    [SerializeField] private TextMeshProUGUI NameMelee = null;
    [SerializeField] private TextMeshProUGUI NameScore = null;
    [SerializeField] private Image Contour = null;
    [SerializeField] private Image ContourScore = null;
    [SerializeField] private Image BG = null;
    [SerializeField] private float turnDecalage = 0f;
    [SerializeField] private float timeTurnDecalage = 0f;

    private int actualScore = 0;

    const float _moveYSpawn = 1.5f;

    public void Init(string _name, Color _color, Color _bgColor, float _decalage)
    {
        ActualizeName(_name);
        ContourScore.color = _color;
        Contour.color = _color;
        BG.color = _bgColor;
        Contour.DOFade(0, .01f);
        ContourScore.DOFade(0, .01f);
        LaunchMovePlayer(_decalage);
    }

    private void LaunchMovePlayer(float _decalage)
    {
        StartCoroutine(MoveSpawnPlayer(_decalage));
    }

    IEnumerator MoveSpawnPlayer(float _decalage)
    {
        yield return new WaitForSeconds(.3f + _decalage / 5);
        Visu.transform.DOMoveY(Visu.transform.position.y - _moveYSpawn, 1f);
    }

    public void ItsMyTurn()
    {
        Visu.transform.DOComplete();
        Visu.transform.DOMoveY(Visu.transform.position.y - turnDecalage, timeTurnDecalage);
        Contour.DOFade(1f, .5f);
        ContourScore.DOFade(1f, .5f);
    }

    public void MyTurnEnd()
    {
        Visu.transform.DOComplete();
        Visu.transform.DOMoveY(Visu.transform.position.y + turnDecalage, timeTurnDecalage);
        Contour.DOFade(0, .5f);
        ContourScore.DOFade(0, .5f);
    }

    public void ActualizeScore(int _score)
    {
        actualScore += _score;
        Score.text = $"- {actualScore} -";
    }

    private void ActualizeName(string _name)
    {
        if (Manager.Instance.gameMode == Manager.GameMode.Melee)
        {
            Score.text = "";
            NameMelee.text = _name;
        }
        else
        {
            ActualizeScore(actualScore);
            NameScore.text = _name;
        }
    }
}