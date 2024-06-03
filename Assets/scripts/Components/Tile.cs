using System;
using UnityEngine;

[Serializable]
public class Tile:MonoBehaviour
{
    public Vector2Int Coord => _coords;
    [SerializeField] private Vector2Int _coords; 
    public int ID => _id;
    [SerializeField] private int _id;
    public void Consturct(Vector2Int coords)
    {
        _coords = coords;
    }
}