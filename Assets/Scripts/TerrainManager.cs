using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BronePoezd.Terrain
{
    class TerrainManager : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> terrainPrefabs;
        [SerializeField]
        int fieldWidth, fieldHeight;
        [SerializeField]
        float tileSize;
        TerrainTile[,] tileMatrix;

        public float TileSize
        {
            get
            {
                return tileSize;
            }
        }

        private void Start()
        {
            CreateTerrain();
        }

        void CreateTerrain()
        {
            tileMatrix = new TerrainTile[fieldWidth, fieldHeight];

            Vector2 tileSizeVector = new Vector2(tileSize, tileSize);
            foreach (GameObject prefab in terrainPrefabs)
            {
                prefab.GetComponent<SpriteRenderer>().size = tileSizeVector;
            }

            Transform thisTransform = GetComponent<Transform>();
            for (int widthCursor = 0; widthCursor < fieldWidth; widthCursor++)
            {
                for (int heightCursor = 0; heightCursor < fieldHeight; heightCursor++)
                {
                    Vector2 newTilePosition = new Vector2(widthCursor * tileSize, heightCursor * tileSize);
                    GameObject newTile = InstantiateTile(terrainPrefabs[0], newTilePosition, thisTransform);
                    newTile.name = string.Format("Tile [{0}, {1}]", widthCursor, heightCursor);
                    TerrainTile newTileScript = newTile.GetComponent<TerrainTile>();
                    newTileScript.SetPosition(new Vector2Int(widthCursor, heightCursor));
                    tileMatrix[widthCursor, heightCursor] = newTileScript;

                }
            }

            Camera.main.GetComponent<CameraController>().InitializeCamera(fieldWidth * tileSize, fieldHeight * tileSize);
        }

        GameObject InstantiateTile (GameObject prefab, Vector2 position, Transform parent)
        {
            return Instantiate(prefab, position, new Quaternion(), parent);
        }

        public Vector2Int GetFieldSize()
        {
            return new Vector2Int(fieldWidth, fieldHeight);
        }

        public TerrainTile[,] GetTileMatrix()
        {
            return tileMatrix;
        }
    }
}
