using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Basic", menuName = "Scriptable/Spawns", order = 1)]

public class SpawnScript : ScriptableObject
{
    public bool isSpawn;
    public bool isReverse; // INDICA DIREÇÃO DO TRANSITO
    public bool isTrain;
    public bool isDouble;
    public int minSpeed;
    public int maxSpeed;

    public GameObject[] prefabs;

}
