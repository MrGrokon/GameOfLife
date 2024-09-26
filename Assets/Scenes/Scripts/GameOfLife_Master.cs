using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife_Master : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CustomGrid _grid = new CustomGrid(10, 10, .25f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
