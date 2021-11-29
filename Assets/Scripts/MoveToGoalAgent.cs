using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    #region Getters
    
    public Transform TargetTransform => _targetTransform;
    
    #endregion
    
    #region Fields
    
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private GoalSpawner _goalSpawner = null;
    
    #endregion
    
    #region Private Properties

    private Transform _targetTransform;
    
    #endregion
    
    #region OnEpisodeBegin
    
    public override void OnEpisodeBegin()
    {
        if (!_goalSpawner.HasGoals)
            _goalSpawner.SpawnRandomNumberOfGoals();
        
        _targetTransform = _goalSpawner.GetClosestGoal(transform.position);
    }
    
    #endregion
    
    #region CollectObservations / OnActionReceived / Heuristic

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
    
    #endregion

    #region OnTriggerEnter
    
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case TagsNames.GoalTagName:
                _goalSpawner.DestroyGoal(other.transform);
                SetReward(+1.0f);
                EndEpisode();
                break;
            case TagsNames.BorderTagName:
                transform.localPosition = Vector3.zero;
                SetReward(-1.0f);
                EndEpisode();
                break;
        }
    }
    
    #endregion
    
    #region OnDrawGizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        if (Application.isPlaying)
        {
            Gizmos.DrawLine(transform.position, _targetTransform.position);
        }
    }
    
    #endregion
}