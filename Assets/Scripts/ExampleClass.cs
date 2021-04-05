// Fill area on tilemap with checkerboard pattern of tileA and tileB
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExampleClass : MonoBehaviour
{
    public TileBase tileA;
    public TileBase tileB;
    public BoundsInt area;
    public Transform gem;

    void Start()
    {
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        for (int index = 0; index < tileArray.Length; index++)
        {
            tileArray[index] = IndexToTile(index);
        }
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.SetTilesBlock(area, tileArray);

        //var wPos = tilemap.layoutGrid.CellToWorld(new Vector3Int(2, 2, 0));
        var wPos = tilemap.GetCellCenterWorld(new Vector3Int(7, 7, 0));
        gem.position = wPos;
    }

    private TileBase IndexToTile(int index)
    {
        TileBase resultTile;

        var rowIdx = index / area.size.x;
        var rowIdxParity = GetNumberParity(rowIdx);
        var cellParity = GetNumberParity(index);

        if (rowIdxParity)
            resultTile = cellParity ? tileA : tileB;
        else
            resultTile = cellParity ? tileB : tileA;
        
        return resultTile;

    }
    
    /// <summary>
    /// Gets the parity of an integer number.
    /// </summary>
    /// <param name="number"> integer number</param>
    /// <returns>Returns true when a number is even and false otherwise</returns>
    private bool GetNumberParity(int number)
    {
        return (number) % 2 == 0 ? true : false;
    }
}