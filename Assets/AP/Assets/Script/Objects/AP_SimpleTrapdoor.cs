using System.Collections;
using System.Collections.Generic;
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

    [System.Serializable]
    public class idList
    {
        public int ID = 0;          // entry ID in the window tab
        public int uniqueID = 0;    // entry Unique ID
    }

    [Header("Inventory Requirement")]
    public bool requireItemToOpen = false;
    public int requiredItemID = 0;

    [Header("Feedback when Locked")]
    public bool b_feedbackActivated = false;
    public List<idList> feedbackIDList = new List<idList>() { new idList() };
    public AudioClip lockedSound;
    public float lockedVolume = 1f;

    private infoUI info;

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

        GameObject tmpObj = GameObject.Find("UI_Infos");
        if (tmpObj) info = tmpObj.GetComponent<infoUI>();

        InitializeBase();

        // Set initial state relative to baseRotation
        transform.localRotation = baseRotation * Quaternion.Euler(isOpened ? openedRotation : closedRotation);
    }

    public void MoveObject()
    {
        if (!isOpened && requireItemToOpen)
        {
            bool hasItem = false;
            if (ingameGlobalManager.instance != null && ingameGlobalManager.instance.currentPlayerInventoryList != null)
            {
                if (ingameGlobalManager.instance.currentPlayerInventoryList.Contains(requiredItemID))
                {
                    hasItem = true;
                }
            }

            if (!hasItem)
            {
                // Play locked sound
                if (lockedSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(lockedSound, lockedVolume);
                }

                // Display feedback
                if (info && b_feedbackActivated && feedbackIDList.Count > 0)
                {
                    bool b_Exist = false;
                    for (var i = 0; i < info.listRefGameObject.Count; i++)
                    {
                        if (gameObject == info.listRefGameObject[i])
                            b_Exist = true;
                    }
                    if (!b_Exist && ingameGlobalManager.instance != null && ingameGlobalManager.instance.currentFeedback != null)
                    {
                        string feedbackText = ingameGlobalManager.instance.currentFeedback.diaryList[ingameGlobalManager.instance.currentLanguage]._languageSlot[feedbackIDList[0].ID].diaryTitle[0];
                        info.playAnimInfo(feedbackText, "Feedback", gameObject);
                    }
                }

                return;
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
