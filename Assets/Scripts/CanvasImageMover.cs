using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasImageMover : MonoBehaviour
{
    [Header("Smoke Settings")]
    public RectTransform smoke1;
    public RectTransform smoke2;
    public float smokeSpeed = 50f;

    private float smokeWidth;

    [Header("Spaceship Settings")]
    public GameObject spaceshipRightPrefab;
    public GameObject spaceshipLeftPrefab;
    public float spaceshipInterval = 2f;
    public Vector2 yRange = new Vector2(100, 800);
    public Vector2 scaleRange = new Vector2(0.5f, 1.2f);
    public float spaceshipSpeed = 150f;
    public RectTransform spaceshipParent;

    private float canvasWidth = 1920f;

    void Start()
    {
        smokeWidth = smoke1.rect.width;
        StartCoroutine(SpawnSpaceships());
    }

    void Update()
    {
        MoveSmoke(smoke1);
        MoveSmoke(smoke2);
    }

    void MoveSmoke(RectTransform smoke)
    {
        smoke.anchoredPosition += Vector2.left * smokeSpeed * Time.deltaTime;

        if (smoke.anchoredPosition.x <= -smokeWidth)
        {
            smoke.anchoredPosition += new Vector2(smokeWidth * 2f, 0);
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
            ship.anchoredPosition += (Vector2)direction * spaceshipSpeed * Time.deltaTime;

            if (ship.anchoredPosition.x < -400f || ship.anchoredPosition.x > canvasWidth + 400f)
            {
                Destroy(ship.gameObject);
                yield break;
            }

            yield return null;
        }
    }
}
