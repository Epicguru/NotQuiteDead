
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public float Duration = 0.2f;
    public float SpawnDistance = 0.3f;
    public Vector2 RandomDistance = new Vector2(0.5f, 0.8f);
    public float Value;
    public TextMesh Text;
    public AnimationCurve AlphaCurve;
    public AnimationCurve PositionCurve;

    private Vector2 targetPos;
    private Vector2 initialPos;
    private float initialDuration;

    public void Start()
    {
        Text.GetComponent<Renderer>().sortingLayerName = "Effects";
    }

    public void Init()
    {
        Vector2 pointOnCircle = Random.insideUnitCircle.normalized;
        initialPos = (Vector2)transform.position + pointOnCircle * SpawnDistance;
        float dst = Random.Range(RandomDistance.x, RandomDistance.y);
        initialDuration = Duration;
        targetPos = (Vector2)transform.position + pointOnCircle * dst;

        Text.text = Mathf.RoundToInt(Value).ToString();
    }

    public void Update()
    {
        // Decrease lifespan
        Duration = Mathf.Clamp(Duration - Time.deltaTime, 0f, float.MaxValue);

        // Closer to zero as time ends...
        float a = Duration / initialDuration;

        // Set text colour.
        Color c = Text.color;
        c.a = AlphaCurve.Evaluate(a);
        Text.color = c;

        // Set position.
        transform.position = Vector2.Lerp(initialPos, targetPos, PositionCurve.Evaluate(a));

        if(a == 0)
        {
            // Pool.
            ObjectPool.Destroy(gameObject, PoolType.DAMAGE_NUMBER);
        }
    }

    public static void Spawn(Vector2 position, float value, float duration = 0.4f, float spawnDistance = 0.5f,  float minDistance = 0.7f, float maxDistance = 0.95f)
    {
        GameObject go = ObjectPool.Instantiate(Spawnables.I.DamageNumber, PoolType.DAMAGE_NUMBER);
        go.transform.position = position;

        DamageNumber number = go.GetComponent<DamageNumber>();

        number.Value = value;
        number.Duration = duration;
        number.SpawnDistance = spawnDistance;
        number.RandomDistance.x = minDistance;
        number.RandomDistance.y = maxDistance;

        number.Init();
    }
}