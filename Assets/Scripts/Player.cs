using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Player : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile frameTile;

    private Vector3Int currentPosition;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void SetPosition(Vector3Int newPosition)
    {
        //Debug.Log($"Moving player to {newPosition}");
        
        // remove old frame
        tilemap.SetTile(currentPosition, null);

        // place new frame
        tilemap.SetTile(newPosition, frameTile);

        currentPosition = newPosition;
    }
}