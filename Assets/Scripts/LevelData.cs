using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Arkanoid/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Raster")]
    public int rows = 5;
    public int columns = 8;
    public float spacingX = 1.8f;
    public float spacingZ = 1.1f;
    public float startZ = 6.5f;

    [Header("Inhalt")]
    [Tooltip("Wie viele der hintersten Zeilen aus stabilen Blöcken bestehen")]
    public int stableRows = 1;
    [Tooltip("X = Zeile, Y = Spalte")]
    public Vector2Int[] specialPositions;
}