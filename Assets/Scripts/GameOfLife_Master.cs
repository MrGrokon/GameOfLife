using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife_Master : MonoBehaviour
{
    private CustomGrid GOL_Grid;

    void Awake()
    {
        GOL_Grid = new CustomGrid(this.transform, 25, 25, .25f);
        //GOL_Grid.SetValue(5,5, 100);
    }

    void Update()
    {
        if (GOL_Grid != null)
        {
            if(Input.GetMouseButtonDown(0)){
                Debug.Log(GOL_Grid.GetValue(GetMousePosition()));
            }
    
            if(Input.GetMouseButtonDown(1)){
                GOL_Grid.SetValue(GetMousePosition(), -100);
            }

            if(Input.GetKeyDown(KeyCode.Space)){
                GOL_Grid.Init();
            }
        }
        else
            Debug.Log("ERROR: Grid Undefined");
    }

    #region Misc
        private Vector3 GetMousePosition(){
            Vector3 _v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _v.z = 0f;
            return _v;
        }
    #endregion
}
