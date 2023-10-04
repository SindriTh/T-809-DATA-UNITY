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
    private float previousDistanceToGoal;

    public override void OnEpisodeBegin()
    {
        targetTransform.localPosition = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
        transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9f, 9f));
        previousDistanceToGoal = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        while (previousDistanceToGoal < 1)
        {
            transform.localPosition = new Vector3(Random.Range(-9, 9), 0, Random.Range(-9, 9));
            previousDistanceToGoal = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
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
            //AddReward(0.1f);
        }
        else if (currentDistanceToGoal > previousDistanceToGoal)
        {
            // Optionally: Decrement the reward for moving away from the target
            // This might encourage the agent to always move towards the target
            AddReward(-0.05f);
        }

        previousDistanceToGoal = currentDistanceToGoal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(50f);
            floorMesh.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
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
