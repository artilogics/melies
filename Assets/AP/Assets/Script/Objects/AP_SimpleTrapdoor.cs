using System.Collections;
using UnityEngine;

public class AP_SimpleTrapdoor : MonoBehaviour
{
    public bool isOpened = false;
    public Vector3 closedRotation = Vector3.zero;
    public Vector3 openedRotation = new Vector3(-90, 0, 0);
    public float speed = 3f;

    public AudioClip openSound;
    public AudioClip closeSound;
    public float volume = 1f;
    [Header("Item Requirement")]
    public bool requiresItem = false;
    public int requiredItemID = 0;
    public int feedbackID = 0;
    public bool deleteItemAfterUse = false;

    private AudioSource audioSource;
    private Coroutine movementCoroutine;

    private Quaternion baseRotation;
    private bool initialized = false;

    void Awake()
    {
        InitializeBase();
    }

    void InitializeBase()
    {
        if (initialized) return;
        baseRotation = transform.localRotation;
        initialized = true;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        InitializeBase();

        // Set initial state relative to baseRotation
        transform.localRotation = baseRotation * Quaternion.Euler(isOpened ? openedRotation : closedRotation);
    }

    public void MoveObject()
    {
        if (requiresItem && !isOpened)
        {
            ingameGlobalManager gManager = ingameGlobalManager.instance;
            int itemIndex = gManager.currentPlayerInventoryList.IndexOf(requiredItemID);

            if (itemIndex == -1)
            {
                // Display feedback if item is missing
                if (gManager.canvasPlayerInfos._infoUI)
                {
                    gManager.canvasPlayerInfos._infoUI.playAnimInfo(
                        gManager.currentFeedback.diaryList[gManager.currentLanguage]._languageSlot[feedbackID].diaryTitle[0], 
                        "Feedback", 
                        gameObject);
                }
                return; // Prevent opening
            }
            
            // Item exists, optional deletion
            if (deleteItemAfterUse)
            {
                gManager.currentPlayerInventoryList.RemoveAt(itemIndex);
                gManager.currentPlayerInventoryObjectVisibleList.RemoveAt(itemIndex);
            }
        }

        isOpened = !isOpened;
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(AnimateRotation());

        // Play sound
        AudioSource targetAudio = audioSource;
        if (targetAudio == null) targetAudio = GetComponentInParent<AudioSource>();

        AudioClip clipToPlay = isOpened ? openSound : closeSound;
        if (clipToPlay != null && targetAudio != null)
        {
            targetAudio.PlayOneShot(clipToPlay, volume);
        }
    }

    IEnumerator AnimateRotation()
    {
        InitializeBase();
        Quaternion targetRotation = baseRotation * Quaternion.Euler(isOpened ? openedRotation : closedRotation);
        Quaternion startRotation = transform.localRotation;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            // Use Slerp for smooth rotation
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.localRotation = targetRotation;
    }

    // --> Save / Load System Compatibility
    public string ReturnSaveData()
    {
        return isOpened.ToString();
    }

    public void saveSystemInitGameObject(string s_Value)
    {
        if (s_Value == "True") isOpened = true;
        else if (s_Value == "False") isOpened = false;

        InitializeBase();
        transform.localRotation = baseRotation * Quaternion.Euler(isOpened ? openedRotation : closedRotation);
    }
}
