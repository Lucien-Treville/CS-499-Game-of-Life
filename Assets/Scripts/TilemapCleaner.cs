using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapCleaner : MonoBehaviour
{
    public Tilemap targetTilemap;

    [ContextMenu("Clean Duplicate Tiles")]
    public void CleanUp()
    {
        if (targetTilemap == null) targetTilemap = GetComponent<Tilemap>();
        
        Debug.Log($"Cleaning {targetTilemap.name}...");
        
        // 1. Get the bounds of the map
        targetTilemap.CompressBounds();
        BoundsInt bounds = targetTilemap.cellBounds;
        
        // 2. Iterate through every position
        int fixedCount = 0;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // Check if a tile exists here
                if (targetTilemap.HasTile(pos))
                {
                    // Get the tile asset
                    TileBase tile = targetTilemap.GetTile(pos);
                    
                    // FORCE reset the tile. 
                    // This deletes any "stacked" tiles at odd Z-positions 
                    // and places a single clean one at Z=0 for that layer.
                    targetTilemap.SetTile(pos, null); 
                    targetTilemap.SetTile(pos, tile);
                    
                    // Optional: Reset transform/rotation if you have messy transforms
                    targetTilemap.SetTransformMatrix(pos, Matrix4x4.identity);
                    
                    fixedCount++;
                }
            }
        }
        
        Debug.Log($"Refreshed {fixedCount} tiles. Internal overlaps should be gone.");
    }
}