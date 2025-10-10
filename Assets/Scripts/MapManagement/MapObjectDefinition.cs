using UnityEngine;

[CreateAssetMenu(menuName = "Game/MapObjectDefinition")]
public class MapObjectDefinition : ScriptableObject
{
    public string objectId;
    public ObjectType type;
    public GameObject[] sizeVariants;
    public Vector2 colliderSize = new(0.8f, 0.8f);
}
