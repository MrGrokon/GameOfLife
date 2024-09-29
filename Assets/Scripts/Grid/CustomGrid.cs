using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CustomGrid
{
    private Transform Parent;
    private int Width;
    private int Height;
    private float UniformCellSize = 10f;
    private Vector3 GridOrigine;

    //Type to be change for a tile class
    private int[,] GridArray;

    #region Instancieur
        public CustomGrid(Transform _parent, int _width, int _height, float _cellSize = 10f, Vector3 _origine = default(Vector3)){
            this.Parent = _parent;
            this.Width = _width;
            this.Height = _height;
            this.UniformCellSize = _cellSize;
            this.GridOrigine = _origine;
    
            InitializeGrid();
        }
    #endregion

    #region Methods
        private void InitializeGrid(){
            GridArray = new int[Width, Height];
            //Debug.Log(Width + " / " + Height);
            DrawGridRenderer();

            
        }

        public void Init(){
            for (int x = 0; x < GridArray.GetLength(0); x++){
                for (int y = 0; y < GridArray.GetLength(1); y++){
                    GridArray.SetValue(x, y, x*y);
                    Debug.Log("x: " + x + " / y: " + y + " value: " + x*y);

                }
            }
        }

        // 1 per inter collum && 1 per inter-lines 
        // finish by 1 to do every 4 external borders
        private void DrawDebugGrid(){
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

        private void DrawGridRenderer(){
            for (int x=1; x < GridArray.GetLength(0); x++){
                DrawLine(GetWorldPosition(0,x), GetWorldPosition(Height, x), "_line");
            }
            for (int y=1; y < GridArray.GetLength(0); y++){
                DrawLine(GetWorldPosition(y,0), GetWorldPosition(y, Width), "_collum_");
            }
            DrawBorders();
        }

        private void DrawLine(Vector3 _start, Vector3 _end, string _name = "___"){
                GameObject _line = GameObject.Instantiate(Resources.Load("_line_") as GameObject, _start, Quaternion.identity, Parent);
                _line.name = _name;
                LineRenderer _lr = _line.GetComponent<LineRenderer>();
                _lr.SetPosition(0, _start);
                _lr.SetPosition(1, _end);
        }

        private void DrawBorders(){
            GameObject _borders = GameObject.Instantiate(Resources.Load("_borders_") as GameObject, Parent);
            LineRenderer _lr = _borders.GetComponent<LineRenderer>();
            _lr.SetPosition(0, GetWorldPosition(0,0));
            _lr.SetPosition(1, GetWorldPosition(0,Width));
            _lr.SetPosition(2, GetWorldPosition(Height,Width));
            _lr.SetPosition(3, GetWorldPosition(Height,0));
        }

        #region Public Get/Set
            #region Using Indexes
              public void SetValue(int x, int y, int value){
                  if(x >=0 && y >= 0 && x < Width && y < Height){
                      //if the coordinate test existing in my grid
                      GridArray[x, y] = value;
                  }
                  else
                    Debug.Log("Warning Something fucked up");
              }
      
              public int GetValue(int x, int y){
                  if(x >=0 && y >= 0 && x < Width && y < Height){
                      return GridArray[x,y];
                  }
                  else{
                      Debug.Log("ERROR: No Cell at this index");
                      return -1;
                  }
              }
            #endregion
        
            #region Using World Coordinate
                public void SetValue(Vector3 _worlPosition, int value){
                    int _x, _y;
                    GetXY(_worlPosition, out _x, out _y);
                    SetValue(_x, _y, value);
                }
    
                public int GetValue(Vector3 _worldPosition){
                    int _x, _y;
                    GetXY(_worldPosition, out _x, out _y);
                    return GetValue(_x, _y);
                }
            #endregion
        #endregion
    #endregion

    #region private Get/Set
        private Vector3 GetWorldPosition(int x, int y){
            return new Vector3(x,y) * UniformCellSize + GetPositionAnchoredAtCenter();
        }

        private void GetXY(Vector3 _worldPosition, out int x, out int y){
            x = Mathf.FloorToInt( (_worldPosition - GetPositionAnchoredAtCenter()).x / UniformCellSize);
            y = Mathf.FloorToInt( (_worldPosition - GetPositionAnchoredAtCenter()).y / UniformCellSize);
        }

        private Vector3 GetPositionAnchoredAtCenter(){
            Vector3 _v = new Vector3();
            _v.x = Width * UniformCellSize / 2;
            _v.y = Height * UniformCellSize / 2;
            _v.z = 0f;
            return GridOrigine - _v;
        }
    #endregion
}
