using UnityEngine;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 7f;
    [SerializeField] private LayerMask interactableLayer = ~0;
    [SerializeField] private bool onlyShowWhenInFront = true;

    [Header("Prompt UI")]
    [SerializeField] private RectTransform promptPanel;
    [SerializeField] private TMP_Text promptText;

    [Header("Prompt Position")]
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 60f);

    private IInteractable currentInteractable;
    private MonoBehaviour currentInteractableObject;
    private Collider currentTargetCollider;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        AutoFindPromptUI();
        HidePrompt();
    }

    private void Start()
    {
        AutoFindPromptUI();
        HidePrompt();
    }

    private void Update()
    {
        FindNearbyInteractable();
        UpdatePromptPosition();

        if (InteractPressed() && currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
            HidePrompt();
        }
    }

    private void AutoFindPromptUI()
    {
        if (promptPanel == null)
        {
            RectTransform[] rects = FindObjectsByType<RectTransform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (RectTransform rect in rects)
            {
                if (rect.name == "InteractionPromptPanel")
                {
                    promptPanel = rect;
                    break;
                }
            }
        }

        if (promptText == null)
        {
            TMP_Text[] texts = FindObjectsByType<TMP_Text>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (TMP_Text text in texts)
            {
                if (text.name == "InteractionPromptText")
                {
                    promptText = text;
                    break;
                }
            }
        }
    }

    private void FindNearbyInteractable()
    {
        currentInteractable = null;
        currentInteractableObject = null;
        currentTargetCollider = null;

        if (playerCamera == null)
        {
            HidePrompt();
            return;
        }

        Collider[] hits = Physics.OverlapSphere(
            playerCamera.transform.position,
            interactionDistance,
            interactableLayer,
            QueryTriggerInteraction.Collide
        );

        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();

            if (interactable == null)
            {
                continue;
            }

            MonoBehaviour interactableObject = interactable as MonoBehaviour;

            if (interactableObject == null)
            {
                continue;
            }

            Vector3 targetPosition = hit.bounds.center;

            if (onlyShowWhenInFront)
            {
                Vector3 directionToTarget = targetPosition - playerCamera.transform.position;
                float dot = Vector3.Dot(playerCamera.transform.forward, directionToTarget.normalized);

                if (dot < 0.35f)
                {
                    continue;
                }
            }

            float distance = Vector3.Distance(playerCamera.transform.position, targetPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
                currentInteractableObject = interactableObject;
                currentTargetCollider = hit;
            }
        }

        if (currentInteractable != null)
        {
            ShowPrompt(currentInteractable.GetPrompt());
        }
        else
        {
            HidePrompt();
        }
    }

    private void UpdatePromptPosition()
    {
        if (promptPanel == null || currentTargetCollider == null || playerCamera == null)
        {
            return;
        }

        Vector3 worldPosition = currentTargetCollider.bounds.center;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z <= 0f)
        {
            HidePrompt();
            return;
        }

        promptPanel.position = new Vector3(
            screenPosition.x + screenOffset.x,
            screenPosition.y + screenOffset.y,
            0f
        );
    }

    private bool InteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.E))
        {
            return true;
        }
#endif

        return false;
    }

    private void ShowPrompt(string message)
    {
        if (promptPanel == null || promptText == null)
        {
            Debug.LogWarning("Prompt Panel or Prompt Text is not assigned in PlayerInteractor.");
            return;
        }

        promptPanel.gameObject.SetActive(true);
        promptText.text = message;
    }

    private void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.gameObject.SetActive(false);
        }

        if (promptText != null)
        {
            promptText.text = "";
        }
    }
}
