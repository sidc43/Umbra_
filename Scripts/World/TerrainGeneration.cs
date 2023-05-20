using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    public Texture2D noiseTexture;
    public Tilemap tilemap;
    public Tile[] water;
    public Tile[] sand;
    public Tile[] snow;
    public Tile[] grass;
    public Tile stoneBlock;
    public float seed;
    public int worldWidth = 100, worldHeight = 100;
    public float noiseFreq = 0.03f;

    [SerializeField]
    private Tile _oldTile;

    private void Start() 
    {
        seed = UnityEngine.Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float currColor = noiseTexture.GetPixel(x, y).r;

                #region Water
                if (currColor < 0.2f)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), water[Random.Range(0, water.Length - 1)]);
                }
                #endregion

                #region Sand
                else if (currColor >= 0.2f && currColor <= 0.35f)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), sand[Random.Range(0, sand.Length - 1)]);
                }
                #endregion

                #region Grass
                else if (currColor > 0.35f && currColor <= 0.82f)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), grass[Random.Range(0, grass.Length - 1)]);
                }
                #endregion

                #region Snow
                else 
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), snow[Random.Range(0, snow.Length - 1)]);
                }
                #endregion
            }
        }
    }

    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldWidth, worldHeight);
        
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * noiseFreq, (y + seed) * noiseFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }

}
