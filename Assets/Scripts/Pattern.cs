using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "ScriptableObj")]
public class Pattern : ScriptableObject
{
    public string Name;
    public Vector2Int[] Cells;

    public Vector2Int GetOrigin(){
        Vector2Int  _max = GetMaxIndex();
        return new Vector2Int(Mathf.CeilToInt(_max.x/2), Mathf.CeilToInt(_max.y/2));
    }

    public Vector2Int GetExtent(){
        //return max dimension / 2 to create a security margin at grid borders
        Vector2Int _max = GetMaxIndex();
        _max.x++;
        _max.y++;
        return _max / 2;
    }

    public Vector2Int GetMaxIndex(){
        int maxX=0, maxY=0;
        for (int i = 0; i < Cells.Length; i++){
            //Get Futher X && Y from Index (0,0)
            if(Cells[i].x > maxX) maxX = Cells[i].x;
            if(Cells[i].y > maxY) maxY = Cells[i].y;
        }
        return new Vector2Int(maxX, maxY);
    }
}