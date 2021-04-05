using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game : MonoBehaviour
{

    public GameBoard gameBoard;
    
    
    private GameDataScriptableObject _gameData;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _gameData = Resources.Load<GameDataScriptableObject>("GameData");
       
        
        if (_gameData == null)
        {
            Debug.LogError("Can't load Game Data");
            return;
        }

        if (gameBoard == null)
        {
            Debug.LogError("Please set the gameBoard variable first");
            return;
        }
        
        gameBoard.InitializeBoard(_gameData);
        SetupCamera();
    }
    
    private void SetupCamera()
    {
        var bounds = _gameData.boardSize;
        var camPos =(bounds.size - Vector3.one)/2.0f;
        camPos.z = -10f;

        Camera.main.transform.position = camPos;
    }
}
