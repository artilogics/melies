// CT_IntroVideo : Intro cinematic that plays at the start of the game scene.
// Blocks player input and movement, plays audio + optional video, skippable with Space.
// Add this to a GameObject in the 01_Default scene. See setup instructions below.
//
// SETUP IN UNITY:
//   1. Create empty GameObject "IntroVideoManager" in 01_Default scene
//   2. Add this script to it
//   3. Add an AudioSource component -> assign Assets/AP/Assets/Audio/Ambiance/Intro_MusicaFinal.wav
//   4. Create a Canvas (Canvas_Intro) with Sort Order 10:
//      - Add CanvasGroup component to the Canvas root
//      - Child: Image (black, stretch full screen) -> Source Image = None -> Color = black
//      - Child: Text "Prem espai per saltar" -> Anchor bottom-left, offset (20, 20), font white
//      - Child (optional): RawImage for the video
//   5. Wire references in Inspector:
//      - Canvas Intro   -> Canvas_Intro GameObject
//      - Canvas Grp     -> CanvasGroup on Canvas_Intro
//      - Txt Skip       -> the Text component
//      - Intro Audio    -> AudioSource on this GameObject
//   6. WHEN YOU HAVE THE VIDEO:
//      - Add VideoPlayer component to this GameObject
//      - Assign the clip
//      - Set target texture (RawImage)
//      - Drag it to "Video Player" field

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CT_IntroVideo : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasIntro;
    public CanvasGroup canvasGrp;

    [Header("Skip Text")]
    public Text txtSkip;

    [Header("Audio")]
    public AudioSource introAudioSource;

    [Header("Video (optional - assign when clip is ready)")]
    public VideoPlayer videoPlayer;

    [Header("Settings")]
    public float fadeOutDuration = 1f;

    void Start()
    {
        if (canvasIntro) canvasIntro.SetActive(false);
        StartCoroutine(I_PlayIntro());
    }

    IEnumerator I_PlayIntro()
    {
        yield return new WaitUntil(() =>
            ingameGlobalManager.instance != null &&
            ingameGlobalManager.instance.initScene);

        if (canvasIntro) canvasIntro.SetActive(true);
        if (canvasGrp)
        {
            canvasGrp.alpha = 1f;
            canvasGrp.interactable = true;
            canvasGrp.blocksRaycasts = true;
        }

        ingameGlobalManager.instance.b_InputIsActivated = false;
        ingameGlobalManager.instance.b_bodyMovement = false;

        if (!ingameGlobalManager.instance.b_DesktopInputs && ingameGlobalManager.instance.canvasMobileInputs)
            ingameGlobalManager.instance.canvasMobileInputs.SetActive(false);

        if (ingameGlobalManager.instance.reticule && ingameGlobalManager.instance.b_DesktopInputs)
            ingameGlobalManager.instance.reticule.SetActive(false);

        if (txtSkip) txtSkip.gameObject.SetActive(true);

        // Silenciar todos los sonidos del juego excepto el audio del intro
        if (introAudioSource != null)
            introAudioSource.ignoreListenerVolume = true;
        AudioListener.volume = 0f;

        if (videoPlayer != null && videoPlayer.clip != null)
            videoPlayer.Play();

        if (introAudioSource != null && introAudioSource.clip != null)
            introAudioSource.Play();

        // Wait two frames so isPlaying reflects the actual playback state
        yield return null;
        yield return null;

        bool hasMedia = (introAudioSource != null && introAudioSource.clip != null) ||
                        (videoPlayer != null && videoPlayer.clip != null);

        while (true)
        {
            if (!ingameGlobalManager.instance.b_Ingame_Pause)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                    break;

                if (hasMedia)
                {
                    bool audioDone = introAudioSource == null || introAudioSource.clip == null || !introAudioSource.isPlaying;
                    bool videoDone = videoPlayer == null || videoPlayer.clip == null || !videoPlayer.isPlaying;
                    if (audioDone && videoDone) break;
                }
            }
            yield return null;
        }

        if (introAudioSource != null && introAudioSource.isPlaying)
            introAudioSource.Stop();
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        // Restaurar el volumen global
        AudioListener.volume = 1f;
        if (introAudioSource != null)
            introAudioSource.ignoreListenerVolume = false;

        if (txtSkip) txtSkip.gameObject.SetActive(false);

        float t = 1f;
        while (t > 0f)
        {
            if (!ingameGlobalManager.instance.b_Ingame_Pause)
            {
                t = Mathf.MoveTowards(t, 0f, Time.deltaTime / fadeOutDuration);
                if (canvasGrp) canvasGrp.alpha = t;
            }
            yield return null;
        }

        if (canvasGrp)
        {
            canvasGrp.interactable = false;
            canvasGrp.blocksRaycasts = false;
        }
        if (canvasIntro) canvasIntro.SetActive(false);

        ingameGlobalManager.instance.b_InputIsActivated = true;
        ingameGlobalManager.instance.b_bodyMovement = true;

        if (ingameGlobalManager.instance.reticule && ingameGlobalManager.instance.b_DesktopInputs)
            ingameGlobalManager.instance.reticule.SetActive(true);

        if (!ingameGlobalManager.instance.b_DesktopInputs && ingameGlobalManager.instance.canvasMobileInputs)
            ingameGlobalManager.instance.canvasMobileInputs.SetActive(true);
    }
}
