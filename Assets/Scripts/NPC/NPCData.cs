using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "NPC/NPC Data", order = 100)]
public class NPCData : ScriptableObject
{
    [Header("Basic Identity")]
    public string npcName = "Unnamed Enemy";
    [TextArea(3, 10)] // Makes the text box bigger in the Inspector
    public string description = "";

    [Header("Visuals & Prefabs")]
    // The specific prefab or model for this character
    public GameObject characterGameObject;
    public GameObject deathEffectPrefab;

    [Header("Core Stats")]
    [Range(10f, 1000f)]
    public float maxHealth = 100f;

    [Range(0.5f, 15f)]
    public float moveSpeed = 2.5f;
}