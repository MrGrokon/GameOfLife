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

    enum GameState{
        Init,
        Playing,
        Paused,
        DeadEnd
    };

    void Awake()
    {
        GOL_Grid = new CustomGrid(this.transform, 50, 50, .25f);
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
                        GOL_Grid.GenerateNewGeneration();
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
                GOL_Grid.FlushGrid();
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
