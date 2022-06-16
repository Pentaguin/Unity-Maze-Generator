using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MazeGenerator : MonoBehaviour
{   
    [Header("Maze Size")]
    public int rows; 
    public int columns;

    [Header("Instantiated Prefabs")]
    public GameObject wall;
    public GameObject floor;
    public GameObject flag;

    [Header("UI")]
    public UserInterface ui;

    private MazeCell[,] grid;
    private int currentRow = 0;
    private int currentColumn = 0;
    private string[] direction = { "Up", "Right", "Down", "Left" };
    private bool allCellsVisited = false;
    
    
    void Start(){
        GenerateMaze();
    }
  
    public void GenerateMaze(){ // destroy all the children of this object and generate a maze
        foreach(Transform transform in transform){ 
            Destroy(transform.gameObject);
        }
        // reset the values to standard
        currentRow = 0;
        currentColumn = 0;
        allCellsVisited = false;

        ChangeCameraPosition(); 
        CreateGrid();
        Algorithm();
    }

    void CreateGrid(){ // Create the grid of the maze
        float size = wall.transform.localScale.x; // get the size of the wall
        grid = new MazeCell[rows,columns];

        // loop through the rows and columns
        for (int i = 0; i < rows; i++){
            for(int j = 0; j < columns; j++){

                // create the floors and the walls
                GameObject floorObject = Instantiate(floor, new Vector3(j * size, 0, -i * size), Quaternion.identity); 
                floorObject.name = "Floor " + i + " " + j; 
                GameObject upWall = Instantiate(wall, new Vector3(j * size, 1.75f, -i * size + 1.25f), Quaternion.identity); // no rotation
                upWall.name = "UpWall " + i + " " + j; 
                GameObject downWall = Instantiate(wall, new Vector3(j * size, 1.75f, -i * size - 1.25f), Quaternion.identity); 
                downWall.name = "DownWall " + i + " " + j; 
                GameObject leftWall = Instantiate(wall, new Vector3(j * size -1.25f, 1.75f, -i * size), Quaternion.Euler(0,90,0)); //rotate the wall
                leftWall.name = "LeftWall " + i + " " + j; 
                GameObject rightWall = Instantiate(wall, new Vector3(j * size + 1.25f, 1.75f, -i * size), Quaternion.Euler(0,90,0)); 
                rightWall.name = "RightWall " + i + " " + j; 

                // access the MazeCell class and set the variables values 
                grid[i,j] = new MazeCell();
                grid[i,j].downWall = downWall;
                grid[i,j].leftWall = leftWall;
                grid[i,j].upWall = upWall;
                grid[i,j].rightWall = rightWall;

                //put all the instantiated objects in the parent
                floorObject.transform.parent = transform;
                upWall.transform.parent = transform;
                downWall.transform.parent = transform;
                leftWall.transform.parent = transform;
                rightWall.transform.parent = transform;

                if(i == rows - 1 && j == columns - 1){ // at the end of the maze instantiate a flag
                    GameObject flagObject = Instantiate(flag, new Vector3(j * size , 1 , i * -size), Quaternion.identity);
                    flagObject.transform.parent = transform;                
                }
            }
        }
    }
    
    void Algorithm(){ // use the Hunt and Kill algorithm to randomly walk a path and destroy the walls
        grid[currentRow,currentColumn].visited = true; // the start position is visited;

        while(!allCellsVisited){ // all cells are still not visited
            Walk();
            Hunt();
        }
    }
    
    void Walk(){ // randomly walks in a direction and destroy that wall
        while(AreThereUnvisitedCells()){ // there are still unvisited cells

            switch (direction.RandomItem()) {  //get a random direction from the array
              
                case "Up": // check up
                    if( isCellUnvisited(currentRow - 1, currentColumn )){ // if the cell above the current position is unvisited
                        if(grid[currentRow,currentColumn].upWall){
                            Destroy(grid[currentRow,currentColumn].upWall); // destroy the upwall
                        }
                        currentRow--;
                        grid[currentRow,currentColumn].visited = true; // go up

                        if(grid[currentRow,currentColumn].downWall){
                            Destroy(grid[currentRow,currentColumn].downWall); // destroy the downwall
                        }
                    }
                    break; 
  
                case "Right": //check right
                    if( isCellUnvisited(currentRow , currentColumn + 1) ){
                        if(grid[currentRow,currentColumn].rightWall){
                            Destroy(grid[currentRow,currentColumn].rightWall);
                        }

                        currentColumn++;
                        grid[currentRow,currentColumn].visited = true;

                        if(grid[currentRow,currentColumn].leftWall){
                            Destroy(grid[currentRow,currentColumn].leftWall);
                        }
                    }
                    break; 

                case "Down": // check down
                    if( isCellUnvisited(currentRow + 1, currentColumn )){

                        if(grid[currentRow,currentColumn].downWall){
                            Destroy(grid[currentRow,currentColumn].downWall);
                        }

                        currentRow++;
                        grid[currentRow,currentColumn].visited = true;

                        if(grid[currentRow,currentColumn].upWall){
                            Destroy(grid[currentRow,currentColumn].upWall);
                        }
                    }
                    break; 

                case "Left": 
                    if( isCellUnvisited(currentRow , currentColumn - 1 ) ){

                        if(grid[currentRow,currentColumn].leftWall){
                            Destroy(grid[currentRow,currentColumn].leftWall);
                        }

                        currentColumn--;
                        grid[currentRow,currentColumn].visited = true;

                        if(grid[currentRow,currentColumn].rightWall){
                            Destroy(grid[currentRow,currentColumn].rightWall);
                        }
                    }
                    break; 
            }
        }
    }

    void Hunt(){ // find an unvisited cell 
        allCellsVisited = true;
        //scan the whole grid
        for (int i = 0; i < rows; i++){
            for(int j = 0; j < columns; j++){
                if(!grid[i,j].visited && AreThereVisitedNeighbours(i,j)){ // the current position is not visited and the neighbours cells are all visited
                    allCellsVisited = false;
                    currentRow = i;
                    currentColumn = j;
                    grid[currentRow,currentColumn].visited = true;
                    DestroyAdjacentWall();
                    return;
                }
            }
        }
    }

    bool AreThereUnvisitedCells(){ // check if there are unvisited cells
        //check up
        if(isCellUnvisited(currentRow - 1,currentColumn )){
            return true;
        }
        //check down
        if(isCellUnvisited(currentRow + 1,currentColumn )){
            return true;
        }
        //check left
        if(isCellUnvisited(currentRow ,currentColumn + 1 )){
            return true;
        }
        //check right
        if(isCellUnvisited(currentRow ,currentColumn - 1)){
            return true;
        }
        return false;
    }

    bool isCellUnvisited(int row, int column){ // check if the cell is unvisited and in the boundary
        if(row >= 0 && row< rows && column >= 0 && column < columns && !grid[row,column].visited ){ // cell is unvisited
            return true;
        }
        else{ //cell is  visited
            return false;
        }
    }

    void DestroyAdjacentWall(){ // check to see if there are any walls that need to be destroyed
        bool destroyedWall = false;

        while(!destroyedWall){ // wall is not destroyed

            switch (direction.RandomItem()) {  //get a random direction from the array

                case "Up": 
                    if(currentRow > 0 && grid[currentRow - 1, currentColumn].visited){ 
                        if(grid[currentRow - 1, currentColumn].downWall){ // check if the downwall exist
                            Destroy(grid[currentRow - 1, currentColumn].downWall); //destroy downwall
                        }

                        if(grid[currentRow,currentColumn].upWall){ // check if the upwall exist
                            Destroy(grid[currentRow,currentColumn].upWall); //destroy upwall
                        }
                        destroyedWall = true;
                    }
                    break;

                case "Down":
                    if(currentRow < rows - 1 && grid[currentRow + 1, currentColumn].visited){
                        if(grid[currentRow + 1, currentColumn].upWall){
                            Destroy(grid[currentRow + 1, currentColumn].upWall);
                        }
                        if(grid[currentRow , currentColumn].downWall){
                            Destroy(grid[currentRow, currentColumn].downWall);
                        }
                        destroyedWall = true;
                    }
                    break;

                case "Left":
                    if(currentColumn > 0 && grid[currentRow, currentColumn - 1].visited){
                        if(grid[currentRow , currentColumn - 1].rightWall){
                            Destroy(grid[currentRow , currentColumn - 1].rightWall);
                        }
                        if(grid[currentRow , currentColumn].leftWall){
                            Destroy(grid[currentRow, currentColumn].leftWall);
                        }
                        destroyedWall = true;
                    }
                    break;

                case "Right":
                    if(currentColumn < columns - 1 && grid[currentRow, currentColumn + 1].visited){
                        if(grid[currentRow , currentColumn + 1].leftWall){
                            Destroy(grid[currentRow , currentColumn + 1].leftWall);
                        }
                        if(grid[currentRow , currentColumn].rightWall){
                            Destroy(grid[currentRow, currentColumn].rightWall);
                        }
                        destroyedWall = true;
                    }
                    break;
            }
        }
    }
    bool AreThereVisitedNeighbours(int row, int column){ // check if there are visited neighbours cells 
        
        //up has a visited cell
        if(row > 0 && grid[row - 1, column].visited){
            return true;
        }
        //down has a visited cell
        if(row < rows && grid[row + 1, column].visited){
            return true;
        }
        //left has a visited cell
        if(column > 0 && grid[row, column - 1].visited){
            return true;
        }
        //right has a visited cell
        if(column < columns && grid[row, column + 1].visited){
            return true;
        }

        return false;
    }
 
    public void ChangeCameraPosition(){ //change the camera position which is dependend on the amount of rows and columns.
        float whitespace = 5f;
        float size = wall.transform.localScale.x; 
        Vector3 camPos = Camera.main.transform.position;
        camPos.x = Mathf.Round(columns / 2) * size;
        camPos.y = (Mathf.Max(rows,columns) * size) + whitespace;
        camPos.z = -Mathf.Round(rows / 2) * size;
        Camera.main.transform.position = camPos;   
    }
}



public static class ArrayExtensions{
 
    // This is an extension method. RandomItem() will now exist on all arrays.
    public static T RandomItem<T>(this T[] array){
        return array[Random.Range(0, array.Length)];
    }
}
