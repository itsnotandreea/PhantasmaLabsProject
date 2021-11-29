using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GoalSpawner : MonoBehaviour
{
    #region Getters
    
    public bool HasGoals => InstantiatedGoals.Count > 0;
    public Random Random
    {
        get
        {
            if (_random == null)
                _random = new Random();

            return _random;
        }
        set
        {
            _random = value;
        }
    }
    public List<Transform> InstantiatedGoals => _instantiatedGoals;
    
    #endregion
    
    #region Fields
    
    [SerializeField] private GameObject _goalPrefab = null;
    [SerializeField] private Transform _goalsHolder = null;
    [SerializeField] private Vector2Int _randomGoalsToSpawn = new Vector2Int(5, 10);
    [SerializeField] private Vector2 _goalSpawningAreaDimensions = new Vector2(90, 90);
    
    #endregion

    #region Private Properties
    
    private List<Transform> _instantiatedGoals = new List<Transform>();
    private Random _random;
    
    #endregion
    
    #region UnitTestInitialize

    public void UnitTestInitialize(GameObject goalPrefab)
    {
        _goalPrefab = goalPrefab;

        SpawnRandomNumberOfGoals();
    }
    
    #endregion
    
    #region SpawnRandomNumberOfGoals / SpawnGoal / DestroyGoal
    
    public void SpawnRandomNumberOfGoals()
    {
        var goalsToSpawn = Random.Next(_randomGoalsToSpawn.x, _randomGoalsToSpawn.y);

        for (var i = 0; i < goalsToSpawn; i++)
        {
            SpawnGoal();
        }
    }

    private void SpawnGoal()
    {
        var newGoal = Instantiate(_goalPrefab, _goalsHolder);
        newGoal.transform.position = GetRandomPosition();

        _instantiatedGoals.Add(newGoal.transform);
    }
    
    public void DestroyGoal(Transform goalTransform)
    {
        if (_instantiatedGoals.Contains(goalTransform))
            _instantiatedGoals.Remove(goalTransform);
        
        Destroy(goalTransform.gameObject);
    }
    
    #endregion
    
    #region GetClosestGoal

    public Transform GetClosestGoal(Vector3 position)
    {
        var closestDistance = 999999.0f;
        Transform closestTransform = null;

        foreach (var entry in InstantiatedGoals)
        {
            var distance = Vector3.Distance(position, entry.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = entry;
            }
        }

        return closestTransform;
    }
    
    #endregion
    
    #region GetRandomPosition

    private Vector3 GetRandomPosition()
    {
        var position = transform.position;
        
        Vector3 randomOffset;
        var randomXOffset = Random.Next((int)-_goalSpawningAreaDimensions.x, (int)_goalSpawningAreaDimensions.x);
        var randomYOffset = Random.Next((int)-_goalSpawningAreaDimensions.y, (int)_goalSpawningAreaDimensions.y);
        randomOffset.x = (float)randomXOffset;
        randomOffset.y = 0;
        randomOffset.z = (float)randomYOffset;

        return position + randomOffset;
    }
    
    #endregion
    
    #region OnDrawGizmosSelected

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        var startPoint = transform.position;
        var size = new Vector3(_goalSpawningAreaDimensions.x * 2, 0, _goalSpawningAreaDimensions.y * 2);
        Gizmos.DrawCube(startPoint, size);
    }
    
    #endregion
}