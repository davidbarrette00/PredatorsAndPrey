using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Script_Actor : MonoBehaviour
{
    Script_Map map;
    public float time_between_procreation = 0.5f;
    public float time_between_moves = 1.0f;
    
    float time_since_last_move = 0.0f;
    float time_since_last_procreation = 0.0f;

    LinkedList<Vector3> path = new LinkedList<Vector3>();
    // Vector3 desired_position = Vector3.zero;

    public int health = 5;
    public int sight_radius = 5;

    String food;

    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.Find("Map").GetComponent<Script_Map>();

        if(gameObject.name.Contains(constants.prefab_coyote)){
            food = constants.prefab_rabbit;
        } else if(gameObject.name.Contains(constants.prefab_rabbit)){
            food = constants.prefab_berries;
        }

        time_between_moves += UnityEngine.Random.Range(-0.1f, 0.1f);
        time_between_procreation += UnityEngine.Random.Range(-0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        // if(health <= 0){
        //     Destroy(gameObject);
        // }

        
        if(time_since_last_move > time_between_moves){

            if(path == null || path.Count == 0){
                foreach(GameObject obj in map.tile_grid[(int) transform.position.x][(int) transform.position.y].objects_on_tile){
                    if(obj.name.Contains(food)){ //on a tile with actor's food
                        map.tile_grid[(int) transform.position.x][(int) transform.position.y].objects_on_tile.Remove(obj);
                        Destroy(obj);

                        if(food.Equals(constants.prefab_berries)){
                            map.generateNewGroupOfBerries();
                        }

                        break;
                    }
                }
                search();
            }
            
            Tile old_tile = map.tile_grid[(int) transform.position.x][(int) transform.position.y];
            transform.position += path.First.Value;
            Tile new_tile = map.tile_grid[(int) transform.position.x][(int) transform.position.y];
            path.RemoveFirst();




            old_tile.objects_on_tile.Remove(gameObject);
            new_tile.objects_on_tile.Add(gameObject);
            time_since_last_move = 0.0f;
            // health -= 1;
        } else {
            //
            //TODO ANIMATE THE MOVE
        }

        if(time_since_last_procreation > time_between_procreation){
            map.handleProcreation(gameObject);

            // health -= 5;
            time_since_last_procreation = 0.0f;
        }

            
        time_since_last_move += Time.deltaTime;
        time_since_last_procreation += Time.deltaTime;
    }

    void search(){
        int x = (int) transform.position.x;
        int y = (int) transform.position.y;

        //search in a square around the actor for food
        for(int i = x - sight_radius; i < x + sight_radius; i++){
            for(int j = y - sight_radius; j < y + sight_radius; j++){
                if(i < 0 || j < 0 || i >= map.map_width || j >= map.map_height){ //out of bounds
                    continue;
                } else {
                    foreach(GameObject obj in map.tile_grid[i][j].objects_on_tile){
                        if(obj.name.Contains(food)){
                            if(gameObject.name.Contains(constants.prefab_rabbit)){
                                print($"food {food} {x} {y} {i} {j}");
                            }
                            FindPath(x, y, i, j);
                            return;
                        }
                    }
                }
            }
        }
        
        //if no food, just roam randomly
        List<Vector3> possible_roaming_moves = get_roaming_moves(x, y);

        int chosen_move_index = UnityEngine.Random.Range(0, possible_roaming_moves.Count);
        if(chosen_move_index == possible_roaming_moves.Count-1){
            path.AddLast(new Vector3(0, 0, 0));
        } else {
            path.AddLast(possible_roaming_moves[chosen_move_index]);
        }
        
    }

    private List<Vector3> get_roaming_moves(int x, int y){

        List<Vector3> possible_moves = new List<Vector3>();

        if (y < map.map_height - 1  && map.tile_grid[x][y+1].tile_id != constants.water_id) { //North
            possible_moves.Add(new Vector3(0, 1, 0.0f));
        } 
        if (y < map.map_height - 1 && x < map.map_width - 1  && map.tile_grid[x+1][y+1].tile_id != constants.water_id) { //North East
            possible_moves.Add(new Vector3(1, 1, 0.0f));
        } 
        if (x < map.map_width - 1  && map.tile_grid[x+1][y].tile_id != constants.water_id) { //East
            possible_moves.Add(new Vector3(1, 0, 0.0f));
        } 
        if (y > 0 && x < map.map_width - 1  && map.tile_grid[x+1][y-1].tile_id != constants.water_id) { //South East
            possible_moves.Add(new Vector3(1, -1, 0.0f));
        } 
        if (y > 0 && map.tile_grid[x][y-1].tile_id != constants.water_id) {//South
            possible_moves.Add(new Vector3(0, -1, 0.0f));
        } 
        if (y > 0 && x > 0 && map.tile_grid[x-1][y-1].tile_id != constants.water_id) { //South West
            possible_moves.Add(new Vector3(-1, -1, 0.0f));
        } 
        if (x > 0 && map.tile_grid[x-1][y].tile_id != constants.water_id) {//West
            possible_moves.Add(new Vector3(-1, 0, 0.0f));
        } 
        if (y < map.map_height - 1 && x > 0 && map.tile_grid[x-1][y+1].tile_id != constants.water_id) {//North West
            possible_moves.Add(new Vector3(-1, 1, 0.0f));
        } 

        return possible_moves;
    }

    // public void getPath(int x1, int y1, int x2, int y2){ //BFS
    //     Queue<Vector3> queue = new Queue<Vector3>();
    //     Vector3 start = new Vector3(x1, y1, 0.0f);
    //     Vector3 end = new Vector3(x2, y2, 0.0f);
    //     queue.Enqueue(start);

    //     while(queue.Count > 0){
    //         Vector3 current = queue.Dequeue();
    //         if(current == end){
    //             path.Add(current);
    //             return;
    //         } else {
    //             List<Vector3> possible_moves = get_possible_moves((int) current.x, (int) current.y);
    //             foreach(Vector3 move in possible_moves){
    //                 queue.Enqueue(move);
    //             }
    //         }
    //     }

    //     foreach(Vector3 move in path){
    //         print(move);
    //     }
    //         Thread.Sleep(100000000);
    //     path.Reverse();
    // }

    public void FindPath(int x_original, int y_original, int i, int j)
    {
        int x = x_original;
        int y = y_original;

        // Move diagonally or horizontally/vertically
        while (x != i || y != j)
        {
            // Check if moving diagonally
            if (x < i && y < j && map.tile_grid[x + 1][y + 1].tile_id == constants.grass_id)
            {
                x += 1;  // Move right
                y += 1;  // Move up
                path.AddLast(new Vector3(1, 1, 0.0f));
            }
            else if (x > i && y < j && map.tile_grid[x - 1][y + 1].tile_id == constants.grass_id)
            {
                x -= 1;  // Move left
                y += 1;  // Move up
                path.AddLast(new Vector3(-1, 1, 0.0f));
            }
            else if (x < i && y > j && map.tile_grid[x + 1][y - 1].tile_id == constants.grass_id)
            {
                x += 1;  // Move right
                y -= 1;  // Move down
                path.AddLast(new Vector3(1, -1, 0.0f));
            }
            else if (x > i && y > j && map.tile_grid[x - 1][y - 1].tile_id == constants.grass_id)
            {
                x -= 1;  // Move left
                y -= 1;  // Move down
                path.AddLast(new Vector3(-1, -1, 0.0f));
            }
            // Check if only horizontal movement is needed
            else if (x < i && map.tile_grid[x + 1][y].tile_id == constants.grass_id)
            {
                x += 1;  // Move right
                path.AddLast(new Vector3(1, 0, 0.0f));
            }
            else if (x > i && map.tile_grid[x - 1][y].tile_id == constants.grass_id)
            {
                x -= 1;  // Move left
                path.AddLast(new Vector3(-1, 0, 0.0f));
            }
            // Check if only vertical movement is needed
            else if (y < j && map.tile_grid[x][y + 1].tile_id == constants.grass_id)
            {
                y += 1;  // Move up
                path.AddLast(new Vector3(0, 1, 0.0f));
            }
            else if (y > j && map.tile_grid[x][y - 1].tile_id == constants.grass_id)
            {
                y -= 1;  // Move down
                path.AddLast(new Vector3(0, -1, 0.0f));
            } else {
                //idk
                path.AddLast(new Vector3(0, 0, 0.0f));
            }

            if(gameObject.name.Contains(constants.prefab_coyote)){
                //need to only add 1 because we need to search for where the rabbit moves next
                print("break coyote path");
                break;
            }
        }
    }
    
}
