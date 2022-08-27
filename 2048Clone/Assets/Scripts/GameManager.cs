using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject tile2;
    public GameObject tile4;
    public GameObject tile8;
    public GameObject tile16;
    public GameObject tile32;
    public GameObject tile64;
    public GameObject tile128;
    public GameObject tile256;
    public GameObject tile512;
    public GameObject tile1024;
    public GameObject tile2048;

    public GameOverScreen gameOverScreen;
    public GG ggScript;

    public GameObject boardManagerObject;

    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    List<GameObject> tileList = new List<GameObject>();
    List<GameObject> tileListCopy = new List<GameObject>();
    List<GameObject> tilesToRemove = new List<GameObject>();
    List<Vector3> postionsToDestroy = new List<Vector3>();

    int[] tileValueScale = new int[] {2,4,8,16,32,64,128,256,512,1024,2048};
    GameObject[] tileObjectScale;

    bool canTakeInput = true;
    bool isMoving = false;
    bool hasAnyMovementHappened;
    string directionOfMovement = "";

    void Start()
    {
        tileObjectScale = new GameObject[] { tile2, tile4, tile8, tile16, tile32, tile64, tile128, tile256, tile512, tile1024, tile2048 };
        int randomNumber = Random.Range(0, 2);
        if(randomNumber == 0)
        {
            spawnTile(tile2);
        }
        else
        {
            spawnTile(tile4);
        }
        randomNumber = Random.Range(0, 1);
        if (randomNumber == 0)
        {
            spawnTile(tile2);
        }
        else
        {
            spawnTile(tile4);
        }
    }

    void Update()
    {   
        //input can be taken only one time
        if (canTakeInput)
        {
            takeInput();
        }
        //if input is taken and the tiles arent moving
        if (!canTakeInput && !isMoving)
        {
            moveTiles();
        }

        //if input is taken and tiles are moving
        if(!canTakeInput && isMoving)
        {   
            //checks if tile movement has finished
            if (checkIfMovementFinished())
            {
                //checks if game is over
                if (boardManagerObject.GetComponent<BoardManager>().isGameOver())
                {   
                    //if game is over the game over image is displayed
                    gameOverScreen.GetComponent<GameOverScreen>().Setup();
                }
                
                if (hasAnyMovementHappened)
                {
                    mergeTiles();

                    //new tile is spawned
                    int randomNumber = Random.Range(0, 2);
                    if (randomNumber == 0)
                    {
                        spawnTile(tile2);
                    }
                    else
                    {
                        spawnTile(tile4);
                    }

                    boardManagerObject.GetComponent<BoardManager>().printBoardArray();
                }

                //bools are reinitialized
                canTakeInput = true;
                isMoving = false;
            }
        }
    }

    void takeInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            directionOfMovement = "Right";
            canTakeInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            directionOfMovement = "Left";
            canTakeInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            directionOfMovement = "Up";
            canTakeInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            directionOfMovement = "Down";
            canTakeInput = false;
        }

        //swipe detection code I stole
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lp.x > fp.x))  //If the movement was to the right)
                        {   //Right swipe
                            directionOfMovement = "Right";
                            canTakeInput = false;
                        }
                        else
                        {   //Left swipe
                            directionOfMovement = "Left";
                            canTakeInput = false;
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lp.y > fp.y)  //If the movement was up
                        {   //Up swipe
                            directionOfMovement = "Up";
                            canTakeInput = false;
                        }
                        else
                        {   //Down swipe
                            directionOfMovement = "Down";
                            canTakeInput = false;
                        }
                    }
                }
            }
        }
    }
   
    void spawnTile(GameObject tileType)
    {   
        //tile in spawned
        GameObject tile = Instantiate(tileType);
        int tileValue = tile.GetComponent<Player>().tileValue;
        
        //new position is set
        tile.transform.position = boardManagerObject.GetComponent<BoardManager>().getSpawnablePosition();

        //position is occupied in board array
        boardManagerObject.GetComponent<BoardManager>().occupyPosition(tile.transform.position, tileValue);
        
        //added to the list of tiles
        tileList.Add(tile);
    }

    void moveTiles()
    {
        isMoving = true;
        hasAnyMovementHappened = false;

        //objects lists are cleared
        tileListCopy.Clear();
        tilesToRemove.Clear();

        //tile list is copied
        foreach(GameObject tile in tileList)
        {
            tileListCopy.Add(tile);
        }

        int tileCounter = 0;
        while (tileCounter < tileList.Count)
        {   
            //gets the tile closest to the wall in the direction of movement
            GameObject tileToUse = tileListCopy[0];
            foreach (GameObject tile in tileListCopy)
            {
                switch (directionOfMovement)
                {
                    case "Right":
                        if (tile.transform.position.x > tileToUse.transform.position.x)
                        {
                            tileToUse = tile;
                        }
                        break;

                    case "Left":
                        if (tile.transform.position.x < tileToUse.transform.position.x)
                        {
                            tileToUse = tile;
                        }
                        break;

                    case "Up":
                        if (tile.transform.position.y > tileToUse.transform.position.y)
                        {
                            tileToUse = tile;
                        }
                        break;
                    case "Down":
                        if (tile.transform.position.y < tileToUse.transform.position.y)
                        {
                            tileToUse = tile;
                        }
                        break;
                }
            }
            
            //get the value of the tile to use
            int tileValue = tileToUse.GetComponent<Player>().tileValue;

            //tile movement is calculated
            int[] tileMovementData = calculateTileMovement(tileToUse, tileValue);
            int howManyBlocksToMove = tileMovementData[0];

            bool mergeTiles;
            if (tileMovementData[1] == 1)
            {
                mergeTiles = true;
            }
            else
            {
                mergeTiles = false;
            }

            if (howManyBlocksToMove > 0 || mergeTiles)
            {
                hasAnyMovementHappened = true;
            }

            //tile position is freed on the array
            boardManagerObject.GetComponent<BoardManager>().freePosition(tileToUse.transform.position);

            int xPos = 0;
            int yPos = 0;
            int[] convertedDataArray = directionNameToXY(directionOfMovement);
            xPos = convertedDataArray[0];
            yPos = convertedDataArray[1];
            
            if (!mergeTiles)
            {   
                //position where the tile will be moved is occupied
                boardManagerObject.GetComponent<BoardManager>().occupyPosition(new Vector3(tileToUse.transform.position.x + xPos * howManyBlocksToMove, tileToUse.transform.position.y + yPos * howManyBlocksToMove, 0), tileValue);
            }
            else
            {   
                //position where the tile will be moved + 1 is occupied with a non existing value
                boardManagerObject.GetComponent<BoardManager>().occupyPosition(new Vector3(tileToUse.transform.position.x + xPos * (howManyBlocksToMove), tileToUse.transform.position.y + yPos * (howManyBlocksToMove), 0), tileValue+1);
                //the current tile will be deleted
                tilesToRemove.Add(tileToUse);
            }

            //the movement is activated
            tileToUse.GetComponent<Player>().activateTileMovement(directionOfMovement, howManyBlocksToMove); ;

            tileListCopy.Remove(tileToUse);
            tileCounter++;
        }
    }
    
    int[] calculateTileMovement(GameObject tile, int tileValue)
    {
        List<int> blockValuesInDirection = new List<int>();
        blockValuesInDirection = boardManagerObject.GetComponent<BoardManager>().getTilesInDirection(tile.transform.position, directionOfMovement);

        //calculates how many boxes the tile has to move
        //calculates if the tile should merge or not
        int howManyBoxToMove = 0;
        int mergeTile = 0;
        bool checkForMerge = true;
        foreach(int blockValue in blockValuesInDirection)
        {
            if(blockValue == 0)
            {
                howManyBoxToMove++;
            }
            else if (blockValue != tileValue)
            {
                checkForMerge = false;
            } 
            else if(blockValue == tileValue && checkForMerge){
                mergeTile = 1;
                howManyBoxToMove++;
            }
        }
        int[] toReturn;
        toReturn = new int[] { howManyBoxToMove, mergeTile };
        return toReturn;
    }

    int[] directionNameToXY(string direction)
    {
        int xPos = 0;
        int yPos = 0;
        switch (direction)
        {
            case "Right":
                xPos = 1;
                break;
            case "Left":
                xPos = -1;
                break;
            case "Up":
                yPos = 1;
                break;
            case "Down":
                yPos = -1;
                break;
        }
        int[] toReturn;
        toReturn = new int[] {xPos, yPos};
        return toReturn;
    }
    
    void mergeTiles()
    {
        postionsToDestroy.Clear();

        //positions of tiles that moved and should merge are added to the list
        foreach (GameObject tile in tilesToRemove)
        {
            postionsToDestroy.Add(new Vector3(tile.transform.position.x, tile.transform.position.y,0));
            tileList.Remove(tile);
            Destroy(tile);
        }
  
        int a = 0;
        foreach (Vector3 b in postionsToDestroy)
        {
            a += 1;
        }
        Debug.Log(a.ToString());

        //list of new tiles that are created after merging
        List<GameObject> mergedTiles = new List<GameObject>();

        foreach (GameObject tile in tileList)
        {   
            Vector3 sobstitutePosition = tile.transform.position;
            
            //if the tile in tilelist has the same position of the tile that needs to be removed
            if (postionsToDestroy.Contains(sobstitutePosition))
            {
                int tileValue = tile.GetComponent<Player>().tileValue;
                int nextIndex = System.Array.IndexOf(tileValueScale, tileValue)+1;

                GameObject tile1 = Instantiate(tileObjectScale[nextIndex]);
                tile1.transform.position = sobstitutePosition;

                //position is occupied by the new tile with the next value
                boardManagerObject.GetComponent<BoardManager>().occupyPosition(sobstitutePosition, tileValueScale[nextIndex]);

                tilesToRemove.Add(tile);
                mergedTiles.Add(tile1);

                if(nextIndex == 10)
                {
                    ggScript.Setup();
                }
            }
        }

        //tiles to remove after merging are removed
        foreach (GameObject tile in tilesToRemove)
        {
            tileList.Remove(tile);
            Destroy(tile);
        }

        foreach (GameObject tile in mergedTiles)
        {
            tileList.Add(tile);
        }
    }

    bool checkIfMovementFinished()
    {
        bool movementFinished = true;
        foreach(GameObject tile in tileList)
        {
            Player tileObject = tile.GetComponent<Player>();
            if (tileObject.startMove)
            {
                movementFinished = false;
            }
        }
        return movementFinished;
    }

}
