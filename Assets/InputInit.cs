using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputInit : MonoBehaviour
{
    [SerializeField] private Image BG = null;
    private int index;

    public void Init(Color _bgColor, int _index)
    {
        BG.color = _bgColor;
        index = _index;
        string _resetName = $"Player {index}";
        Manager.Instance.ChangePlayerName(index, _resetName);
    }

    public void ReadInput(string _name)
    {
        Manager.Instance.ChangePlayerName(index, _name);
    }
}
