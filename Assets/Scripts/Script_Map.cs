using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Following https://www.youtube.com/watch?v=qNZ-0-7WuS8
public class Script_Map : MonoBehaviour
{
    public bool isDebug = false;
    private Dictionary<int, GameObject> tileSet;
    
    Dictionary<int, GameObject> tile_groups;

    public GameObject prefab_water;
    public GameObject prefab_grass;
    public GameObject prefab_berries;
    
    // public GameObject prefab_tree;
    // public GameObject prefab_sand;
    // public GameObject prefab_rock;
    // public GameObject prefab_snow;
    // public GameObject prefab_ice;
    // public GameObject prefab_dirt;
    
    public GameObject prefab_predator;
    public GameObject prefab_prey;
    public int map_width = 16;
    public int map_height = 9;

    public float magnification = 7.0f;
    public int max_actors; 
    public int num_berries;

    int x_offset = -5; //decrease this to shift the map left and increase this to shift the map right
    int y_offset = 0; //decrease this to shift the map down and increase this to shift the map up
    public int starting_predators;
    public int starting_prey;

    List<List<int>> noise_grid = new List<List<int>>();
    public List<List<Tile>> tile_grid = new List<List<Tile>>();

    List<GameObject> actors = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        CreateTileSet();
        CreateTileGroups();//organize the heirarchy
        GenerateMap();
        for(int i = 0; i < starting_predators; i++){
            SpawnActors(prefab_predator);
        }
        for(int i = 0; i < starting_prey; i++){
            SpawnActors(prefab_prey);
        }


        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(map_width / 2, map_height / 2, camera.transform.position.z);
        
    }

    void Update(){
        print("Actor count: " + actors.Count);
    }

    void CreateTileSet(){
        tileSet = new Dictionary<int, GameObject>();

        tileSet.Add(constants.grass_id, prefab_grass);
        tileSet.Add(constants.water_id, prefab_water); 
    }

    void CreateTileGroups(){
        tile_groups = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, GameObject> prefab_pair in tileSet){
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform; //this gameobject's transform, set this group as a child
            tile_group.transform.localPosition = Vector3.zero;
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }

    public void GenerateMap(){

        for(int x = 0; x < map_width; x++){
            
            List<int> noise_column = new List<int>();
            List<Tile> tile_column = new List<Tile>();

            for(int y = 0; y < map_height; y++){

                int tile_id = getIdUsingPerlin(x, y);
                noise_column.Add(tile_id);
                tile_column.Add(CreateTile(tile_id, x, y));
            
            }

            noise_grid.Add(noise_column);
            tile_grid.Add(tile_column);
        }

        for(int x = 0; x < num_berries; x++){
            generateBerry();
        }
    }

    public void generateBerry(){
        Tile tile = getRandomSpawnableTile(); 

        GameObject berry = Instantiate(prefab_berries, new Vector3(tile.x, tile.y, 0), Quaternion.identity);
        tile_grid[tile.x][tile.y].Add(berry);
    } 

    int getIdUsingPerlin(int x, int y){
        float raw_perlin = Mathf.PerlinNoise((x - x_offset) / magnification, (y - y_offset) / magnification);
        float clamp_perlin = Mathf.Clamp(raw_perlin, 0.0f, 1.0f);

        float scale_perlin = clamp_perlin * tileSet.Count;
        if(scale_perlin >= tileSet.Count){
            scale_perlin = tileSet.Count - 1;
        }

        return Mathf.FloorToInt(scale_perlin); // round down to get integer
    }

    Tile CreateTile(int tile_id, int x, int y){
        Tile tile = new Tile(tile_id, x, y);
        GameObject tile_game_object = Instantiate(tileSet[tile_id], new Vector3(x, y, 0), Quaternion.identity);

        tile_game_object.name = tileSet[tile_id].name;
        tile_game_object.transform.parent = tile_groups[tile_id].transform;

        tile.game_object = tile_game_object;
        return tile;
    }

    void SpawnActors(GameObject prefab){
        Tile tile = getRandomSpawnableTile();
        GameObject actor = Instantiate(prefab, new Vector3(tile.x, tile.y, 0), Quaternion.identity);
        actors.Add(actor);
    }

    public void handleProcreation(GameObject actor){

        if(actors.Count >= max_actors){
            return;
        }
        
        GameObject new_actor = null;

        if(actor.name.Contains("prefab_predator")){
            new_actor = Instantiate(prefab_predator, new Vector3(actor.transform.position.x, actor.transform.position.y, 0), Quaternion.identity);
        } else if(actor.name.Contains("prefab_prey")){
            new_actor = Instantiate(prefab_prey, new Vector3(actor.transform.position.x, actor.transform.position.y, 0), Quaternion.identity);
        }

        actors.Add(new_actor);
    }

    public Tile getRandomSpawnableTile(){
        int x = Random.Range(0, map_width);
        int y = Random.Range(0, map_height);

        while(tile_grid[x][y].tile_id == constants.water_id){
            x = Random.Range(0, map_width);
            y = Random.Range(0, map_height);
        } 

        return tile_grid[x][y];
    }

    public void handleEatingBerry(GameObject actor, GameObject berry){
        Destroy(berry);
    }
}
