using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// TODO
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameDataScriptableObject", order = 1)]
public class GameDataScriptableObject : ScriptableObject
{
    [Header("Gameboard Settings")]
    public TileBase evenTile;
    public TileBase oddTile;
    public BoundsInt boardSize;
    public float gemMoveTime;
    public float gemRotationSpeed;
    public AnimationCurve moveCurve;
    public int gemsStartHeight;

    [Header("Gems Sprite Settings")]
    [SerializeField]
    private Sprite gemSprite;
    [SerializeField]
    private Material gemMaterial;

    public Color purple;
    public Color pink;
    public Color yellow;
    public Color blue;
    public Color green;
    public float gemCreationAlphaValue =0.45f;
    
    [Header("Gems Effects Settings")]
    public float alpha = 1;

    public float dissolveAnimationTime;
    public float minDissolveValue;
    public float maxDissolveValue;
    public AnimationCurve dissolveCurve;

    [Header("Prefabs")] public Gem gem;

    public Sprite GetSprite()
    {
        if (gemSprite is null)
        {
            Debug.LogError("Please configure the Gem's sprite image on GameData Asset.");
        }

        return gemSprite;
    }

    public Material GetMaterial()
    {
        if (gemMaterial is null)
        {
            Debug.LogError("Please configure the Gem's material on GameData Asset.");
        }

        return gemMaterial;
    }
    
    public Color GetColorByGemType(GemType type)
    {
        Color color = new Color();
        
        switch (type)
        {
            case GemType.Purple:
                color = purple;
                break;
             case GemType.Pink:
                color = pink;
                break;
             case GemType.Yellow:
                color = yellow;
                break;
             case GemType.Blue:
                color = blue;
                break;
             case GemType.Green:
                color = green;
                break;
        }

        color.a =gemCreationAlphaValue;
        return color;
    }
    
}


