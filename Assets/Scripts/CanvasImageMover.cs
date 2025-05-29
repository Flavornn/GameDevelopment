using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasImageMover : MonoBehaviour
{
    [Header("Smoke Settings")]
    public RectTransform smoke1;     // First smoke image RectTransform
    public RectTransform smoke2;     // Second smoke image RectTransform
    public float smokeSpeed = 100f;  // Speed at which smoke moves horizontally (pixels/second)
    public bool smokeMovesLeft = true;  // Direction of smoke movement

    private float smokeWidth;        // Width of the smoke image, used for looping

    [Header("Spaceship Settings")]
    public RectTransform spaceshipParent;   // Parent container for spaceships on the canvas
    public Sprite spaceshipRightSprite;     // Sprite that moves to the right (x+)
    public Sprite spaceshipLeftSprite;      // Sprite that moves to the left (x-)
    public float spaceshipSpeed = 150f;     // Spaceship horizontal speed (pixels/second)
    public float spawnInterval = 2f;         // Interval between spaceship spawns (seconds)

    public Vector2 spaceshipYRange = new Vector2(-100f, 100f);   // Y range for random spawn position
    public Vector2 spaceshipScaleRange = new Vector2(0.5f, 1.5f);  // Min and max scale for spaceship (uniform scale)

    public float spaceshipSpawnXRight = -200f;  // X position for spawning spaceships moving right (offscreen left)
    public float spaceshipSpawnXLeft = 1200f;   // X position for spawning spaceships moving left (offscreen right)

    private List<RectTransform> activeSpaceships = new List<RectTransform>();

    void Start()
    {
        // Calculate smoke width from the first smoke image width
        smokeWidth = smoke1.rect.width;

        // Position smoke2 right next to smoke1
        if (smokeMovesLeft)
        {
            smoke1.anchoredPosition = Vector2.zero;
            smoke2.anchoredPosition = new Vector2(smokeWidth, 0);
        }
        else
        {
            smoke1.anchoredPosition = Vector2.zero;
            smoke2.anchoredPosition = new Vector2(-smokeWidth, 0);
        }

        // Start spawning spaceships repeatedly
        StartCoroutine(SpawnSpaceshipsRoutine());
    }

    void Update()
    {
        MoveSmoke();
        MoveSpaceships();
    }

    // Moves smoke images seamlessly left or right
    void MoveSmoke()
    {
        float direction = smokeMovesLeft ? -1f : 1f;
        Vector2 moveDelta = new Vector2(direction * smokeSpeed * Time.deltaTime, 0);

        smoke1.anchoredPosition += moveDelta;
        smoke2.anchoredPosition += moveDelta;

        // If smoke1 completely out of bounds, reposition it to the other side
        if (smokeMovesLeft && smoke1.anchoredPosition.x <= -smokeWidth)
        {
            smoke1.anchoredPosition = new Vector2(smoke2.anchoredPosition.x + smokeWidth, smoke1.anchoredPosition.y);
        }
        else if (!smokeMovesLeft && smoke1.anchoredPosition.x >= smokeWidth)
        {
            smoke1.anchoredPosition = new Vector2(smoke2.anchoredPosition.x - smokeWidth, smoke1.anchoredPosition.y);
        }

        // Same for smoke2
        if (smokeMovesLeft && smoke2.anchoredPosition.x <= -smokeWidth)
        {
            smoke2.anchoredPosition = new Vector2(smoke1.anchoredPosition.x + smokeWidth, smoke2.anchoredPosition.y);
        }
        else if (!smokeMovesLeft && smoke2.anchoredPosition.x >= smokeWidth)
        {
            smoke2.anchoredPosition = new Vector2(smoke1.anchoredPosition.x - smokeWidth, smoke2.anchoredPosition.y);
        }
    }

    // Coroutine to spawn spaceships at intervals
    IEnumerator SpawnSpaceshipsRoutine()
    {
        while (true)
        {
            SpawnSpaceship();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSpaceship()
    {
        // Decide which spaceship to spawn (left or right)
        bool spawnRight = (Random.value > 0.5f);

        // Create new GameObject with Image component
        GameObject spaceshipGO = new GameObject("Spaceship", typeof(RectTransform), typeof(Image));
        spaceshipGO.transform.SetParent(spaceshipParent, false);

        RectTransform rt = spaceshipGO.GetComponent<RectTransform>();
        Image img = spaceshipGO.GetComponent<Image>();

        // Set sprite and initial position depending on direction
        if (spawnRight)
        {
            img.sprite = spaceshipRightSprite;
            rt.anchoredPosition = new Vector2(spaceshipSpawnXRight, Random.Range(spaceshipYRange.x, spaceshipYRange.y));
        }
        else
        {
            img.sprite = spaceshipLeftSprite;
            rt.anchoredPosition = new Vector2(spaceshipSpawnXLeft, Random.Range(spaceshipYRange.x, spaceshipYRange.y));
        }

        // Set random scale within range, same scale for X and Y to keep aspect ratio
        float scale = Random.Range(spaceshipScaleRange.x, spaceshipScaleRange.y);
        rt.localScale = new Vector3(scale, scale, 1);

        // Add this spaceship to active list with movement info in a component
        SpaceshipMover mover = spaceshipGO.AddComponent<SpaceshipMover>();
        mover.Initialize(spawnRight ? Vector2.right : Vector2.left, spaceshipSpeed);

        activeSpaceships.Add(rt);
    }

    // Move spaceships every frame and destroy if out of screen bounds
    void MoveSpaceships()
    {
        for (int i = activeSpaceships.Count - 1; i >= 0; i--)
        {
            RectTransform rt = activeSpaceships[i];
            SpaceshipMover mover = rt.GetComponent<SpaceshipMover>();

            if (mover == null)
            {
                activeSpaceships.RemoveAt(i);
                Destroy(rt.gameObject);
                continue;
            }

            mover.Move(Time.deltaTime);

            // Check if out of horizontal bounds of canvas (assuming canvas width ~ Screen.width)
            float screenWidth = spaceshipParent.rect.width;

            if (mover.direction.x > 0 && rt.anchoredPosition.x > screenWidth + 200) // some buffer
            {
                activeSpaceships.RemoveAt(i);
                Destroy(rt.gameObject);
            }
            else if (mover.direction.x < 0 && rt.anchoredPosition.x < -200)
            {
                activeSpaceships.RemoveAt(i);
                Destroy(rt.gameObject);
            }
        }
    }
}

// Helper component to move a spaceship
public class SpaceshipMover : MonoBehaviour
{
    public Vector2 direction;
    public float speed;

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
    }

    public void Move(float deltaTime)
    {
        RectTransform rt = transform as RectTransform;
        rt.anchoredPosition += direction * speed * deltaTime;
    }
}
