using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife_Master : MonoBehaviour
{
    private CustomGrid GOL_Grid;
    public static GameOfLife_Master Instance;

    [SerializeField][Range(0.01f, 1f)] public float GenerationLifeTime = 0.25f;
    private int GenerationCount = 0;
    [SerializeField] GameState MyGameState = GameState.Init;
    private float _elapsedTime;
    private bool _gameIsRunning;
    public Color BorderColor = Color.white;

    private Pattern _UsedPattern;

    public enum GameState{
        Init,
        Playing,
        Paused,
        DeadEnd
    };

    void Awake()
    {
        SingletonInstance();
        GOL_Grid = new CustomGrid(this.transform, 50, 50, .2f);
    }

    void Update()
    {
        if(GOL_Grid != null){
            switch (MyGameState)
            {
                case GameState.Init:
                    InitializerInputs();
                break;

                case GameState.Playing:
                    _elapsedTime += Time.deltaTime;
                    if(_elapsedTime >= GenerationLifeTime){
                        GOL_Grid.RefreshGrid(out _gameIsRunning);
                        IncrementGenCount();
                        _elapsedTime = 0f;
                        if(_gameIsRunning == false){
                            MyGameState = GameState.DeadEnd;
                            UI_Behaviors.Instance.DisplayGameOverScreen();
                        }
                    }
                break;
            }
        }
        else
            Debug.Log("ERROR: Grid Undefined");
    }
    
    #region Input Actions
        private void InitializerInputs(){
            if(Input.GetMouseButtonDown(0)){
                GOL_Grid.ChangeValue(GetMousePosition());
            }

            if(Input.GetMouseButtonDown(1)){
                int _x, _y;
                GOL_Grid.GetXY(GetMousePosition(), out _x, out _y);
                Debug.Log("X: " + _x + " / Y: " + _y + " -> " + GOL_Grid.IsCellAliveNextState(_x, _y));
            }

            if(Input.GetKeyDown(KeyCode.A)){
                GOL_Grid.RefreshGrid(out _gameIsRunning);
                IncrementGenCount();
            }

            if(Input.GetKeyDown(KeyCode.Space)){
                Debug.Log("Extent: " + _UsedPattern.GetExtent() + " Origine: " + _UsedPattern.GetOrigin() );
            }
        }
    #endregion

    #region Misc
        private Vector3 GetMousePosition(){
            Vector3 _v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _v.z = 0f;
            return _v;
        }

        private void IncrementGenCount(){
            GenerationCount++;
            UI_Behaviors.Instance.ChangeGenerationCounter(GenerationCount);
        }

        private void ResetGenCount(){
            GenerationCount = 0;
            UI_Behaviors.Instance.ChangeGenerationCounter(GenerationCount);
        }

        private void SingletonInstance(){
            if(Instance == null){
                Instance = this;
            }
            else{
                Destroy(this);
            }
        }

        public GameState GetGameState(){
            return MyGameState;
        }
    #endregion

    #region Public Entry Points
        public CustomGrid GetGrid(){
            return GOL_Grid;
        }

        public int GetGenCount(){
            return GenerationCount;
        }

        public void ProceedWithSimulation(){
            MyGameState = GameState.Playing;
        }

        public void PauseSimulation(){
            MyGameState = GameState.Paused;
            _elapsedTime = 0f;
        }

        public void ChangePatternUsed(Pattern newPattern){
            _UsedPattern = newPattern;
        }
    #endregion
    
}
