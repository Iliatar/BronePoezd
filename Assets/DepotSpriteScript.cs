using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BronePoezd.Terrain;

public class DepotSpriteScript : MonoBehaviour
{

    SpriteRenderer sprite;
    TerrainManager terrainManager;
    float xScaler, yScaler;

    public void Initialize()
    {
        Debug.Log("DepotSpriteScript Awake() executed");
        sprite = GetComponent<SpriteRenderer>();
        terrainManager = FindObjectOfType<TerrainManager>();
        float tileSize = terrainManager.TileSize;
        xScaler = 0.6f;
        yScaler = 0.8f;
        Vector2 spriteSize = new Vector2(tileSize * xScaler, tileSize * yScaler);
        sprite.size = spriteSize;
        RemoveSprite();
    }

    public void PlaceSprite(Vector2Int tilePosition)
    {
        Vector2 tileWorldPosition = terrainManager.GetTileMatrix()[tilePosition.x, tilePosition.y].transform.position;
        float newX = tileWorldPosition.x;
        float newY = tileWorldPosition.y + terrainManager.TileSize / 2 * (yScaler - 1);
        Vector3 newPosition = new Vector3(newX, newY, -1);
        transform.position = newPosition;
        sprite.enabled = true;
    }

    public void RemoveSprite()
    {
        sprite.enabled = false;
    }

}
