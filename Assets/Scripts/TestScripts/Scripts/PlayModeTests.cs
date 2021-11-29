using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Random = System.Random;

public class PlayModeTests
{
    [UnityTest]
    public IEnumerator InstantiatesRandomNumberOfGoals()
    {
        var goalPrefab = Resources.Load<GameObject>("Prefabs/Goal");
        var goalGameObject = new GameObject();
        var goalSpawner = goalGameObject.AddComponent<GoalSpawner>();
        var goalsToSpawn = new Vector2Int(5, 10);

        var expectedNumberOfGoals = new Random().Next(goalsToSpawn.x, goalsToSpawn.y);
        var random = Substitute.For<Random>();
        random.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(expectedNumberOfGoals);
        goalSpawner.Random = random;
        
        goalSpawner.UnitTestInitialize(goalPrefab);
        
        yield return null;

        var spawnedGoal = GameObject.FindGameObjectsWithTag(TagsNames.GoalTagName);
        Assert.AreEqual(expectedNumberOfGoals, spawnedGoal.Length);
    }

    [UnityTest]
    public IEnumerator IsAgentTargetingClosestGoal()
    {
        var platformPrefab = Resources.Load<GameObject>("Prefabs/Platform");
        Object.Instantiate(platformPrefab);
        var moveToGoalAgent = Object.FindObjectOfType<MoveToGoalAgent>();
        var goalSpawner = Object.FindObjectOfType<GoalSpawner>();

        yield return null;

        var closestGoal = goalSpawner.GetClosestGoal(moveToGoalAgent.transform.position);
        Assert.AreEqual(closestGoal, moveToGoalAgent.TargetTransform);
    }
}
