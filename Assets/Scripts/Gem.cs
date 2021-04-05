using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO
/// </summary>
public enum GemType
{
    Purple,
    Pink,
    Yellow,
    Blue,
    Green
}

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour
{
    private GemType _gemType;
    private bool _isMatched;
    private bool _isSelected;
    private bool _allowRotation;
    private SpriteRenderer _spriteRenderer;
    private bool _isInit;
    private GameDataScriptableObject _gameData;
    private Vector3Int _nextPos; // _lastPos;
    private Vector2Int _currPos;
    private bool _isMoving;
    private Queue<Vector3Int> _moves;
    private float _moveTime;
    private static readonly int DissolveColor = Shader.PropertyToID("_DissolveColor");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    //-----------------------------------------------------------------------------------------------------------
    public GemType Type
    {
        get => _gemType;
        set => SetType(value);
    }
    //-----------------------------------------------------------------------------------------------------------

    public bool IsMatched
    {
        get => _isMatched;
        set => _isMatched = value;
    }
    //-----------------------------------------------------------------------------------------------------------
    
    public bool IsSelected
    {
        get => _isSelected;
        set => _isSelected = value;
    }

    //-----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="gameData"></param>
    public void Initialize(GameDataScriptableObject gameData)
    {
        _isInit = true;
        _gameData = gameData;
        _moveTime = _gameData.gemMoveTime;
        _spriteRenderer.sprite = _gameData.GetSprite();
        _spriteRenderer.material = _gameData.GetMaterial();
    }
    
    //-----------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //-----------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _isInit = false;
        _isMoving = false;
        _nextPos =  Vector3Int.RoundToInt(transform.position);
        //_lastPos = _nextPos;
        _moves = new Queue<Vector3Int>();
    }

    //-----------------------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        if (_isSelected)
        {
            Rotate();
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
        
        float alpha = _gameData.alpha;
        var color = _gameData.GetColorByGemType(_gemType);
        color.a = alpha;
        _spriteRenderer.material.SetColor(EmissionColor, color);
    }
    
    //-----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <returns> Returns a Vector2Int Gem's position</returns>
    public Vector2Int Pos()
    {
         
        return  _currPos;
    }
    
    //-----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <returns> Returns a Vector2Int Gem's position</returns>
    public void SetPos(Vector2Int pos)
    {
         _currPos = pos;
        
    }
    
    //-----------------------------------------------------------------------------------------------------------

    public void Move(Vector2Int pos)
    {
        var tempPos = new Vector2Int(_nextPos.x, _nextPos.y);

        var nextPos = new Vector3Int(pos.x, pos.y, 0);
        
        if(!_isMoving)
            StartCoroutine(IEnumUpdateMove(nextPos, _gameData.gemMoveTime));
    }

    //-----------------------------------------------------------------------------------------------------------
    
    private void UpdateMove()
    {
        if (_moves.Count > 0 && !_isMoving)
            _nextPos = _moves.Dequeue();

        if (transform.position == _nextPos)
        {
            _isMoving = false;
            return;
        }

        _isMoving = true;
        var step = 10.0f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _nextPos, step);
    }
    
    //-----------------------------------------------------------------------------------------------------------
    private IEnumerator IEnumUpdateMove(Vector3 nextPos, float time)
    {
        _isMoving = true;

        var moveCurve = _gameData.moveCurve;

        bool reached = false;//transform.position == _nextPos;

        var pos = transform.position;
        var timeUntilNow = 0.0f;
        
        while (!reached)
        {
            if (Vector3.Distance(transform.position, nextPos) < 0.01f)
            {
                reached = true;
                transform.position = nextPos;
                _currPos = new Vector2Int((int) nextPos.x, (int) nextPos.y);
                break;
            }

            timeUntilNow += Time.deltaTime;
            var t = Mathf.Clamp01(timeUntilNow / time);

            transform.position = Vector3.Lerp(pos, nextPos, moveCurve.Evaluate(t));
            
            yield return null;
        }
        _isMoving = false;

    }
    
    //-----------------------------------------------------------------------------------------------------------

    private void SetType(GemType type)
    {
        if (!_isInit)
        {
            Debug.LogError("Please initialize the Gem first");
            return;
        }

        _gemType = type;
        var color = _gameData.GetColorByGemType(type);
        _spriteRenderer.color = color;
        _spriteRenderer.material.SetColor(DissolveColor, color);
        _spriteRenderer.material.SetColor(EmissionColor, color);
        
    }

    //-----------------------------------------------------------------------------------------------------------
    private void Rotate()
    {
        var speed = _gameData.gemRotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward,speed);
    }
    //-----------------------------------------------------------------------------------------------------------
    
    public void DestroyGem()
    {
        StartCoroutine(IEnumDestroyGem());
    }

    private IEnumerator IEnumDestroyGem()
    {
        
        var timeUntilNow = 0.0f;
        var isDestroyed = false;
        var time = _gameData.dissolveAnimationTime;
        var minDissovelVal = _gameData.minDissolveValue;
        var maxDissovelVal = _gameData.maxDissolveValue;
        var dissolveCurve = _gameData.dissolveCurve;

        while (!isDestroyed)
        {
            if ((time - timeUntilNow) < 0.01f)
            {
                isDestroyed = true;
            }
            timeUntilNow += Time.deltaTime;
            var t = Mathf.Clamp01(timeUntilNow / time);
            var currDissovelVal = Mathf.Lerp(minDissovelVal, maxDissovelVal, dissolveCurve.Evaluate(t));
            _spriteRenderer.material.SetFloat(DissolveAmount, currDissovelVal);
            yield return null;
            
        }
       
    }
    //-----------------------------------------------------------------------------------------------------------
}
