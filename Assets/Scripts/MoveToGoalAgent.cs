using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private GoalSpawner _goalSpawner = null;
    [SerializeField] private MeshRenderer _floorMeshRenderer = null;
    [SerializeField] private Material _redMaterial = null;
    [SerializeField] private Material _greenMaterial = null;

    private Transform _targetTransform;
    
    public override void OnEpisodeBegin()
    {
        if (!_goalSpawner.HasGoals)
            _goalSpawner.SpawnRandomNumberOfGoals();
        
        _targetTransform = GetClosestTransform(_goalSpawner.InstantiatedGoals);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var moveX = actions.ContinuousActions[0];
        var moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * _moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case TagsNames.GoalTagName:
                _floorMeshRenderer.material = _greenMaterial;
                _goalSpawner.DestroyGoal(other.transform);
                SetReward(+1.0f);
                EndEpisode();
                break;
            case TagsNames.BorderTagName:
                _floorMeshRenderer.material = _redMaterial;
                transform.localPosition = Vector3.zero;
                SetReward(-1.0f);
                EndEpisode();
                break;
        }
    }

    private Transform GetClosestTransform(List<Transform> transforms)
    {
        var closestDistance = 999999.0f;
        Transform closestTransform = null;

        foreach (var entry in transforms)
        {
            var distance = Vector3.Distance(transform.position, entry.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = entry;
            }
        }

        return closestTransform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (Application.isPlaying)
        {
            Gizmos.DrawLine(transform.position, _targetTransform.position);
        }
    }
}