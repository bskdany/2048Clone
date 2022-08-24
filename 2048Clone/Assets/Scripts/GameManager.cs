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
    private BoardManager boardManager;
    private Player currentTile;

    List<GameObject> tileList = new List<GameObject>();
    List<GameObject> tileListCopy = new List<GameObject>();
    List<GameObject> tilesToRemove = new List<GameObject>();

    int[] tileValueScale = new int[] {2,4,8,16,32,64,128,256,512,1024,2048};
    GameObject[] tileObjectScale;

    bool canTakeInput = true;
    bool isMoving = false;
    bool hasAnyMovementHappened;
    string directionOfMovement = "";

    void Start()
    {
        tileObjectScale = new GameObject[] { tile2, tile4, tile8, tile16, tile32, tile64, tile128, tile256, tile512, tile1024, tile2048 };
        int randomNumber = Random.Range(0, 1);
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
        if (canTakeInput)
        {
            takeInput();
        }
        if (!canTakeInput && !isMoving)
        {
            moveTiles();
        }
        if(!canTakeInput && isMoving)
        {
            if (checkIfMovementFinished())
            {
                boardManager = boardManagerObject.GetComponent<BoardManager>();
                

                if (boardManager.GetComponent<BoardManager>().isGameOver())
                {
                    gameOverScreen.GetComponent<GameOverScreen>().Setup();
                }
                
                canTakeInput = true;
                isMoving = false;
                if (hasAnyMovementHappened)
                {
                    mergeTiles();

                    foreach (GameObject tile in tilesToRemove)
                    {
                        tileList.Remove(tile);
                        Destroy(tile);
                    }

                    

                    int randomNumber = Random.Range(0, 1);
                    if (randomNumber == 0)
                    {
                        spawnTile(tile2);
                    }
                    else
                    {
                        spawnTile(tile4);
                    }
                    //boardManager.printBoardArray();
                }
                
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
    }
   
    void spawnTile(GameObject tileType)
    {   
        //tile in spawned
        GameObject tile = Instantiate(tileType);
        Player tileScript = tile.GetComponent<Player>();
        int tileValue = tileScript.tileValue;
        //new position is set
        boardManager = boardManagerObject.GetComponent<BoardManager>();
        tile.transform.position = boardManager.getSpawnablePosition();

        boardManager.occupyPosition(tile.transform.position, tileValue);
        
        currentTile = tile.GetComponent<Player>();
        currentTile.tileValue = tileValue;
        //added to the list of tiles
        tileList.Add(tile);
    }

    void moveTiles()
    {
        isMoving = true;
        hasAnyMovementHappened = false;
        boardManager = boardManagerObject.GetComponent<BoardManager>();

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
            Player tileScript = tileToUse.GetComponent<Player>();
            int tileValue = tileScript.tileValue;

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

            boardManager.freePosition(tileToUse.transform.position);

            int xPos = 0;
            int yPos = 0;
            int[] convertedDataArray = directionNameToXY(directionOfMovement);
            xPos = convertedDataArray[0];
            yPos = convertedDataArray[1];
            if (!mergeTiles)
            {
                boardManager.occupyPosition(new Vector3(tileToUse.transform.position.x + xPos * howManyBlocksToMove, tileToUse.transform.position.y + yPos * howManyBlocksToMove, 0), tileValue);
            }
            else
            {
                boardManager.occupyPosition(new Vector3(tileToUse.transform.position.x + xPos * (howManyBlocksToMove+1), tileToUse.transform.position.y + yPos * (howManyBlocksToMove+1), 0), tileValue+1);
                tilesToRemove.Add(tileToUse);
            }
            currentTile = tileToUse.GetComponent<Player>();
            currentTile.activateTileMovement(directionOfMovement, howManyBlocksToMove);

            tileListCopy.Remove(tileToUse);

            tileCounter++;
        }
    }
    
    int[] calculateTileMovement(GameObject tile, int tileValue)
    {
        List<int> blockValuesInDirection = new List<int>();
        blockValuesInDirection = boardManager.getTilesInDirection(tile.transform.position, directionOfMovement);

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
                if (mergeTile == 1)
                {
                    mergeTile = 0;
                }
                else
                {
                    mergeTile = 1;
                }
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
        int xPos;
        int yPos;
        int[] convertedDirection;
        convertedDirection = directionNameToXY(directionOfMovement);
        xPos = convertedDirection[0];
        yPos = convertedDirection[1];

        List<Vector3> postionsToDestroy = new List<Vector3>();

        foreach (GameObject tile in tilesToRemove)
        {
            postionsToDestroy.Add(new Vector3(tile.transform.position.x+xPos, tile.transform.position.y + yPos,0));
        }

        List<GameObject> mergedTiles = new List<GameObject>();
        foreach (GameObject tile in tileList)
        {
            Vector3 sobstitutePosition = tile.transform.position;
            if (postionsToDestroy.Contains(sobstitutePosition))
            {
                Player tileScript = tile.GetComponent<Player>();
                int tileValue = tileScript.tileValue;
                int nextIndex = System.Array.IndexOf(tileValueScale, tileValue)+1;

                Destroy(tile);

                GameObject tile1 = Instantiate(tileObjectScale[nextIndex]);
                tile1.transform.position = sobstitutePosition;

                Player newTile = tile1.GetComponent<Player>();
                newTile.tileValue = tileValueScale[nextIndex];
                boardManager.occupyPosition(sobstitutePosition, tileValueScale[nextIndex]);

                tilesToRemove.Add(tile);
                mergedTiles.Add(tile1);

                if(nextIndex == 10)
                {
                    ggScript.Setup();
                }
            }
        }
        foreach(GameObject tile in mergedTiles)
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
