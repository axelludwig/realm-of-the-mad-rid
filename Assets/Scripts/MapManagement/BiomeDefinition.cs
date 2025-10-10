using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/BiomeDefinition")]
public class BiomeDefinition : ScriptableObject
{
    public string biomeId;
    public TileBase defaultGround;
    [Range(0, 1)] public float treeDensity = 0.05f;
    public MapObjectDefinition[] spawnableObjects;
}