using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class GameFlowManager : MonoBehaviour
{
    const int wordLength = 5;
    const int amountOfRows = 6;

    [SerializeField]
    [Tooltip("Prefab for the letter")]
    public Letter letterPrefab = null;

    [SerializeField]
    [Tooltip("Board")]
    public GameObject board = null;

    [SerializeField]
    [Tooltip("Word repository")]
    public WordRepository wordRepository = null;

    [SerializeField]
    public CanvasGroup gameOver = null;

    [SerializeField]
    Key[] keys = null;

    List<Letter> letters = null;
    Row[] rows;
    int index = 0;
    int currentRow = 0;
    char?[] guess = new char?[wordLength];
    char[] wordle = new char[wordLength];

    public PuzzleState puzzleState{get; private set;} = PuzzleState.InProgress;

    void Awake(){
        rows = board.GetComponentsInChildren<Row>();
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        gameOver.blocksRaycasts=false;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetWord();
        SetupGrid();
        foreach(Key key in keys){
            key.Pressed += OnKeyPressed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown){
            ParseInput(Input.inputString);
        }
    }

    void OnKeyPressed(KeyCode keyCode){
        if(puzzleState != PuzzleState.InProgress){
            if(keyCode == KeyCode.Return){
                Restart();
            }
            return;
        }
        if(keyCode == KeyCode.Return){
            GuessWord();
        }else if(keyCode == KeyCode.Backspace || keyCode == KeyCode.Delete){
            DeleteLetter();
        }else if(keyCode >= KeyCode.A && keyCode <= KeyCode.Z){
           int i = keyCode - KeyCode.A;
           EnterLetter((char)((int)'A' + i));
        }
    }

    public void ParseInput(string s){
        if(puzzleState != PuzzleState.InProgress){
            return;
        }
        foreach(char c in s){
            if(c == '\b'){
                DeleteLetter();
            }else if(c == '\n' || c == '\r'){
                
                GuessWord();
            }else{
                EnterLetter(c);
            }
        }
    }

    public void SetupGrid(){
        if(letters == null){
            letters = new List<Letter>();
        }
         Debug.Log(rows[0]);
        // Debug.Log(rows[0].GetComponentInChildren<Letter>().letter);
        for(int i = 0; i < amountOfRows; i++){
            for(int j = 0; j < wordLength; j++){
                //Debug.Log(rows[0].letters[j]);
                letters.Add(rows[i].letters[j]);
            }
        }
    }

    public void SetWord(){
        string w = wordRepository.GetRandomWord();
        for(int i = 0; i < wordLength; i++){
            wordle[i] = w[i];
        }
    }

    public void EnterLetter(char c){
        if(index < wordLength){
            c = char.ToLower(c);
            letters[(currentRow * wordLength) + index].EnterLetter(c);
            guess[index] = c;
            index++;
        }
    }
    public void DeleteLetter(){
        if(index > 0){
            index--;
            letters[(currentRow * wordLength) + index].DeleteLetter();
            guess[index] = null;
        }
    }

    public void Shake(){
        for(int i=0; i<wordLength; i++){
            letters[(currentRow * wordLength) + i].Shake();
        }
    }

    public void GuessWord(){
        char[] remaining = new char[wordLength];
        LetterState[] letterStates =new  LetterState[wordLength];
        for(int i=0; i<wordLength; i++){
            remaining[i] = wordle[i];
        }
        if(index != wordLength){
            Shake();
        }else{
            StringBuilder wordBuilder = new StringBuilder();
            for(int i=0; i<wordLength; i++){
                wordBuilder.Append(guess[i].Value);
            }
            if(wordRepository.CheckWordExists(wordBuilder.ToString())){
                bool incorrect = false;
                for(int i=0; i<wordLength; i++){
                    if(guess[i] == wordle[i]){
                        letterStates[i] = LetterState.Correct;
                        remaining[i] = ' ';
                    }
                }
                for(int i=0; i<wordLength; i++){
                    if(guess[i] != wordle[i]){
                        incorrect = true;
                        bool contains = false;
                        for(int j=0; j<wordLength; j++){
                            if(guess[i] == remaining[j]){
                                letterStates[i] = LetterState.WrongLocation;
                                remaining[j] = ' ';
                                contains=true;
                                break;
                            }
                        }
                        if(!contains){
                            letterStates[i] = LetterState.Incorrect;
                        }
                    }
                }

                for(int i=0; i<wordLength; i++){
                    StartCoroutine(PlayLetterAnimation(i*0.5f,currentRow * wordLength +i, letterStates[i], i));
                }

                if(incorrect){
                    index = 0;
                    currentRow++;
                    if(currentRow >= amountOfRows){
                        puzzleState = PuzzleState.Failed;
                    }
                }else{
                    puzzleState = PuzzleState.Complete;
                }
            }else{
                Shake();
            }
        }   
    }

     public void GameOver(){
        //board.SetActive(false);
        gameOver.interactable = true;
        gameOver.blocksRaycasts = true;
        StartCoroutine(Fade(gameOver, 1f, 3f));
    }

    public void Restart(){
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        gameOver.blocksRaycasts=false;

        board.SetActive(true);
        puzzleState = PuzzleState.InProgress;

        foreach(Letter l in letters){
            l.Clear();
        }
        foreach(Key key in keys){
           key.ResetState();
        }
        index = 0;
        currentRow = 0; 
        for(int i = 0; i<wordLength; i++){
            guess[i] = null;
        }
        SetWord();
    }


    private IEnumerator PlayLetterAnimation(float offset, int index, LetterState letterState, int counter){
        yield return new WaitForSeconds(offset);
        letters[index].SetState(letterState);

        string s = ""+letters[index].Entry;

        foreach(Key key in keys){
           string s2 = key.GetKeyCode.ToString();
            if(s.ToUpper() == s2){
                key.SetState(letterState);
                break;
            }
        }
        if(puzzleState != PuzzleState.InProgress && counter == 4){
            GameOver();
        }
    }

     private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay){
        yield return new WaitForSeconds(delay);
       
        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;
        
        while(elapsed < duration){
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
