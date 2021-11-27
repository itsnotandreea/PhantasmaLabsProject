using UnityEngine;

public class GoalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _goalPrefab = null;
    [SerializeField] private Transform _goalsHolder = null;
    [SerializeField] private Vector2Int _randomGoalsToSpawn = new Vector2Int(5, 10);
    [SerializeField] private Vector2 _goalSpawningDimensions = new Vector2(10, 10);

    private void Start()
    {
        SpawnRandomNumberOfGoals();
    }

    private void SpawnRandomNumberOfGoals()
    {
        var goalsToSpawn = Random.Range(_randomGoalsToSpawn.x, _randomGoalsToSpawn.y);

        for (var i = 0; i < goalsToSpawn; i++)
        {
            SpawnGoal();
        }
    }

    private void SpawnGoal()
    {
        var newGoal = Instantiate(_goalPrefab, _goalsHolder);
        newGoal.transform.position = GetRandomPosition();
    }

    private Vector3 GetRandomPosition()
    {
        var position = transform.position;
        
        Vector3 randomOffset;
        randomOffset.x = Random.Range(-_goalSpawningDimensions.x, _goalSpawningDimensions.x);
        randomOffset.y = 0;
        randomOffset.z = Random.Range(-_goalSpawningDimensions.y, _goalSpawningDimensions.y);

        return position + randomOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        var startPoint = transform.position;
        var size = new Vector3(_goalSpawningDimensions.x * 2, 0, _goalSpawningDimensions.y * 2);
        Gizmos.DrawCube(startPoint, size);
    }
}