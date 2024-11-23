using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile{
    public int tile_id;
    public int x;
    public int y;

    public GameObject game_object;

    public List<GameObject> objects_on_tile = new List<GameObject>();

    public Tile(int tile_id, int x, int y){
        this.tile_id = tile_id;
        this.x = x;
        this.y = y;
    }

    public void removeFromTile(GameObject gameObject){
        objects_on_tile.Remove(gameObject);
    }

    public void Add(GameObject gameObject){ 
        objects_on_tile.Add(gameObject);
    }
}