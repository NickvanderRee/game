using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    public int gridWidth = 2;
    public int gridHeight = 2;
    public float wallHeight = 3f;
    public int roomSize = 5;
    public int mazeWidth = 10;  // Default size
    public int mazeHeight = 10; // Default size
    public float obstacleRemovalProbability = 0.3f; // Probability of removing an obstacle

    public NavMeshSurface navMeshSurface;
    public Material mazeMaterial;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject keyPrefab;
    public GameObject exitPrefab;

    private int[,] maze;
    private Vector2Int keyPosition;
    private Vector2Int playerPosition;
    private Vector2Int enemyPosition;
    private Vector2Int exitPosition;

    List<Vector2Int> visitedCells = new List<Vector2Int>();

    void Start()
    {
        maze = new int[mazeWidth, mazeHeight];
        GenerateMaze();
        BuildMaze();
        BuildNavMesh();
        PlacePlayer();
        PlaceKey();
        PlaceExit();
        PlaceEnemy();
    }

    void GenerateMaze()
    {
        Debug.Log("Generating maze...");

        // Check if mazeWidth and mazeHeight are valid
        if (mazeWidth <= 0 || mazeHeight <= 0)
        {
            Debug.LogError("Invalid maze dimensions. Please ensure mazeWidth and mazeHeight are greater than 0.");
            return;
        }

        // Initialize maze with walls
        maze = new int[mazeHeight, mazeWidth]; // Ensure correct dimensions
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                if (y == 0 || y == mazeHeight - 1 || x == 0 || x == mazeWidth - 1)
                {
                    maze[y, x] = 1; // Place walls on the perimeter
                }
                else
                {
                    maze[y, x] = 0; // Initialize inner cells as empty
                }
            }
        }

        // Generate rooms
        for (int y = 1; y < mazeHeight - 1; y++)
        {
            for (int x = 1; x < mazeWidth - 1; x++)
            {
                if (Random.value < 0.2f) // Adjust probability as needed
                {
                    GenerateRoom(x, y);
                }
            }
        }

        // Randomly remove obstacles
        for (int y = 1; y < mazeHeight - 1; y++)
        {
            for (int x = 1; x < mazeWidth - 1; x++)
            {
                if (Random.value < obstacleRemovalProbability)
                {
                    maze[y, x] = 0;
                }
            }
        }

        // Ensure connectivity
        for (int y = 1; y < mazeHeight - 1; y++)
        {
            for (int x = 1; x < mazeWidth - 1; x++)
            {
                if (maze[y, x] == 0 && IsSurroundedByWalls(x, y))
                {
                    maze[y, x] = 1;
                }
            }
        }

        Debug.Log("Maze generation complete.");
    }

    void GenerateRoom(int startX, int startY)
    {
        int halfRoomSize = roomSize / 2;

        for (int y = startY - halfRoomSize; y <= startY + halfRoomSize && y < mazeHeight - 1; y++)
        {
            for (int x = startX - halfRoomSize; x <= startX + halfRoomSize && x < mazeWidth - 1; x++)
            {
                if (x > 0 && x < mazeWidth && y > 0 && y < mazeHeight)
                {
                    maze[y, x] = 1;
                }
            }
        }
    }

    bool IsSurroundedByWalls(int x, int y)
    {
        return maze[y - 1, x] == 1 && maze[y + 1, x] == 1 && maze[y, x - 1] == 1 && maze[y, x + 1] == 1;
    }

    void PlaceKey()
    {
        keyPosition = FindValidPosition();
        if (keyPosition == Vector2Int.zero)
        {
            Debug.LogError("Unable to find suitable key position.");
            return;
        }

        visitedCells.Add(keyPosition);
        Debug.Log("Key Position: " + keyPosition);
        GameObject key = Instantiate(keyPrefab, new Vector3(keyPosition.x * gridWidth, wallHeight / wallHeight, keyPosition.y * gridHeight), Quaternion.identity);
        key.tag = "Key";
    }

    void PlaceExit()
    {
        exitPosition = FindValidPosition();
        if (exitPosition == Vector2Int.zero)
        {
            Debug.LogError("Unable to find suitable exit position.");
            return;
        }

        visitedCells.Add(exitPosition);
        Debug.Log("Exit Position: " + exitPosition);
        GameObject exit = Instantiate(exitPrefab, new Vector3(exitPosition.x * gridWidth, wallHeight / wallHeight, exitPosition.y * gridHeight), Quaternion.identity);
        exit.tag = "Exit";
    }

    void PlacePlayer()
    {
        playerPosition = FindValidPosition();
        if (playerPosition == Vector2Int.zero)
        {
            Debug.LogError("Unable to find suitable player position.");
            return;
        }

        Debug.Log("Player Position: " + playerPosition);
        GameObject player = Instantiate(playerPrefab, new Vector3(playerPosition.x * gridWidth, 1, playerPosition.y * gridHeight), Quaternion.identity);
        player.transform.localScale = new Vector3(1, 1, 1);
        player.tag = "Player";
    }

    void PlaceEnemy()
    {
        enemyPosition = FindValidPosition();
        if (enemyPosition == Vector2Int.zero)
        {
            Debug.LogWarning("Unable to find suitable enemy position.");
            return;
        }

        visitedCells.Add(enemyPosition);
        Debug.Log("Enemy Position: " + enemyPosition);
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(enemyPosition.x * gridWidth, 0.5f, enemyPosition.y * gridHeight), Quaternion.identity);
        enemy.transform.localScale = new Vector3(1, 1, 1);
    }

    Vector2Int FindValidPosition()
    {
        List<Vector2Int> potentialPositions = new List<Vector2Int>();

        for (int y = 1; y < mazeHeight - 1; y++)
        {
            for (int x = 1; x < mazeWidth - 1; x++)
            {
                if (maze[y, x] == 0 && !visitedCells.Contains(new Vector2Int(x, y)))
                {
                    potentialPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (potentialPositions.Count == 0)
        {
            Debug.LogWarning("Unable to find suitable position within maximum attempts.");
            return Vector2Int.zero;
        }

        int randomIndex = Random.Range(0, potentialPositions.Count);
        return potentialPositions[randomIndex];
    }

    void BuildMaze()
    {
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                if (maze[y, x] == 1)
                {
                    CreateWall(new Vector3(x * gridWidth, wallHeight / 2, y * gridHeight));
                }
                else
                {
                    CreateFloor(new Vector3(x * gridWidth, 0, y * gridHeight));
                }
            }
        }
    }

    void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh built"); // Add this line to verify NavMesh is built
    }

    void CreateWall(Vector3 position)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = new Vector3(gridWidth, wallHeight, gridHeight);
        wall.layer = LayerMask.NameToLayer("Obstacle");
        NavMeshObstacle navMeshObstacle = wall.AddComponent<NavMeshObstacle>();
        navMeshObstacle.carving = true;

        // Assign MazeMaterial to the wall GameObject
        if (mazeMaterial != null)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            renderer.material = mazeMaterial;
        }
    }

    void CreateFloor(Vector3 position)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.position = position;
        floor.transform.localScale = new Vector3(gridWidth, 0.1f, gridHeight);
        floor.layer = LayerMask.NameToLayer("Floor");

        // Assign MazeMaterial to the floor GameObject
        if (mazeMaterial != null)
        {
            Renderer renderer = floor.GetComponent<Renderer>();
            renderer.material = mazeMaterial;
        }
    }
}