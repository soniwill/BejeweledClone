using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public enum SearchDir
{
   Positive,
   Negative,
}

public enum Axis
{
    Vertical,
    Horizontal
}

public class GameBoard : MonoBehaviour
{
    public bool runTest;
    public Vector2Int initialTestPos;

    private GameDataScriptableObject _gameData;
    private BoundsInt _bounds;
    private Gem [,] _gems;
    private List<Gem> _matchesCandidates;
    private bool _isDragging;
    private MouseInput _mouseInput;
    private Gem _selectedGem;
    private float _swapTime;
    private bool _initGemSelection;

    //-----------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _mouseInput = new MouseInput();
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void OnEnable()
    {
        _mouseInput.Enable();
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void OnDisable()
    {
        _mouseInput.Disable();
    }
    
    //-----------------------------------------------------------------------------------------------------------

    void Start()
    {
        _mouseInput.Mouse.MouseUp.performed += _ => MouseUp();
        _mouseInput.Mouse.MouseDown.performed += _ => MouseDown();
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void Update()
    {
        Drag();
    }
    
    //-----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Converts a cell index of a tilemap one of two game board TileBase 
    /// </summary>
    /// <param name="index"> tilemap cell index </param>
    /// <returns> Returns a TileBase tile</returns>
    private TileBase IndexToBoardTile(int index)
    {
        TileBase resultTile;

        var evenTile = _gameData.evenTile;
        var oddTile = _gameData.oddTile;
        var bounds = _gameData.boardSize;

        var rowIdx = index / bounds.size.x;
        var rowIdxParity = GetNumberParity(rowIdx);
        var cellParity = GetNumberParity(index);

        if (rowIdxParity)
            resultTile = cellParity ? evenTile : oddTile;
        else
            resultTile = cellParity ? oddTile : evenTile;
        
        return resultTile;

    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    
    /// <summary>
    /// Gets the parity of an integer number.
    /// </summary>
    /// <param name="number"> integer number</param>
    /// <returns>Returns true when a number is even and false otherwise</returns>
    private bool GetNumberParity(int number)
    {
        return (number) % 2 == 0;
    }
    
    //-----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Constructs the Checkerboard.
    /// </summary>
    private void SetupBoard()
    {
        _bounds = _gameData.boardSize;
        _swapTime = _gameData.gemMoveTime;
        var tileArray = new TileBase[_bounds.size.x * _bounds.size.y * _bounds.size.z];
        for (var index = 0; index < tileArray.Length; index++)
        {
            tileArray[index] = IndexToBoardTile(index);
        }
        var tilemap = GetComponent<Tilemap>();
        tilemap.SetTilesBlock(_bounds, tileArray);
    }
    
    //-----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Public function that initializes the game board
    /// </summary>
    /// <param name="gameData">GameDataScriptableObject game data</param>
    public void InitializeBoard(GameDataScriptableObject gameData)
    {
        _gameData = gameData;
        _matchesCandidates = new List<Gem>();
        _gems = new Gem[gameData.boardSize.size.x, gameData.boardSize.size.y];

        SetupBoard();
        FillBoard();
        FindPreMatches();
    }

    //-----------------------------------------------------------------------------------------------------------
    
    private List<Gem> UpdateBoard(List<Vector2Int> gemsPos)
    {
        if(gemsPos==null) return null;

        List<Gem> movedGems = new List<Gem>();
        
        //Extracts the first position of any column where a match has occurred.
        List<Vector2Int> columnsStartPos = new List<Vector2Int>();
        Vector2Int magicVector = new Vector2Int(1, 0);
        
        foreach (var pos in gemsPos)
        {
            var startPos = pos * magicVector;
            if(!columnsStartPos.Contains(startPos))
                columnsStartPos.Add(startPos);
        }

        var nullPosQueue = new Queue<Vector2Int>();
        foreach (var pos in columnsStartPos)
        {
            var checkedGems = CheckColumn(pos, ref nullPosQueue);
            if (checkedGems.Count > 0) movedGems = movedGems.Union(checkedGems).ToList() ;
            nullPosQueue.Clear();
        }
        return movedGems;
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    private List<Gem> CheckColumn(Vector2Int startPos, ref Queue<Vector2Int> nullPosQueue, SearchDir searchDir = SearchDir.Positive)
    {
        var firstEmptyPos = startPos;
        var offsetPos = Vector2Int.zero;
        var movedGems = new List<Gem>();


        offsetPos = searchDir == SearchDir.Positive ? Vector2Int.up : Vector2Int.down;
        var nextPos = startPos + offsetPos;

        if (!CheckBoardBounds(startPos)) return movedGems;
        
        var gem = GetGem(startPos);
        if (gem != null)
        {
            if (nullPosQueue.Count > 0)
            {
                var emptyPos = nullPosQueue.Dequeue();
                
                gem.Move(emptyPos);
                _gems[emptyPos.x, emptyPos.y] = gem;
                _gems[startPos.x, startPos.y] = null;
                gem.SetPos(emptyPos);
                
                movedGems.Add(gem);
                nullPosQueue.Enqueue(startPos);
            }
            var checkedGems = CheckColumn(nextPos, ref nullPosQueue, searchDir);
            if(checkedGems.Count!=0 ) movedGems= movedGems.Union(checkedGems).ToList();
        }
        else
        {
            nullPosQueue.Enqueue(startPos);
            var checkedGems =CheckColumn(nextPos, ref nullPosQueue, searchDir);
            if(checkedGems.Count!=0 ) movedGems = movedGems.Union(checkedGems).ToList();
        }

        return movedGems;
    }

    //-----------------------------------------------------------------------------------------------------------
    
    private List<Gem> FillBoard()
    {
        var gems = new List<Gem>();
        var newGemsPos = SortGems();

        foreach (var pos in newGemsPos)
        {
            var gem = GetGem(pos);
            gem.Move(pos);
            gems.Add(gem);
        }

        return gems;
    }

    //-----------------------------------------------------------------------------------------------------------
    
     private  List<Vector2Int> FindEmptyCells()
     {
         var emptyCellsPos = new List<Vector2Int>(); 
         var size = _bounds.size;
         for (var y = 0; y < size.y; y++)
         {
             for (var x = 0; x < size.x; x++)
             {
                 var pos = new Vector2Int(x, y);
                 var gem = GetGem(pos);

                 if (gem == null)
                 {
                     emptyCellsPos.Add(pos);
                 }
             }
         }

         return emptyCellsPos;
     }
    
    //-----------------------------------------------------------------------------------------------------------

    private GemType GetRandomGemType()
    {
        Array values = Enum.GetValues(typeof(GemType));
        return (GemType) values.GetValue(Random.Range(0, values.Length));
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    private List<Vector2Int> SortGems()
    {
        var emptyPositions = FindEmptyCells();
        foreach (var cellPos in emptyPositions)
        {
            var type = GetRandomGemType();
            var pos = new Vector3Int(cellPos.x, cellPos.y+_gameData.gemsStartHeight,0);
            
            var gem = Instantiate(_gameData.gem, pos, Quaternion.identity, transform);
            gem.Initialize(_gameData);
            gem.Type = type;
            _gems[cellPos.x, cellPos.y] = gem;
        }

        return emptyPositions;
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    private void DestroyMatchedGems(List<Gem> gems)
    {
        if (gems == null) return;
        foreach (var gem in gems)
        {
            if(gem==null) continue;
            DestroyGemAtPos(gem.Pos());
        }
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void DestroyGemAtPos(Vector2Int pos)
    {
        var gem = GetGem(pos);
        if (gem != null && gem.IsMatched)
        {
            _gems[pos.x, pos.y] = null;
            gem.DestroyGem();
        }
        
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private List<Gem> FindMatches(List<Gem> gems)
    {
        List<Gem> matches = new List<Gem>();
        if (gems == null) return matches;
        
        foreach (var gem in gems)
        {
            
            var matchedGems = FindMatches(gem.Pos());
            if(matchedGems!=null) matches = matches.Union(matchedGems).ToList();
        }
        return matches;
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    private List<Gem> FindMatches(Vector2Int pos)
    {
        List<Gem> matches = new List<Gem>();

        var currGem = GetGem(pos);
        if (currGem == null) return null;
        
        matches.Add(currGem);
        var vertMatches = FindMatchesOnAxis(pos);
        var horMatches = FindMatchesOnAxis(pos, Axis.Horizontal);
        
        

        if (vertMatches != null)
        {
            matches = matches.Union(vertMatches).ToList();
        }
        
        if (horMatches != null)
        {
            matches = matches.Union(horMatches).ToList();
        }

        if (matches.Count >= 3)
        {
            foreach (var gem in matches)
            {
                gem.IsMatched = true;
            }
        }
        else
        {
            matches = null;
        }

        return matches;
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    private List<Gem> FindMatchesOnAxis(Vector2Int pos, Axis axisSearch = Axis.Vertical )
    {
        List<Gem> matches = null;
        
        var currGem = GetGem(pos);

        if (currGem == null) return null; 
        
        List<Gem> negativeMatches = CheckMatches(pos, SearchDir.Negative,axisSearch);
        List<Gem> positiveMatches = CheckMatches(pos, SearchDir.Positive,axisSearch);

        var combinedMatches = negativeMatches.Union(positiveMatches).ToList();
        
        if (combinedMatches.Count >= 2)
        {
            matches = combinedMatches;
        }

        return matches;
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private List<Gem> CheckMatches(Vector2Int pos, SearchDir checkDir, Axis searchAxis = Axis.Vertical)
    {
        var matches = new List<Gem>();
        var gem = GetGem(pos.x, pos.y);
        
        var offsetPos = searchAxis == Axis.Vertical ? new Vector2Int(0, 1) : new Vector2Int(1, 0);
        
        if (checkDir == SearchDir.Negative) offsetPos = -1*offsetPos;
        
        var nextPosToCheck = pos + offsetPos;

        var nextGem = GetGem(nextPosToCheck);

        var isValidPos = CheckBoardBounds(nextPosToCheck);

        if (isValidPos && CompareGemType(gem, nextGem))
        {
            matches.Add(nextGem);
            matches = matches.Union(CheckMatches(nextPosToCheck, checkDir, searchAxis)).ToList();
        }

        return matches;
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    private void FindPreMatches()
    {
        for (var y = 0; y < _bounds.size.y; y++)
        {
            for (var x = 0; x < _bounds.size.x; x++)
            {
                RemoveMatches(new Vector2Int(x, y));
            }
        }
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void RemoveMatches(Vector2Int pos)
    {
        var gem = GetGem(pos.x, pos.y);

        var changeType = true;

        var upGem = GetGem(pos + Vector2Int.up);
        var downGem = GetGem(pos + Vector2Int.down);
        var leftGem = GetGem(pos + Vector2Int.left);
        var rightGem = GetGem(pos + Vector2Int.right);

        while (changeType)
        {
            changeType = false;
            if (CompareGemType(gem, upGem)) 
            {
                gem.Type = GetRandomGemType();
                changeType = true;
            }

            if (CompareGemType(gem, downGem))
            {
                gem.Type = GetRandomGemType();
                changeType = true;
            }

            if (CompareGemType(gem, leftGem))
            {
                gem.Type = GetRandomGemType();
                changeType = true;
            }

            if (CompareGemType(gem, rightGem))
            {
                gem.Type = GetRandomGemType();
                changeType = true;
            }
        }

    }
    
    //-----------------------------------------------------------------------------------------------------------

    
    private Gem GetGem(int x, int y)
    {
        return GetGem(new Vector2Int(x,y));
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private Gem GetGem(Vector2Int pos)
    {
        Gem gem = null;

        var isInBoard = CheckBoardBounds(pos);
        if (isInBoard)
        {
            gem = _gems[pos.x, pos.y];
        }
        
        return gem;
    }

    //-----------------------------------------------------------------------------------------------------------
    

    /// <summary>
    /// Check if pos is within the game board bounds
    /// </summary>
    /// <param name="pos">Vector2Int variable keeping a position </param>
    /// <returns>true if pos is within game board bounds, false otherwise </returns>
    private bool CheckBoardBounds(Vector2Int pos)
    {
        return _bounds.Contains(new Vector3Int(pos.x, pos.y, 0));
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private bool CompareGemType(Gem a, Gem b)
    {
        if (a == null || b == null) return false;

        if (a.Type == b.Type)
        {
            return true;
        }
        return false;
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void SwapGems(Gem selected, Gem overridden)
    {
        StartCoroutine(IEnumSwapGems(selected, overridden));
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    private IEnumerator IEnumSwapGems(Gem selected, Gem overridden)
    {
        var overriddenPos = overridden.Pos();
        var selectedPos = selected.Pos();
        overridden.Move(selectedPos);
        selected.Move(overriddenPos);
        _gems[overriddenPos.x, overriddenPos.y] = selected;
        _gems[selectedPos.x, selectedPos.y] = overridden;
        yield return new WaitForSeconds(_swapTime);
        
        var overriddenMatches = FindMatches(overriddenPos);
        var selectedMatches = FindMatches(selectedPos);

        var combineList = new List<Gem>();
        if(!(selectedMatches!=null||overriddenMatches!=null))
        {
             _gems[overriddenPos.x, overriddenPos.y] = overridden;
             _gems[selectedPos.x, selectedPos.y] = selected;
             selected.Move(selectedPos);
             overridden.Move(overriddenPos);
        }
        else
        {
            yield return new WaitForSeconds(_swapTime);

            if (selectedMatches != null )
            {
                combineList = combineList.Union(selectedMatches).ToList();
            }
            
            if(overriddenMatches!=null)
            {
                combineList = combineList.Union(overriddenMatches).ToList();
            }
            
            StartCoroutine(IEnumClearAndUpdateBoard(combineList)); 
            
        }
    }
    //-----------------------------------------------------------------------------------------------------------
    
    private IEnumerator IEnumClearAndUpdateBoard(List<Gem> gems)
    {
        List<Gem> movedGems = new List<Gem>();
        List<Gem> matches = new List<Gem>();
        
        yield return new WaitForSeconds(0.25f);
        var isFinished = false;

        while (!isFinished)
        {
            var gemsPos = GetGemsPos(gems);
            DestroyMatchedGems(gems);
            yield return new WaitForSeconds(_gameData.dissolveAnimationTime);
            movedGems = UpdateBoard(gemsPos);
            yield return new WaitForSeconds(0.3f);
            movedGems = movedGems.Union(FillBoard()).ToList();
            yield return new WaitForSeconds(0.3f);
            
            matches = FindMatches(movedGems);
            
            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(IEnumClearAndUpdateBoard(matches));
            }
        }
        
        yield return null;
    }

    //-----------------------------------------------------------------------------------------------------------
    
    private List<Vector2Int> GetGemsPos(List<Gem> gems)
    {
        var gemsPos = new List<Vector2Int>();
        foreach (var gem in gems)
        {
            if(gem==null) continue;

            gemsPos.Add(gem.Pos());
        }

        return gemsPos;
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void MouseUp()
    {
        _isDragging = false;
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void MouseDown()
    {
        var mousePos = GetValidMousePos();
        
        //For any click outside board bounds
        if (!CheckBoardBounds(mousePos)) return;
        
        if (_initGemSelection)
        {
            var gem = GetGem(mousePos);
            if(_selectedGem!=gem)
                SwapGems(_selectedGem, gem);
            _initGemSelection = false;
            _selectedGem.IsSelected = false;
            _selectedGem = null;
        }
        else
        {
            _selectedGem = GetGem(mousePos);
            _selectedGem.IsSelected = true;
            _isDragging = true;
            _initGemSelection = true;
        }
        
        
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void Drag()
    {
        if (_isDragging)
        {
            Vector2Int mousePos = GetValidMousePos();

            var overriddenGem = GetGem(mousePos);

            if (_selectedGem != overriddenGem)
            {
                _initGemSelection = false;
                SwapGems(_selectedGem, overriddenGem);
                _selectedGem.IsSelected = false;
                _isDragging = false;
                _selectedGem = null;
            }
        }
        
    }
    
    //-----------------------------------------------------------------------------------------------------------

    private Vector2Int GetValidMousePos()
    {
        
        Vector2 mousePos = _mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        if (!(Camera.main == null)) mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        var mousePosInt = Vector2Int.RoundToInt(mousePos);

        if (_selectedGem != null)
        {
            var clickPos = _selectedGem.Pos();

            var distance = Vector2Int.Distance(clickPos, mousePosInt);

            if (distance > 1|| !CheckBoardBounds(mousePosInt)) return clickPos;
        }
        
        return mousePosInt;
    }
    //-----------------------------------------------------------------------------------------------------------

}
