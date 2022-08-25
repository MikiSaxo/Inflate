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

    private TouchScreenKeyboard keyboard;
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
        var namu = _name;
        
        if(namu == "")
            namu = $"Player {index + 1}";

        Manager.Instance.ChangePlayerName(index, namu);
    }

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
}
