using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryClueManager : MonoBehaviour
{
    [Header("Required Clues")]
    [SerializeField] private string[] requiredClueIds =
    {
        "exam_timetable_note",
        "library_keycard"
    };

    [Header("Door")]
    [SerializeField] private LibraryDoor libraryDoor;

    [Header("UI")]
    [SerializeField] private Text statusText;
    [SerializeField] private float messageDuration = 4f;

    private readonly HashSet<string> collectedClues = new HashSet<string>();

    public void CollectClue(string clueId, string clueName, string clueMessage)
    {
        if (string.IsNullOrEmpty(clueId))
        {
            Debug.LogError("Clue ID is empty. Give every clue a unique clueId.");
            return;
        }

        if (collectedClues.Contains(clueId))
        {
            return;
        }

        collectedClues.Add(clueId);

        Debug.Log("Collected clue: " + clueName + " (" + collectedClues.Count + "/" + requiredClueIds.Length + ")");

        ShowMessage(clueName + " collected.\n" + clueMessage);

        if (HasAllRequiredClues())
        {
            UnlockLibraryDoor();
        }
    }

    private bool HasAllRequiredClues()
    {
        foreach (string requiredId in requiredClueIds)
        {
            if (!collectedClues.Contains(requiredId))
            {
                return false;
            }
        }

        return true;
    }

    private void UnlockLibraryDoor()
{
    if (libraryDoor == null)
    {
        Debug.LogWarning("Both clues collected, but no library door exists yet. Create a door later and assign it in LibraryClueManager.");
        ShowMessage("Both library clues found. Door will unlock after you add a door.");
        return;
    }

    libraryDoor.UnlockDoor();

    ShowMessage("Both library clues found. The library door is now unlocked.");
}
    public bool HasCollectedAllClues()
    {
        return HasAllRequiredClues();
    }

    private void ShowMessage(string message)
    {
        if (statusText == null) return;

        statusText.gameObject.SetActive(true);
        statusText.text = message;

        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), messageDuration);
    }

    private void ClearMessage()
    {
        if (statusText == null) return;

        statusText.text = "";
        statusText.gameObject.SetActive(false);
    }
}
