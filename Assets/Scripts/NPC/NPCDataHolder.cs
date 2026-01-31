using UnityEngine;

public class NPCDataHolder : MonoBehaviour
{
    [SerializeField]
    private NPCData data;           // ← drag your NPCMeleeBasic or NPCRangedShooter here

    public NPCData Data => data;    // read-only access
}
