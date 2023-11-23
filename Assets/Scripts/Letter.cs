using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Letter : MonoBehaviour
{
    int animatorResetTrigger = Animator.StringToHash("Reset");
    int animatorShakeTrigger = Animator.StringToHash("Shake");
    int animatorStateParameter = Animator.StringToHash("State");

     Animator m_animator = null;

     public TextMeshProUGUI letter = null;

     public char? Entry {get; private set;} = null;
     
     public LetterState state { get; private set; } = LetterState.Unknown;

     private void Awake(){
          letter = GetComponentInChildren<TextMeshProUGUI>();
          m_animator = GetComponent<Animator>();
         
     }

     private void Start(){
          letter.text = null;
     }

     public void EnterLetter(char c){
          Entry = c;
          letter.text = c.ToString().ToUpper();
     }

     public void DeleteLetter(){
          Entry = null;
          letter.text = null;
          m_animator.SetTrigger(animatorResetTrigger);
     }

     public void Shake(){
          m_animator.SetTrigger(animatorShakeTrigger);
     }

     public void SetState(LetterState state){
          this.state = state;
          m_animator.SetInteger(animatorStateParameter, (int)state);
     }
     
     public void Clear(){
          state = LetterState.Unknown;
          m_animator.SetInteger(animatorStateParameter, -1);
          m_animator.SetTrigger(animatorResetTrigger);
          Entry = null;
          letter.text = null;
     }
}
