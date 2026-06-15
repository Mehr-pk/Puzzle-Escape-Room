using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClueItem : MonoBehaviour, IInteractable
{
    [Header("Clue Data")]
    [SerializeField] private string clueId;
    [SerializeField] private string clueName;

    [TextArea(2, 5)]
    [SerializeField] private string clueMessage;

    [Header("Manager")]
    [SerializeField] private LibraryClueManager clueManager;

    private bool pickedUp;

    private void Awake()
    {
        if (clueManager == null)
        {
            clueManager = FindFirstObjectByType<LibraryClueManager>();
        }
    }

    public string GetPrompt()
    {
        return "[ E ] Pick up " + clueName;
    }

    public void Interact(GameObject player)
    {
        if (pickedUp) return;

        if (clueManager == null)
        {
            Debug.LogError("LibraryClueManager is missing. Assign LibraryPuzzleManager in the Clue Manager field.");
            return;
        }

        pickedUp = true;

        Debug.Log("Picked clue: " + clueName);

        clueManager.CollectClue(clueId, clueName, clueMessage);

        Collider clueCollider = GetComponent<Collider>();

        if (clueCollider != null)
        {
            clueCollider.enabled = false;
        }
    }
}
