#if UNITY_EDITOR
// CT_IntroVideoSetup : Editor window that creates the full intro video system in one click.
// Open via: Tools > El Somni de Mèliès > Setup Intro Video
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor.SceneManagement;

public class CT_IntroVideoSetup : EditorWindow
{
    private VideoClip _videoClip;
    private AudioClip _audioClip;
    private string    _log  = "";
    private bool      _done = false;

    private const string RT_PATH    = "Assets/Art/IntroVideoRT.renderTexture";
    private const string AUDIO_PATH = "Assets/AP/Assets/Audio/Ambiance/Intro_MusicaFinal.wav";
    private const string WUI_PATH   = "Assets/AP/Assets/Resources/Develope-El Somni de Méliès/TextList/wUI.asset";

    // Translations per language index (0=Català, 1=Español, 2=English)
    private static readonly string[] SKIP_TEXT = {
        "Prem espai per saltar",
        "Pulsa espacio para saltar",
        "Press space to skip"
    };

    [MenuItem("Tools/El Somni de Mèliès/Setup Intro Video")]
    public static void ShowWindow()
    {
        CT_IntroVideoSetup w = GetWindow<CT_IntroVideoSetup>(true, "Setup Intro Video", true);
        w.minSize = new Vector2(380, 340);
        w.maxSize = new Vector2(380, 340);
        w._audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(AUDIO_PATH);
    }

    private void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Intro Video - Configuració automàtica", titleStyle);
        EditorGUILayout.Space(4);
        DrawSeparator();

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("1. Arxius", EditorStyles.boldLabel);

        _audioClip = (AudioClip)EditorGUILayout.ObjectField(
            "  Clip d'àudio", _audioClip, typeof(AudioClip), false);
        _videoClip = (VideoClip)EditorGUILayout.ObjectField(
            "  Clip de vídeo", _videoClip, typeof(VideoClip), false);

        if (_videoClip == null)
            EditorGUILayout.HelpBox(
                "Pots deixar el vídeo buit (pantalla negra).\n" +
                "Quan el tinguis, arrossega'l al VideoPlayer de l'Inspector.",
                MessageType.Info);

        EditorGUILayout.Space(8);
        DrawSeparator();
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("2. Crear", EditorStyles.boldLabel);

        GUI.backgroundColor = _done ? new Color(0.4f, 0.8f, 0.4f) : new Color(0.4f, 0.7f, 1f);
        if (GUILayout.Button(_done ? "✓  Tot configurat!" : "  Configurar-ho tot  ", GUILayout.Height(36)))
            if (!_done) RunSetup();
        GUI.backgroundColor = Color.white;

        if (_log != "")
        {
            EditorGUILayout.Space(6);
            DrawSeparator();
            EditorGUILayout.Space(4);
            GUIStyle logStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
            EditorGUILayout.LabelField(_log, logStyle, GUILayout.ExpandHeight(true));
        }
    }

    private void DrawSeparator()
    {
        Rect r = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(r, new Color(0.5f, 0.5f, 0.5f, 0.5f));
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void RunSetup()
    {
        _log  = "";
        _done = false;

        if (GameObject.Find("IntroVideoManager") != null)
        {
            _log = "<color=orange>⚠ Ja existeix un 'IntroVideoManager' a l'escena.\n"
                 + "Elimina'l primer si vols refer la configuració.</color>";
            return;
        }

        // ── 1. Entry al wUI TextList (sistema de textos de l'asset) ──────────
        int skipTextID = AddEntryToWUI();
        if (skipTextID < 0)
        {
            _log = "<color=red>✗ No s'ha pogut accedir a wUI.asset. Revisa la ruta.</color>";
            return;
        }

        // ── 2. RenderTexture ──────────────────────────────────────────────────
        RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(RT_PATH);
        if (rt == null)
        {
            rt = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
            rt.name = "IntroVideoRT";
            AssetDatabase.CreateAsset(rt, RT_PATH);
            AssetDatabase.SaveAssets();
            Log("✓ RenderTexture creada");
        }
        else
        {
            Log("· RenderTexture ja existia, reutilitzada");
        }

        // ── 3. Canvas ─────────────────────────────────────────────────────────
        GameObject canvasObj = new GameObject("Canvas_Intro");
        Undo.RegisterCreatedObjectUndo(canvasObj, "Setup Intro Video");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
        CanvasGroup canvasGroup = canvasObj.AddComponent<CanvasGroup>();
        Log("✓ Canvas_Intro creat (Sort Order 10)");

        // ── 4. Background ─────────────────────────────────────────────────────
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bg = bgObj.AddComponent<Image>();
        bg.color = Color.black;
        SetFullStretch(bg.rectTransform);

        // ── 5. RawImage per al vídeo ──────────────────────────────────────────
        GameObject rawImgObj = new GameObject("VideoImage");
        rawImgObj.transform.SetParent(canvasObj.transform, false);
        RawImage rawImage = rawImgObj.AddComponent<RawImage>();
        rawImage.texture = rt;
        rawImage.color   = Color.white;
        SetFullStretch(rawImage.rectTransform);
        Log("✓ VideoImage connectat a la RenderTexture");

        // ── 6. Text de saltar (TextProperties del sistema AP) ─────────────────
        GameObject txtObj = new GameObject("TxtSkip");
        txtObj.transform.SetParent(canvasObj.transform, false);

        // Text component (renderitza el text)
        Text txt = txtObj.AddComponent<Text>();
        txt.text      = SKIP_TEXT[0];   // valor per defecte en editor; runtime l'actualitza TextProperties
        txt.color     = Color.white;
        txt.fontSize  = 28;
        txt.alignment = TextAnchor.MiddleLeft;
        txt.font      = GetDefaultFont();

        // Shadow per llegibilitat
        Shadow shadow = txtObj.AddComponent<Shadow>();
        shadow.effectColor    = new Color(0, 0, 0, 0.8f);
        shadow.effectDistance = new Vector2(2, -2);

        // TextProperties: integra el text al sistema de traduccions de l'asset
        TextProperties textProp    = txtObj.AddComponent<TextProperties>();
        textProp.editorType        = 2;          // 2 = currentInfo (wUI)
        textProp.managerID         = skipTextID;
        textProp.language_AutoUpdate = true;

        RectTransform txtRect = txt.rectTransform;
        txtRect.anchorMin        = Vector2.zero;
        txtRect.anchorMax        = Vector2.zero;
        txtRect.pivot            = Vector2.zero;
        txtRect.sizeDelta        = new Vector2(560, 50);
        txtRect.anchoredPosition = new Vector2(30, 30);
        Log($"✓ TxtSkip creat amb TextProperties (wUI ID {skipTextID})");

        // ── 7. Manager GameObject ─────────────────────────────────────────────
        GameObject managerObj = new GameObject("IntroVideoManager");
        Undo.RegisterCreatedObjectUndo(managerObj, "Setup Intro Video");

        AudioSource audioSrc = managerObj.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.loop        = false;
        audioSrc.volume      = 1f;
        if (_audioClip != null)
        {
            audioSrc.clip = _audioClip;
            Log("✓ AudioSource amb Intro_MusicaFinal assignat");
        }
        else
        {
            Log("<color=orange>⚠ No s'ha trobat Intro_MusicaFinal.wav — assigna'l manualment</color>");
        }

        VideoPlayer vp     = managerObj.AddComponent<VideoPlayer>();
        vp.playOnAwake     = false;
        vp.isLooping       = false;
        vp.renderMode      = VideoRenderMode.RenderTexture;
        vp.targetTexture   = rt;
        vp.audioOutputMode = VideoAudioOutputMode.None;
        if (_videoClip != null)
        {
            vp.clip = _videoClip;
            Log("✓ VideoPlayer amb clip assignat");
        }
        else
        {
            Log("· VideoPlayer sense clip (pantalla negra fins que n'arroseguis un)");
        }

        CT_IntroVideo introScript    = managerObj.AddComponent<CT_IntroVideo>();
        introScript.canvasIntro      = canvasObj;
        introScript.canvasGrp        = canvasGroup;
        introScript.txtSkip          = txt;
        introScript.introAudioSource = audioSrc;
        introScript.videoPlayer      = vp;
        introScript.fadeOutDuration  = 1f;
        Log("✓ CT_IntroVideo connectat");

        // ── Finalise ──────────────────────────────────────────────────────────
        Selection.activeGameObject = managerObj;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        _done = true;
        _log += "\n<b>Llest! Guarda l'escena (Ctrl+S).</b>";
        if (_videoClip == null)
            _log += "\n<color=grey>Vídeo: arrossega el clip al VideoPlayer de l'Inspector.</color>";
        Repaint();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Afegeix "Prem espai per saltar" al wUI TextList per als 3 idiomes.
    // Retorna el managerID (índex) de l'entrada, o -1 si hi ha error.
    // ─────────────────────────────────────────────────────────────────────────
    private int AddEntryToWUI()
    {
        TextList wUI = AssetDatabase.LoadAssetAtPath<TextList>(WUI_PATH);
        if (wUI == null || wUI.diaryList == null || wUI.diaryList.Count == 0)
            return -1;

        int entryCount = wUI.diaryList[0]._languageSlot.Count;

        // Guard: if the last entry of lang 0 already matches our text, skip adding
        if (entryCount > 0)
        {
            string lastTitle = wUI.diaryList[0]._languageSlot[entryCount - 1].diaryTitle[0];
            if (lastTitle == SKIP_TEXT[0])
            {
                Log($"· Entrada '{SKIP_TEXT[0]}' ja existia al wUI (ID {entryCount - 1})");
                return entryCount - 1;
            }
        }

        Undo.RecordObject(wUI, "Add Skip Text to wUI");

        for (int lang = 0; lang < wUI.diaryList.Count; lang++)
        {
            string title = lang < SKIP_TEXT.Length ? SKIP_TEXT[lang] : SKIP_TEXT[0];

            var sub = new TextList.apSubtitle
            {
                showMore        = new List<bool>   { false },
                startPointsClip = new List<float>  { 0f },
                firstLetter     = new List<float>  { 0f },
                lastLetter      = new List<int>    { 30 },
                bypasstextLayout= new List<bool>   { false },
                textSub         = new List<string> { "" }
            };

            var slot = new TextList.languageSlot
            {
                diaryTitle             = new List<string>    { title },
                diaryTextDisplayState  = new List<bool>      { true },
                diaryText              = new List<string>    { "" },
                diaryAudioClip         = new List<AudioClip> { null },
                diarySub               = new List<TextList.apSubtitle> { sub },
                diarySprite            = new List<Sprite>    { null },
                refGameObject          = null,
                showInInventory        = false,
                prefabSizeInViewer     = 0.5f,
                prefabRotationInViewer = Vector3.one,
                uniqueItemID           = 0,
                itemType               = 0,
                audioPriority          = 0
            };

            wUI.diaryList[lang]._languageSlot.Add(slot);
        }

        EditorUtility.SetDirty(wUI);
        AssetDatabase.SaveAssets();
        Log($"✓ Entrada afegida al wUI (ID {entryCount}) — CA/ES/EN");
        return entryCount;
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static void SetFullStretch(RectTransform rt)
    {
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.sizeDelta        = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private void Log(string msg)
    {
        _log += (_log == "" ? "" : "\n") + msg;
    }

    private Font GetDefaultFont()
    {
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return f;
    }
}
#endif
