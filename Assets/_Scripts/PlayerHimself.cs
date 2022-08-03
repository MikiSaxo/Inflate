using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerHimself : MonoBehaviour
{
    [SerializeField] private GameObject Visu = null;
    [SerializeField] private TextMeshProUGUI Name = null;
    [SerializeField] private Image Contour = null;
    [SerializeField] private Image ContourScore = null;
    [SerializeField] private float turnDecalage;
    [SerializeField] private float timeTurnDecalage;

    private int actualScore = 0;
    private string namee = string.Empty;

    const float _moveYSpawn = 1f;

    public void Init(string _name, Color _color, float _decalage)
    {
        ActualizeName(_name);
        ContourScore.color = _color;
        Contour.color = _color;
        Contour.DOFade(0, .01f);
        LaunchMovePlayer(_decalage);
    }

    private void LaunchMovePlayer(float _decalage)
    {
        StartCoroutine(MoveSpawnPlayer(_decalage));
    }

    IEnumerator MoveSpawnPlayer(float _decalage)
    {
        yield return new WaitForSeconds(.3f + _decalage /5);
        Visu.transform.DOMoveY(Visu.transform.position.y - _moveYSpawn, 1f);
    }

    public void ItsMyTurn()
    {
        Visu.transform.DOComplete();
        Visu.transform.DOMoveY(Visu.transform.position.y - turnDecalage, timeTurnDecalage);
        Contour.DOFade(1f, .5f);
    }

    public void MyTurnEnd()
    {
        Visu.transform.DOComplete();
        Visu.transform.DOMoveY(Visu.transform.position.y + turnDecalage, timeTurnDecalage);
        Contour.DOFade(0, .5f);
    }

    public void ActualizeScore(int _score)
    {
        actualScore += _score;
        Name.text = $"{namee} : {_score}";
    }

    private void ActualizeName(string _name)
    {
        namee = _name;
        ActualizeScore(actualScore);
    }
}