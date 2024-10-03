using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife_Master : MonoBehaviour
{
    private CustomGrid GOL_Grid;

    [SerializeField][Range(0.01f, 1f)] public float GenerationLifeTime = 0.25f;
    private int GenerationCount = 0;
    [SerializeField] GameState MyGameState = GameState.Init;
    private float _elapsedTime;
    private bool _gameIsRunning;
    public Color BorderColor = Color.white;

    enum GameState{
        Init,
        Playing,
        Paused,
        DeadEnd
    };

    void Awake()
    {
        GOL_Grid = new CustomGrid(this.transform, 75, 75, .1f);
    }

    void Update()
    {
        if(GOL_Grid != null){
            switch (MyGameState)
            {
                case GameState.Init:
                    CheckChangeSimulationState(GameState.Playing);
                    InitializerInputs();
                break;

                case GameState.Playing:
                    _elapsedTime += Time.deltaTime;
                    if(_elapsedTime >= GenerationLifeTime){
                        GOL_Grid.RefreshGrid(out _gameIsRunning);
                        GenerationCount++;
                        _elapsedTime = 0f;
                    }
                break;

                case GameState.Paused:
                    CheckChangeSimulationState(GameState.Playing);
                    _elapsedTime = 0f;
                break;
            }
        }
        else
            Debug.Log("ERROR: Grid Undefined");
    }
    
    #region Input Actions
        private void CheckChangeSimulationState(GameState newState){
            if(Input.GetKeyDown(KeyCode.Space)){
                MyGameState = newState;
            }
        }

        private void InitializerInputs(){
            if(Input.GetMouseButtonDown(0)){
                GOL_Grid.ChangeValue(GetMousePosition());
            }

            if(Input.GetKeyDown(KeyCode.R)){
                GOL_Grid.ClearGrid();
            }

            if(Input.GetMouseButtonDown(1)){
                int _x, _y;
                GOL_Grid.GetXY(GetMousePosition(), out _x, out _y);
                Debug.Log("X: " + _x + " / Y: " + _y + " -> " + GOL_Grid.IsCellAliveNextState(_x, _y));
            }

            if(Input.GetKeyDown(KeyCode.A)){
                GOL_Grid.RefreshGrid(out _gameIsRunning);
                GenerationCount++;
            }
        }
    #endregion

    #region Misc
        private Vector3 GetMousePosition(){
            Vector3 _v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _v.z = 0f;
            return _v;
        }
    #endregion
}
