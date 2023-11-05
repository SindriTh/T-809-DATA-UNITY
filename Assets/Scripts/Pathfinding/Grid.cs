using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Grid
{
    public Node[,] grid;
    public int gridSizeX, gridSizeZ;
    public float nodeDiameter;
    public float nodeRadius;
    public Vector3 gridWorldSize;
    public LayerMask unwalkableMask;
    private Transform obstacleTransform; 


    public Grid(Vector3 worldSize, float nodeRadius, LayerMask unwalkableMask, Transform obstacleTransform)
    {
        this.nodeRadius = nodeRadius;
        this.nodeDiameter = nodeRadius * 2;
        this.gridWorldSize = worldSize;
        this.unwalkableMask = unwalkableMask;
        gridSizeX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(worldSize.z / nodeDiameter);
        this.obstacleTransform = obstacleTransform;
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeZ];
        Vector3 worldBottomLeft = new Vector3(-gridWorldSize.x / 2, 0, -gridWorldSize.z / 2);

        // Get obstacle boundaries
        Vector3 obstacleMin = obstacleTransform.localPosition - (obstacleTransform.localScale / 2);
        Vector3 obstacleMax = obstacleTransform.localPosition + (obstacleTransform.localScale / 2);

        // Add buffer to obstacle boundaries to keep at least one unit away
        float buffer = 1f; // The buffer distance
        obstacleMin -= new Vector3(buffer, 0, buffer);
        obstacleMax += new Vector3(buffer, 0, buffer);

        for (int x = 0; x < gridSizeX; x++)
        { 
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                bool walkable = true;

                // Check if this node is within the obstacle boundaries (with buffer)
                if (worldPoint.x >= obstacleMin.x && worldPoint.x <= obstacleMax.x &&
                    worldPoint.z >= obstacleMin.z && worldPoint.z <= obstacleMax.z)
                {
                    walkable = false;
                }

                grid[x, z] = new Node(walkable, worldPoint, x, z);
            }
        }
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        return grid[x, z];
    }

    // Additional methods to get the neighbors of a node, draw the grid, etc.
}
