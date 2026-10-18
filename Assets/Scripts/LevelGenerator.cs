using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private LevelData[] levels;
    [SerializeField] private int currentLevel = 0;

    [Header("Prefabs")]
    [SerializeField] private GameObject stablePrefab;
    [SerializeField] private GameObject specialPrefab;
    [SerializeField] private GameObject[] rowPrefabs;

    [Header("Referenzen")]
    [SerializeField] private GameManager gameManager;

    private void OnEnable()
    {
        if (gameManager != null) gameManager.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        if (gameManager != null) gameManager.StateChanged -= OnStateChanged;
    }

    private void Start()
    {
        Generate(currentLevel);
    }

    private void OnStateChanged(GameState state)
    {
        if (state != GameState.LevelComplete) return;

        if (currentLevel + 1 >= levels.Length)
        {
            Debug.Log("Alle Level geschafft");
            return;
        }

        currentLevel++;
        Generate(currentLevel);
        gameManager.ResumePlaying();
    }

    private void Generate(int levelIndex)
    {
        if (levels == null || levelIndex < 0 || levelIndex >= levels.Length) return;
        if (stablePrefab == null) return;
        if (rowPrefabs == null || rowPrefabs.Length == 0) return;

        LevelData data = levels[levelIndex];
        float startX = -(data.columns - 1) * data.spacingX / 2f;

        for (int row = 0; row < data.rows; row++)
        {
            for (int col = 0; col < data.columns; col++)
            {
                GameObject prefab;

                if (IsSpecial(data, row, col) && specialPrefab != null)
                {
                    prefab = specialPrefab;
                }
                else if (row < data.stableRows)
                {
                    prefab = stablePrefab;
                }
                else
                {
                    prefab = rowPrefabs[(row - data.stableRows) % rowPrefabs.Length];
                }

                float x = startX + col * data.spacingX;
                float z = data.startZ - row * data.spacingZ;
                float y = prefab.transform.position.y;

                Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
            }
        }
    }

    private bool IsSpecial(LevelData data, int row, int col)
    {
        if (data.specialPositions == null) return false;

        for (int i = 0; i < data.specialPositions.Length; i++)
        {
            if (data.specialPositions[i].x == row && data.specialPositions[i].y == col)
            {
                return true;
            }
        }

        return false;
    }
}