using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;


public class BoardManager : MonoBehaviour
{   
    //why are 2d arrays declared like that? oh god my eyes
    int[,] boardArray = new int[,] 
    {
        {-1,-1,-1,-1,-1,-2},
        {-1,0,0,0,0,-1 },   
        {-1,0,0,0,0,-1 },   
        {-1,0,0,0,0,-1 },   
        {-1,0,0,0,0,-1 },   
        {-4,-1,-1,-1,-1,-3 }
    };

    List<int> blockValuesInDirection = new List<int>();

    public List<int> getTilesInDirection(Vector3 position, string where)
    {
        float y_index=0f;
        float x_index=0f;   
        switch (where)
        {   
            case "Right":
                x_index = 1f;
                break;
            case "Left":
                x_index = -1f;
                break;
            case "Up":
                y_index = 1f;
                break;
            case "Down":
                y_index = -1f;
                break;
        }

        blockValuesInDirection.Clear();

        float baseX = x_index;
        float baseY = y_index;
        while (boardArray[(int)Mathf.Round(-(position.y - 4) - y_index), (int)Mathf.Round(position.x + 1 + x_index)] != -1)
        {
            blockValuesInDirection.Add(boardArray[(int)Mathf.Round(-(position.y -4) - y_index), (int)Mathf.Round(position.x + 1 + x_index)]);
            x_index += baseX;
            y_index += baseY;
        }
        return blockValuesInDirection;
    }

    public Vector3 getSpawnablePosition()
    {
        List<Vector3> possibleSpawnPositions = new List<Vector3>();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (boardArray[i, j] == 0)
                {
                    possibleSpawnPositions.Add(new Vector3(j-1, 4-i, 0));
                }
            }
        }
        Vector3 spawnPosition = possibleSpawnPositions[Random.Range(0, possibleSpawnPositions.Count)];
        return spawnPosition;
    }

    public void freePosition(Vector3 position)
    {
        boardArray[(int)Mathf.Round(-(position.y - 4)), (int)Mathf.Round(position.x+1)] = 0;
    }

    public void occupyPosition(Vector3 position, int value)
    {
        boardArray[(int)Mathf.Round(-(position.y - 4)), (int)Mathf.Round(position.x+1)] = value;
    }

    public void printBoardArray()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < boardArray.GetLength(1); i++)
        {
            for (int j = 0; j < boardArray.GetLength(0); j++)
            {
                sb.Append(boardArray[i, j]);
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    public bool isGameOver()
    {   
        for (int yPos = 0; yPos < 5; yPos++)
        {
            for (int xPos = 0; xPos < 5; xPos++)
            {
                if (boardArray[yPos, xPos] != -1)
                {
                    int tileValue = boardArray[yPos, xPos];
                    if(tileValue== boardArray[yPos+1, xPos] || boardArray[yPos + 1, xPos] == 0)
                    {
                        return false;
                    }
                    if (tileValue == boardArray[yPos-1, xPos] || boardArray[yPos + 1, xPos] == 0)
                    {
                        return false;
                    }
                    if (tileValue == boardArray[yPos, xPos+1] || boardArray[yPos + 1, xPos] == 0)
                    {
                        return false;
                    }
                    if (tileValue == boardArray[yPos, xPos-1] || boardArray[yPos + 1, xPos] == 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
