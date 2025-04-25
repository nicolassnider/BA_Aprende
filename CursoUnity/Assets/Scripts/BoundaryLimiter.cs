using UnityEngine;

public class BoundaryLimiter : MonoBehaviour
{
    public Transform mainChar; // Reference to the MainChar object
    public Transform northWall; // Reference to the North wall
    public Transform southWall; // Reference to the South wall
    public Transform westWall;  // Reference to the West wall
    public Transform eastWall;  // Reference to the East wall

    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        // Calculate the bounds based on the walls' positions
        minBounds = new Vector2(westWall.position.x, southWall.position.y);
        maxBounds = new Vector2(eastWall.position.x, northWall.position.y);
    }

    void Update()
    {
        // Clamp the MainChar's position within the bounds
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(mainChar.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(mainChar.position.y, minBounds.y, maxBounds.y),
            mainChar.position.z // Keep the z-position unchanged
        );

        mainChar.position = clampedPosition;
    }
}