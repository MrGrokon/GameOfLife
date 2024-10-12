using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Behaviors : MonoBehaviour
{
    public static UI_Behaviors Instance;
    private Scene ActualScene;
    private TextMeshProUGUI GenerationCounter, DeathGenerationCounter;
    private Button ChangeState_Btn;
    private GameObject GameOverScreen;
    private GameObject InGameButtonsParent;
    [SerializeField] private List<Pattern> Patterns;
    private TMP_Dropdown PatternSelectionDropdown;

    #region UnityEvents
        private void Awake() {
            SingletonInstance();
            InitializeRefrences();
            InitSelectorDropdown();
        }

        private void Start() {
            Pattern _initPattern = Patterns[ PatternSelectionDropdown.value ];
            GameOfLife_Master.Instance.ChangePatternUsed(_initPattern);
        }
    #endregion

    #region Accessed From Outside
        public void ChangeGenerationCounter(int I){
            GenerationCounter.SetText("Generation Count: " + I);
        }

        public void DisplayGameOverScreen(){
            foreach (Transform child in InGameButtonsParent.transform){
                child.GetComponent<Button>().interactable = false;
            }
            DeathGenerationCounter.SetText(GameOfLife_Master.Instance.GetGenCount() + " Generation(s)");
            GameOverScreen.SetActive(true);
        }
    #endregion

    #region UI Buttons Actions
        public void ChangeGameState(){
            GameOfLife_Master.GameState _state = GameOfLife_Master.Instance.GetGameState();
            if(_state == GameOfLife_Master.GameState.Init || _state == GameOfLife_Master.GameState.Paused){
                //game is initializing or pause
                GameOfLife_Master.Instance.ProceedWithSimulation();
                ChangeBtnText(ChangeState_Btn, "Pause");
            }
            else{
                //game is running so i had to pause it
                GameOfLife_Master.Instance.PauseSimulation();
                ChangeBtnText(ChangeState_Btn, "Resume");
            }
        }

        public void RandomizeGameGrid(){
            GameOfLife_Master.Instance.GetGrid().RandomizeGrid();
        }
    
        public void ClearGameGrid(){
            GameOfLife_Master.Instance.GetGrid().ClearGrid();
        }
    
        public void ReloadScene(){
            SceneManager.LoadScene(ActualScene.name);
        }
    
        public void CloseGame(){
            Debug.Log("Game will now close");
            Application.Quit();
        }
    
        public void GetDropdownSelectorValue(){
            //get the pattern linked to the Dropdown
            Pattern _pattern = Patterns[PatternSelectionDropdown.value];

            //Pass it to the Game Master Script
            //It will used it to define how cells are spawned
            Debug.Log(_pattern.Name);
            GameOfLife_Master.Instance.ChangePatternUsed(_pattern);
        }
    #endregion

    #region Misc
        private void SingletonInstance(){
            if(Instance == null){
                Instance = this;
            }
            else{
                GameObject.Destroy(this.gameObject);
            }
        }

        private void ChangeBtnText(Button Btn, string NewText){
            //ChangeState_Btn.SetText = NewText;
            TextMeshProUGUI _text = Btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if(_text != null){
                _text.SetText(NewText);
            }
            else{
                Debug.Log("Warning: Button text not found");
            }
        }

        private void InitializeRefrences(){
            ActualScene = SceneManager.GetActiveScene();
            GenerationCounter = this.transform.Find("GenerationCounter").GetComponent<TextMeshProUGUI>();
            ChangeState_Btn = this.transform.Find("Buttons").Find("ChangeState").GetComponent<Button>();
            GameOverScreen = this.transform.Find("GameOverScreen").gameObject;
            InGameButtonsParent = this.transform.Find("Buttons").gameObject;
            DeathGenerationCounter = GameOverScreen.transform.Find("GenerationCounter").GetComponent<TextMeshProUGUI>();
            PatternSelectionDropdown = this.transform.Find("PatternSelector").GetComponent<TMP_Dropdown>();
        }

        private void InitSelectorDropdown(){
            //remove the empty option at the initial state of my dropdown
            PatternSelectionDropdown.options.RemoveAt(0);

            //add an option for each Pattern scriptable object passed to my code
            foreach (var _pattern in Patterns){
                TMP_Dropdown.OptionData _option = new TMP_Dropdown.OptionData(_pattern.Name, null); 
                PatternSelectionDropdown.options.Add(_option);
            }
        }
    #endregion
    
}
