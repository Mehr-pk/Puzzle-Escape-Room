using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class LibraryDoor : MonoBehaviour, IInteractable
{
    [Header("Door Rotation")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private bool openAutomaticallyWhenUnlocked = false;

    [Header("UI")]
    [SerializeField] private Text doorMessageText;
    [SerializeField] private float messageDuration = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lockedClip;
    [SerializeField] private AudioClip unlockClip;
    [SerializeField] private AudioClip openClip;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isUnlocked;
    private bool isOpen;

    private void Awake()
    {
        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        closedRotation = doorPivot.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    public string GetPrompt()
    {
        if (isOpen)
        {
            return "Door is open";
        }

        if (isUnlocked)
        {
            return "Press E to open library door";
        }

        return "Press E to check locked door";
    }

    public void Interact(GameObject player)
    {
        if (isOpen) return;

        if (!isUnlocked)
        {
            ShowMessage("Door is locked. Find both library clues first.");
            PlaySound(lockedClip);
            return;
        }

        OpenDoor();
    }

    public void UnlockDoor()
    {
        if (isUnlocked) return;

        isUnlocked = true;

        Debug.Log("Library door unlocked.");

        ShowMessage("Library door unlocked.");
        PlaySound(unlockClip);

        if (openAutomaticallyWhenUnlocked)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        Debug.Log("Library door opened.");

        ShowMessage("Door opened.");
        PlaySound(openClip);

        StopAllCoroutines();
        StartCoroutine(RotateDoor(openRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(doorPivot.localRotation, targetRotation) > 0.2f)
        {
            doorPivot.localRotation = Quaternion.Slerp(
                doorPivot.localRotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        doorPivot.localRotation = targetRotation;
    }

    private void ShowMessage(string message)
    {
        if (doorMessageText == null) return;

        doorMessageText.gameObject.SetActive(true);
        doorMessageText.text = message;

        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), messageDuration);
    }

    private void ClearMessage()
    {
        if (doorMessageText == null) return;

        doorMessageText.text = "";
        doorMessageText.gameObject.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.PlayOneShot(clip);
    }
}
