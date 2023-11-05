using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToGoalAgent : Agent {

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMesh;
    [SerializeField] private Transform obstacleTransform; // Reference to the obstacle's transform
    private const float ENVIRONMENT_HALF_SIZE = 10f; // Half of 20x20 environment

    private const float STAY_RADIUS = 2f;
    private const float CHECK_INTERVAL = 0.5f; // Interval to check agent's position
    private const float MAX_STAY_DURATION = 1; // Seconds

    private Queue<Vector3> recentPositions = new Queue<Vector3>();
    private float timeInRadius = 0f;
    private float lastCheckTime = 0f;

    private float previousDistanceToGoal;

    public override void OnEpisodeBegin()
    {
        AdjustObstacle(); // Adjust the obstacle's position and size
        PositionAgent();
        PositionGoal();

        previousDistanceToGoal = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        recentPositions.Clear();
        timeInRadius = 0f;
        lastCheckTime = Time.time;
    }

    private void AdjustObstacle()
    {
        bool isVertical = Random.Range(0, 2) == 0;
        float obstacleLength = Random.Range(0f, 10f);
        Vector3 obstacleSize = isVertical ? new Vector3(1, 1, obstacleLength) : new Vector3(obstacleLength, 1, 1);

        obstacleTransform.localScale = obstacleSize;

        obstacleTransform.localPosition = new Vector3(Random.Range(-8f + 2, 8f - 2), 0.5f, Random.Range(-8f + 2, 8f - 2));
    }


    private void PositionAgent()
    {
        do
        {
            transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
        }
        while (IsTooCloseToObstacle());
    }

    private bool IsTooCloseToObstacle()
    {
        float obstacleHalfWidth = obstacleTransform.transform.localScale.x / 2;
        float obstacleHalfLength = obstacleTransform.transform.localScale.z / 2;
        float distanceToObstacle = Vector3.Distance(transform.localPosition, obstacleTransform.transform.localPosition);

        return distanceToObstacle < Mathf.Max(obstacleHalfWidth, obstacleHalfLength) + 2;
    }

    private void PositionGoal()
    {
        Vector3 directionToObstacle = (obstacleTransform.transform.localPosition - transform.localPosition).normalized;
        Vector3 goalPositionBeyondObstacle = obstacleTransform.transform.localPosition + directionToObstacle * ((obstacleTransform.transform.localScale.z / 2) + 3); // Added +1 to make sure they don't touch

        // Clamp the goal's position to ensure it's within the environment boundaries and at least 1 unit away
        goalPositionBeyondObstacle.x = Mathf.Clamp(goalPositionBeyondObstacle.x, -ENVIRONMENT_HALF_SIZE + 1, ENVIRONMENT_HALF_SIZE - 1);
        goalPositionBeyondObstacle.z = Mathf.Clamp(goalPositionBeyondObstacle.z, -ENVIRONMENT_HALF_SIZE + 1, ENVIRONMENT_HALF_SIZE - 1);

        targetTransform.localPosition = goalPositionBeyondObstacle;

        // Ensure minimum distance between agent and goal
        while (Vector3.Distance(transform.localPosition, targetTransform.localPosition) < 1)
        {
            PositionAgent();
        }
    }



    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(obstacleTransform.localPosition);
        sensor.AddObservation(obstacleTransform.localScale.x);
        sensor.AddObservation(obstacleTransform.localScale.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 6f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        float currentDistanceToGoal = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        if (currentDistanceToGoal < previousDistanceToGoal)
        {
            // Increment the reward for getting closer to the target
            AddReward(0.1f);
        }
        else if (currentDistanceToGoal > previousDistanceToGoal)
        {
            // Optionally: Decrement the reward for moving away from the target
            // This might encourage the agent to always move towards the target
            AddReward(-0.2f);
        }

        if (Time.time - lastCheckTime >= CHECK_INTERVAL)
        {
            CheckStayDuration();
            lastCheckTime = Time.time;
        }

        previousDistanceToGoal = currentDistanceToGoal;
    }

    private void CheckStayDuration()
    {
        recentPositions.Enqueue(transform.localPosition);

        // Remove oldest position if we exceed the number of checks within MAX_STAY_DURATION
        if (recentPositions.Count > (MAX_STAY_DURATION / CHECK_INTERVAL))
        {
            Vector3 oldestPosition = recentPositions.Dequeue();

            if (Vector3.Distance(oldestPosition, transform.localPosition) < STAY_RADIUS)
            {
                timeInRadius += CHECK_INTERVAL;

                if (timeInRadius >= MAX_STAY_DURATION)
                {
                    //AddReward(-1f); // Apply a negative reward
                    timeInRadius = 0f; // Reset time in radius
                }
            }
            else
            {
                timeInRadius = 0f; // Reset time in radius
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(200f);
            floorMesh.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-50f);
            floorMesh.material = loseMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Obstacle>(out Obstacle obstacle))
        {
            SetReward(-50f);
            floorMesh.material = loseMaterial;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }
}
