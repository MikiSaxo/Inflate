using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputInit : MonoBehaviour
{
    [SerializeField] private Image BG = null;
    [SerializeField] TMP_InputField mainInputField;
    [SerializeField] int maxLenght;
    

    private int index;

    void Start()
    {
        mainInputField.characterLimit = maxLenght;
    }

    public void Init(Color _bgColor, int _index)
    {
        BG.color = _bgColor;
        index = _index;
        string _resetName = $"Player {index + 1}";
        Manager.Instance.ChangePlayerName(index, _resetName);
    }

    public void ReadInput(string _name)
    {
        Manager.Instance.ChangePlayerName(index, _name);
    }
}
