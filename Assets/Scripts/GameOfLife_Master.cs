using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife_Master : MonoBehaviour
{
    //User Defined
    [SerializeField][Range(0.01f, 1f)] float GenerationLifeTime = 0.25f;
    [SerializeField] Vector2Int GridSize;
    [SerializeField][Range(0.01f, 1f)] float LineThiccness = .2f;

    //private
    private CustomGrid GOL_Grid;
    private int GenerationCount = 0;
    GameState MyGameState = GameState.Init;
    private float _elapsedTime;
    private bool _gameIsRunning;
    private Pattern ActivePattern;

    //public
    public static GameOfLife_Master Instance;

    public enum GameState{
        Init,
        Playing,
        Paused,
        DeadEnd
    };

    void Awake()
    {
        SingletonInstance();
        GOL_Grid = new CustomGrid(this.transform, GridSize.x, GridSize.y, LineThiccness);
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
                GOL_Grid.DrawPattern(ActivePattern, GetMousePosition());
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
            GOL_Grid.ChangeGridRendererState(false);
        }

        public void PauseSimulation(){
            MyGameState = GameState.Paused;
            GOL_Grid.ChangeGridRendererState(true);
            _elapsedTime = 0f;
        }

        public void ChangePatternUsed(Pattern newPattern){
            ActivePattern = newPattern;
        }
    #endregion
    
}
