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

    //this 2 dimension array will copy the height and with to contain indexed GO reference
    private GameObject[,] Cells;

    //will contain the alive/death states for every Tiles.
    //this could be remove and replaced by if(Cells[x,y] != null) ?
    private bool[,] GridArray;

    #region Instancieur
        public CustomGrid(Transform _parent, int _width, int _height, float _cellSize = 10f, Vector3 _origine = default(Vector3)){
            this.Parent = _parent;
            this.Width = _width;
            this.Height = _height;
            this.UniformCellSize = _cellSize;
            this.GridOrigine = _origine;
    
            InitializeGrid();
            RandomlyInitGrid();
        }

        private void InitializeGrid(){
            GridArray = new bool[Width, Height];
            Cells = new GameObject[Width, Height];
            //Debug.Log(Width + " / " + Height);
            DrawGridRenderer();
        }
    
        private void RandomlyInitGrid(bool _initValue = true){
            for (int x = 0; x < GridArray.GetLength(0); x++){
                //iterate through Width (Left -> Right)
                for (int y = 0; y < GridArray.GetLength(1); y++){
                    //iterate through Height (Bottom -> Top)
                    bool _b = Convert.ToBoolean( Mathf.FloorToInt(UnityEngine.Random.Range(0,2)) );
                    GridArray[x,y] = _b;
                    
                    if(_b)
                        DrawAliveCell(x, y);
                }
            }
        }
    #endregion

    #region Public Methods
        public void RefreshGrid(out bool GameIsRunning){
            FlushGridRendering(); //clear old generation rendered
            GenerateNewGeneration(); //compute 
            DrawAllAliveCells();
            GameIsRunning = true;
        }
    
        public void ClearGrid(){
            for (int x = 0; x < GridArray.GetLength(0); x++){
                for (int y = 0; y < GridArray.GetLength(1); y++){
                    GridArray[x,y] = false;
                    GameObject.Destroy(Cells[x,y]);
                }
            }
        }

        #region Public Get/Set
            public void ChangeValue(int x, int y){
                if(x >=0 && y >= 0 && x < Width && y < Height){
                    //if the coordinate test existing in my grid
                    //invert the value contained at this index
                    bool _b =!GridArray[x, y];
                    GridArray[x, y] = _b;
                    //draw a cell at the index
                    if(_b){
                        DrawAliveCell(x, y);
                    }
                    else{
                        GameObject.Destroy(Cells[x,y]);
                    }
                }
                else
                    Debug.Log("Warning Something fucked up");
                }
        
            public void ChangeValue(Vector3 _worlPosition){
                int _x, _y;
                GetXY(_worlPosition, out _x, out _y);
                ChangeValue(_x, _y);
            }
              
            public bool GetValue(int x, int y){
                if(x >=0 && y >= 0 && x < Width && y < Height){
                    return GridArray[x,y];
                }
                else{
                    Debug.Log("ERROR: No Cell at this index");
                }
                return false;
            }
            
            public bool GetValue(Vector3 _worldPosition){
                int _x, _y;
                GetXY(_worldPosition, out _x, out _y);
                return GetValue(_x, _y);
            }
        #endregion
    #endregion

    #region New Generation Computing
        private void GenerateNewGeneration(){
            bool[,] _nextGeneration = new bool[Width, Height];

            for (int x=0; x < GridArray.GetLength(0); x++){
                //iterate through Width (Left -> Right)
                for (int y=0; y < GridArray.GetLength(1); y++){
                    //iterate through Height (Bottom -> Top)
                    bool _isAlive = IsCellAliveNextState(x,y);
                    _nextGeneration[x,y] = _isAlive;
                    /*
                    //bad idead to mix computation and spawning ?
                    if(_isAlive){
                        DrawAliveCell(x,y);
                    }
                    */
                }
            }
            GridArray = _nextGeneration;
        }

        //public for debug reason
        public bool IsCellAliveNextState(int X, int Y, out string _msg){
                int _neighbourAlives = GetNumberOfNeighboursAlive(X,Y);
                if(GridArray[X,Y]==true && _neighbourAlives == 2 || _neighbourAlives == 3){
                    //if my tile is alive && with 2 our 3 neighbours i stay alive
                    _msg = "StayAlive";
                    return true;
                }
                else if(GridArray[X,Y]==false && _neighbourAlives == 3){
                    //if im dead and i have exactly 3 neighbour i came back to live by reproduction
                    _msg = "Reproduction";
                    return true;
                }
                //if not i die by Overpopulation, Underpopulation or Stay Dead
                _msg = "Stay Dead or Overpopulation";
                return false;
            }

        //public for debug reason
        public bool IsCellAliveNextState(int X, int Y){
            int _neighbourAlives = GetNumberOfNeighboursAlive(X,Y);
            if(GridArray[X,Y]==true && _neighbourAlives == 2 || _neighbourAlives == 3){
                //if my tile is alive && with 2 our 3 neighbours i stay alive
                return true;
            }
            else if(GridArray[X,Y]==false && _neighbourAlives == 3){
                //if im dead and i have exactly 3 neighbour i came back to live by reproduction
                return true;
            }
            //if not i die by Overpopulation, Underpopulation or Stay Dead
            return false;
        }
    
        private int GetNumberOfNeighboursAlive(int X, int Y){
            int _count = 0;
            /*
            is the index tested is on one of the 4 border ?
            if the index tested is on the Left/Right Borders i test if it's on of the 4 corners
            if not i test if the index is Purely on the Top/Bottom Borders(excluding the corners)
            if not i'll look out for all 8 surrounding Tiles
    
            rmd: Border as 5 Neighbours, Corner as 3 Neighbours, Standards as 8 Neighbours.
            */
            if(X==0){
                //i can always test X+1 on perpendicular axis
                if(GridArray[X+1,Y]) _count++;
    
                    if(Y==0){
                        //Bottom Left Corner
                        if(GridArray[X,Y+1]) _count++;
                        if(GridArray[X+1,Y+1]) _count++;
                    }
                    else if(Y==Height-1){
                        //Top Left Corner
                        if(GridArray[X,Y-1]) _count++;
                        if(GridArray[X+1,Y-1]) _count++;
                    }
                    else{
                        //Left Border Additional tests
                        if(GridArray[X,Y+1]) _count++;
                        if(GridArray[X,Y-1]) _count++;
                        if(GridArray[X+1,Y-1]) _count++;
                        if(GridArray[X+1,Y+1]) _count++;
                    }
                }
            else if(X==Width-1){
                //i can always test X-1 on perpendicular axis
                if(GridArray[X-1,Y]) _count++;
                    
                if(Y==0){
                    //Bottom Right Corner
                    if(GridArray[X,Y+1]) _count++;
                    if(GridArray[X-1,Y+1]) _count++;
                }
                else if(Y==Height-1){
                    //Top Right Corner
                    if(GridArray[X,Y-1]) _count++;
                    if(GridArray[X-1,Y-1]) _count++;
                }
                else{
                    //Right Border additional tests
                    if(GridArray[X,Y+1]) _count++;
                    if(GridArray[X,Y-1]) _count++;
                    if(GridArray[X-1,Y-1]) _count++;
                    if(GridArray[X-1,Y+1]) _count++;
                }
            }
            else if(Y==0 && X!=0 && X!=Width-1){
                //if im purely on the Bottom Border
                if(GridArray[X+1,Y]) _count++;
                if(GridArray[X-1,Y]) _count++;
                if(GridArray[X,Y+1]) _count++;
                if(GridArray[X-1,Y+1]) _count++;
                if(GridArray[X+1,Y+1]) _count++;
    
            }
            else if(Y==Height-1 && X!=0 && X!=Width-1){
                //if im purely on the Top Border
                    if(GridArray[X+1,Y]) _count++;
                    if(GridArray[X-1,Y]) _count++;
                    if(GridArray[X,Y-1]) _count++;
                    if(GridArray[X-1,Y-1]) _count++;
                    if(GridArray[X+1,Y-1]) _count++;
            }
            else{
                //All tiles int the middle
                //Test all 8 adjacent tiles and increment cound if alive
                if(GridArray[X+1,Y]) _count++;
                if(GridArray[X,Y+1]) _count++;
                if(GridArray[X-1,Y]) _count++;
                if(GridArray[X,Y-1]) _count++;
                if(GridArray[X-1,Y-1]) _count++;
                if(GridArray[X-1,Y+1]) _count++;
                if(GridArray[X+1,Y-1]) _count++;
                if(GridArray[X+1,Y+1]) _count++;
            }
            //Debug.Log("X: " + X + " / Y: " + Y + "  -> " + _count);
            return _count;
        }

        //public for debbug reasons
        public int GetNumberOfNeighboursAlive(Vector3 _worldPosition){
            int _x, _y;
            GetXY(_worldPosition, out _x, out _y);
            return GetNumberOfNeighboursAlive(_x, _y);
        }
    
        
    #endregion

    #region Rendering Functions
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
                GameObject _line = GameObject.Instantiate(Resources.Load("_line_") as GameObject, _start, Quaternion.identity, Parent.GetChild(0));
                _line.name = _name;
                LineRenderer _lr = _line.GetComponent<LineRenderer>();
                _lr.SetPosition(0, _start);
                _lr.SetPosition(1, _end);
        }

        private void DrawBorders(){
            GameObject _borders = GameObject.Instantiate(Resources.Load("_borders_") as GameObject, Parent.GetChild(0));
            LineRenderer _lr = _borders.GetComponent<LineRenderer>();
            _lr.SetPosition(0, GetWorldPosition(0,0));
            _lr.SetPosition(1, GetWorldPosition(0,Width));
            _lr.SetPosition(2, GetWorldPosition(Height,Width));
            _lr.SetPosition(3, GetWorldPosition(Height,0));
        }
    
        private void DrawAliveCell(int x, int y){
            GameObject _cell = GameObject.Instantiate(Resources.Load("_aliveCell_") as GameObject, Parent.GetChild(1));
            _cell.transform.position = GetWorldPosition(x, y) + new Vector3(UniformCellSize/2, UniformCellSize/2, 0f);
            _cell.transform.localScale = new Vector3(UniformCellSize, UniformCellSize, UniformCellSize);
            Cells[x,y] = _cell;
        }

        private void DrawAllAliveCells(){
            for (int x=0; x < GridArray.GetLength(0); x++){
                for (int y=0; y < GridArray.GetLength(1); y++){
                    if(GridArray[x,y]){
                        DrawAliveCell(x,y);
                    }
                }
            }
        }

        private void FlushGridRendering(){
            for (int x = 0; x < GridArray.GetLength(0); x++){
                for (int y = 0; y < GridArray.GetLength(1); y++){
                    //GridArray[x,y] = false;
                    GameObject.Destroy(Cells[x,y]);
                }
            }
        }
    #endregion

    #region private Get/Set
        private Vector3 GetWorldPosition(int x, int y){
            return new Vector3(x,y) * UniformCellSize + GetGridPositionAnchoredAtCenter();
        }

        public void GetXY(Vector3 _worldPosition, out int x, out int y){
            x = Mathf.FloorToInt( (_worldPosition - GetGridPositionAnchoredAtCenter()).x / UniformCellSize);
            y = Mathf.FloorToInt( (_worldPosition - GetGridPositionAnchoredAtCenter()).y / UniformCellSize);
        }

        private Vector3 GetGridPositionAnchoredAtCenter(){
            Vector3 _v = new Vector3();
            _v.x = Width * UniformCellSize / 2;
            _v.y = Height * UniformCellSize / 2;
            _v.z = 0f;
            return GridOrigine - _v;
        }
    #endregion

    #region Deprecated
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
    #endregion
}
