using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class Key : MonoBehaviour
{
    [SerializeField]
    KeyCode keyCode = KeyCode.None;

    [Serializable]
    public struct LetterStateColor{
        public LetterState letterState;
        public Color color;
    }

    [SerializeField]
    LetterStateColor[] letterStateColors = null;

    Image image = null;

    Color startingColor = Color.grey;

    public Action<KeyCode> Pressed;

    void Awake(){
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if(text && string.IsNullOrEmpty(text.text)){
            text.text = keyCode.ToString();
        }
        image = GetComponent<Image>();
        startingColor = image.color;
    }

    private void OnButtonClick(){
        Pressed?.Invoke(keyCode);
    }

   public void SetState(LetterState state){
        foreach(LetterStateColor letterStateColor in letterStateColors){
            if(letterStateColor.letterState == state){
                image.color = letterStateColor.color;
                break;
            }
        }
   }

   public void ResetState(){
        image.color = startingColor;
   }

    public KeyCode GetKeyCode { get {return keyCode;}}
}
