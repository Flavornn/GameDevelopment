using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMover : MonoBehaviour
{
    [Header("Smoke Settings (Leftward)")]
    public RectTransform smoke1;
    public RectTransform smoke2;
    public float smokeSpeed = 50f;

    [Header("Smoke Settings (Rightward)")]
    public RectTransform smoke3;
    public RectTransform smoke4;
    public float reverseSmokeSpeed = 30f;

    [Header("Cloud Settings (Leftward)")]
    public RectTransform cloud1;
    public RectTransform cloud2;
    public float cloudSpeed = 20f;

    private float canvasWidth = 1920f;

    private float smokeWidth;
    private float reverseSmokeWidth;
    private float cloudWidth;

    [Header("Spaceship Settings")]
    public GameObject spaceshipRightPrefab;
    public GameObject spaceshipLeftPrefab;
    public float spaceshipInterval = 2f;
    public Vector2 yRange = new Vector2(100, 800);
    public Vector2 scaleRange = new Vector2(0.5f, 1.2f);
    public float spaceshipSpeed = 150f;
    public RectTransform spaceshipParent;

    void Start()
    {
        if (smoke1 != null) smokeWidth = smoke1.rect.width;
        if (smoke3 != null) reverseSmokeWidth = smoke3.rect.width;
        if (cloud1 != null) cloudWidth = cloud1.rect.width;

        StartCoroutine(SpawnSpaceships());
    }

    void Update()
    {
        // Smoke scrolling left
        MoveSeamless(smoke1, smokeSpeed, -1, smokeWidth);
        MoveSeamless(smoke2, smokeSpeed, -1, smokeWidth);

        // Smoke scrolling right
        MoveSeamless(smoke3, reverseSmokeSpeed, 1, reverseSmokeWidth);
        MoveSeamless(smoke4, reverseSmokeSpeed, 1, reverseSmokeWidth);

        // Clouds scrolling left
        MoveSeamless(cloud1, cloudSpeed, -1, cloudWidth);
        MoveSeamless(cloud2, cloudSpeed, -1, cloudWidth);
    }

    void MoveSeamless(RectTransform element, float speed, int direction, float width)
    {
        if (element == null) return;

        element.anchoredPosition += Vector2.right * direction * speed * Time.deltaTime;

        if (direction < 0 && element.anchoredPosition.x <= -width)
        {
            element.anchoredPosition += new Vector2(width * 2f, 0);
        }
        else if (direction > 0 && element.anchoredPosition.x >= width)
        {
            element.anchoredPosition -= new Vector2(width * 2f, 0);
        }
    }

    IEnumerator SpawnSpaceships()
    {
        while (true)
        {
            yield return new WaitForSeconds(spaceshipInterval);

            bool goRight = Random.value > 0.5f;
            GameObject prefab = goRight ? spaceshipRightPrefab : spaceshipLeftPrefab;

            GameObject ship = Instantiate(prefab, spaceshipParent);
            RectTransform rect = ship.GetComponent<RectTransform>();

            float y = Random.Range(yRange.x, yRange.y);
            float scale = Random.Range(scaleRange.x, scaleRange.y);
            rect.localScale = new Vector3(scale, scale, 1);

            float startX = goRight ? -100f : canvasWidth - 1000f;
            rect.anchoredPosition = new Vector2(startX, y);

            StartCoroutine(MoveSpaceship(rect, goRight ? Vector2.right : Vector2.left));
        }
    }

    IEnumerator MoveSpaceship(RectTransform ship, Vector2 direction)
    {
        while (true)
        {
            ship.anchoredPosition += direction * spaceshipSpeed * Time.deltaTime;

            if (ship.anchoredPosition.x < -400f || ship.anchoredPosition.x > canvasWidth + 400f)
            {
                Destroy(ship.gameObject);
                yield break;
            }

            yield return null;
        }
    }
}
