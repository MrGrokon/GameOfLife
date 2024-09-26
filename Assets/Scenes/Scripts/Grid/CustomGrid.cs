using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid
{
    private int Width;
    private int Height;
    private float UniformCellSize = 10f;

    //Type to be change for a tile class
    private int[,] GridArray;

    #region Instancieur
        public CustomGrid(int _width, int _height, float _cellSize = 10f){
            this.Width = _width;
            this.Height = _height;
            this.UniformCellSize = _cellSize;
    
            InitializeGrid();
        }
    #endregion

    #region Methods
        private void InitializeGrid(){
            GridArray = new int[Width, Height];
            Debug.Log(Width + " / " + Height);
    
            for (int x = 0; x < GridArray.GetLength(0); x++){
                //Increment on array's Width
                for (int y = 0; y < GridArray.GetLength(1); y++){ 
                    //increment on array's Height 
    
                    Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1, y), Color.white, Mathf.Infinity);  
                    Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y+1), Color.white, Mathf.Infinity);  
                }
            }
            Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.white, Mathf.Infinity);  
            Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.white, Mathf.Infinity); 
    
        }
    #endregion

    #region Get/Set
        private Vector3 GetWorldPosition(int x, int y){
            return new Vector3(x,y) * UniformCellSize;
        }
    
        private void GetXY(Vector3 _worldPosition, out int x, out int y){
            x = Mathf.FloorToInt(_worldPosition.x / UniformCellSize);
            y = Mathf.FloorToInt(_worldPosition.y / UniformCellSize);
        }
    
        private void SetValue(int x, int y, int value){
            if(x >=0 && y >= 0 && x < Width && y < Height){
                //if the coordinate test existing in my grid
                GridArray[x, y] = value;
            }
        }
    
        private void SetValue(Vector3 _worlPosition, int value){
            int _x, _y;
            GetXY(_worlPosition, out _x, out _y);
            SetValue(_x, _y, value);
        }
    #endregion
}
