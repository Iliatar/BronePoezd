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
                    InstantiateTile(terrainPrefabs[0], newTilePosition, thisTransform);
                }
            }

            Camera.main.GetComponent<CameraController>().InitializeCamera(fieldWidth * tileSize, fieldHeight * tileSize);
        }

        void InstantiateTile (GameObject prefab, Vector2 position, Transform parent)
        {
            Instantiate(prefab, position, new Quaternion(), parent);
        }
    }
}
