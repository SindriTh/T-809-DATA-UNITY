using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public Vector3 gridWorldSize = new Vector3(20, 0, 20); // Assuming a 20x20 world size
    public float nodeRadius = 0.125f; // 0.25 diameter for grid spacing
    public LayerMask unwalkableMask; // Set this in the inspector to your unwalkable layers
    [SerializeField] private Transform obstacleTransform; // Reference to the obstacle's transform
    [SerializeField] private Transform agent;
    [SerializeField] private Transform targetTransform;



    private Grid grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid(gridWorldSize, nodeRadius, unwalkableMask, obstacleTransform);
        // Now your grid is populated with nodes, which are walkable or not based on the obstacleTransform
    }

    // Update is called once per frame
    void Update()
    {
        if (grid != null)
        {

            Node startNode = grid.NodeFromWorldPoint(agent.position);
            Node targetNode = grid.NodeFromWorldPoint(targetTransform.position);

            AStarPathfinder pathfinder = new AStarPathfinder(grid.grid); // Assuming the AStarPathfinder accepts a Node[,] as a parameter
            List<Node> path = pathfinder.FindPath(startNode, targetNode);

            if (path != null)
            {
                // Handle the found path, like drawing it or moving an object along it
                DrawPath(path);
            }
        } else
        {
            // print grid is null
            print("grid is null");
            grid = new Grid(gridWorldSize, nodeRadius, unwalkableMask, obstacleTransform);

        }
    }

    void DrawPath(List<Node> path)
    {
        // This is where you would handle the visual representation of your path
        // For example, using line renderers or simply drawing debug lines
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i].worldPosition, path[i + 1].worldPosition, Color.green, 2f);
        }
    }
}