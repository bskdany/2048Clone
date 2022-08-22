using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    float moveSpeed = 0.1f;
    public bool startMove = false;
    public int tileValue;
    float deltaMove = 0f;
    float vectorX;
    float vectorY; 
    float howManyBoxToMove = 0f;
    
    void Update()
    {
        handleMovement();
    }

    void handleMovement()
    {
        if (startMove)
        {   
            if (deltaMove < howManyBoxToMove - moveSpeed)
            {
                transform.position += new Vector3(vectorX * moveSpeed, vectorY * moveSpeed, 0);
                deltaMove += moveSpeed;
            }
            //corrects the last movement because float sucks
            else
            {
                transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0);
                deltaMove = 0f;
                startMove = false;            
            }
        }
    }

    public void activateTileMovement(string direction, int howManyBoxesToMove)
    {
        howManyBoxToMove = howManyBoxesToMove;
        vectorX = 0;
        vectorY = 0;
        switch (direction)
        {
            case "Right":
                vectorX = 1;
                break;
            case "Left":
                vectorX = -1;
                break;
            case "Up":
                vectorY = 1;
                break;
            case "Down":
                vectorY = -1;
                break;
        }
        startMove = true;
    }
}
