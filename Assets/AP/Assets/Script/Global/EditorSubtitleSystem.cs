//Description : EditorSubtitleSystem : Subtitle editor in the inspector. Call by window w_TextnVoice
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class EditorSubtitleSystem {

	Vector2 scrollPos;
	Editor gameObjectEditor2;

	public Texture2D MakeTex(int width, int height, Color col) {					
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	public string	part1Text;
	public bool 	part01bypass = false;


	public Texture2D spritePos;

	public float posOnClip =0;
	public float posOnClip2 =0;

	public float zoomClip = 1;


	public int posNumber = 0;


	public float posInClip = 0;

	public AudioClip currentClip;

	//private int currentMarkerTestSubtitle = 0;

	private GameObject txt_Subtitle;

	public string tmps;

	private Rect waveformRectPosition;

	void OnEnable () {
		//Tex_01 = MakeTex(2, 2, Color.yellow); 
	}


	public void DisplaySubtitleSystem(
		AudioClip _clip,
		string textText,
		SerializedProperty textListSubProp,
		SerializedProperty textListSubPropCurrentLanguage,
		GUIStyle style_Blue, 
		int currentLanguage,
		SerializedProperty diaryList,
		int _i,
		int _j)
	{

		float tmpStartPosition = textListSubProp.FindPropertyRelative ("startPointsClip").GetArrayElementAtIndex (0).floatValue;

		GUIStyle bgColor = new GUIStyle();
		bgColor.normal.background = EditorGUIUtility.whiteTexture;




		if (_clip != null ) {
			if (gameObjectEditor2 == null || currentClip != _clip) {
				gameObjectEditor2 = Editor.CreateEditor (_clip);
				currentClip = _clip;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("Audio name: " + _clip.name + " /  Duration : " +  _clip.length.ToString ());
			EditorGUILayout.EndHorizontal ();


// --> Preview audioClip
			generateSpace (2);
			waveformRectPosition = GUILayoutUtility.GetLastRect();

			EditorGUILayout.BeginHorizontal (GUILayout.Width (605));



			EditorGUILayout.EndHorizontal ();
			generateSpace (20);

// --> Generate Sprite Position



			generateSpace (5);


            if (GUILayout.Button("Play audio clip"))
            {
                AP_StopAllClips();
                AP_PlayClip(_clip, 50 * _clip.frequency);
            }
			if (GUILayout.Button ("Stop audio clip")) {
                AP_StopAllClips ();
			}
			generateSpace (1);


			displayTextParts (tmpStartPosition,textText, textListSubPropCurrentLanguage,style_Blue);

			if (currentLanguage == 0) {
				if (GUILayout.Button ("Add New Marker")) {
					AddNewMarker (textListSubProp, _clip, diaryList, _i, _j);
				}
				if (textListSubProp.FindPropertyRelative ("startPointsClip").arraySize > 1) {
					if (GUILayout.Button ("Remove Last Marker")) {
						RemoveLastMarker (textListSubProp, diaryList, _i, _j);
					}
				}
			}

            if (AP_IsClipPlaying (_clip)) {
				for (var i = 0; i < textListSubProp.FindPropertyRelative ("startPointsClip").arraySize; i++) {
                    if (textListSubPropCurrentLanguage.FindPropertyRelative ("startPointsClip").GetArrayElementAtIndex (i).floatValue > AP_GetClipPosition (_clip)) {

						if (txt_Subtitle == null)
							txt_Subtitle = GameObject.Find ("txt_Subtitle");
						
						if (txt_Subtitle && i > 0
						    &&
							txt_Subtitle.GetComponent<Text> ().text != textListSubPropCurrentLanguage.FindPropertyRelative ("textSub").GetArrayElementAtIndex (i - 1).stringValue) {
							txt_Subtitle.GetComponent<Text> ().text = textListSubPropCurrentLanguage.FindPropertyRelative ("textSub").GetArrayElementAtIndex (i - 1).stringValue;
							UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
						} 

						break;
					} else {
						if (txt_Subtitle && i == textListSubProp.FindPropertyRelative ("startPointsClip").arraySize - 1
							&&
							txt_Subtitle.GetComponent<Text> ().text != textListSubPropCurrentLanguage.FindPropertyRelative ("textSub").GetArrayElementAtIndex (i).stringValue) {
							txt_Subtitle.GetComponent<Text> ().text = textListSubPropCurrentLanguage.FindPropertyRelative ("textSub").GetArrayElementAtIndex (i).stringValue;
							UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
						}

					}
				}
			}


		}

		gameObjectEditor2.OnInteractivePreviewGUI (new Rect (5, waveformRectPosition.y, 600, 100), bgColor);
		//generateSpace (20);

		EditorGUILayout.BeginVertical();
		spritePosition (tmpStartPosition, _clip, textListSubProp,currentLanguage);
		EditorGUILayout.EndVertical ();
		generateSpace (5);
	}



	private void spritePosition (float tmpStartPosition,AudioClip _clip, SerializedProperty textListSubProp,int currentLanguage){

		EditorGUILayout.Space ();


		SerializedProperty startPointsClip = textListSubProp.FindPropertyRelative ("startPointsClip");

		for (var i = 0; i < startPointsClip.arraySize; i++) {

			float _posOnClip = (startPointsClip.GetArrayElementAtIndex (i).floatValue / _clip.length) * 600;

			if (spritePos == null) {
				spritePos = (Texture2D)EditorGUIUtility.Load ("Assets/AP/Assets/Textures/Sprites/Edit/Triangle.png");
			} else {
				EditorGUI.DrawPreviewTexture (new Rect (_posOnClip + 3, waveformRectPosition.y + 120 - 130, 2, 150), spritePos);
			}

			if (posNumber == i && currentLanguage == 0) {
				startPointsClip.GetArrayElementAtIndex (i).floatValue = GUI.HorizontalSlider (new Rect (0, waveformRectPosition.y+ 120, 608, 16), startPointsClip.GetArrayElementAtIndex (i).floatValue, 0.0F, _clip.length);
			}

			if (GUI.Button (new Rect (_posOnClip - 7, waveformRectPosition.y + 85 + 15, 25, 20), i.ToString())) {
               
				if (i == posNumber) {
					if (!AP_IsClipPlaying (_clip)) {
                        AP_PlayClip (_clip, 50 * _clip.frequency);
                        AP_SetClipSamplePosition (_clip, Mathf.RoundToInt (startPointsClip.GetArrayElementAtIndex (i).floatValue * _clip.frequency));
					} else {
                        AP_StopAllClips ();
					}

				}
				posNumber = i;
			}

		}
	}




	private void displayTextParts (float tmpStartPosition,string textText, SerializedProperty textListSubProp,GUIStyle style_Yellow_01){

		Rect r = GUILayoutUtility.GetLastRect();

		for(var i = 0;i < textListSubProp.FindPropertyRelative ("startPointsClip").arraySize;i++){

			SerializedProperty _showMore = textListSubProp.FindPropertyRelative ("showMore").GetArrayElementAtIndex (i);
			SerializedProperty _firstLetter = textListSubProp.FindPropertyRelative ("firstLetter").GetArrayElementAtIndex (i);
			SerializedProperty _lastLetter = textListSubProp.FindPropertyRelative ("lastLetter").GetArrayElementAtIndex (i);

			SerializedProperty _bypasstextLayout = textListSubProp.FindPropertyRelative ("bypasstextLayout").GetArrayElementAtIndex (i);
			SerializedProperty _textSub = textListSubProp.FindPropertyRelative ("textSub").GetArrayElementAtIndex (i);

			EditorGUILayout.BeginVertical (style_Yellow_01);

		EditorGUILayout.BeginHorizontal ();
// --> ShowMore
			if (_showMore.boolValue) {
				if (GUILayout.Button ("v",GUILayout.Width(20))) {
					_showMore.boolValue = false;
				}
			}
			else{
				if (GUILayout.Button (">",GUILayout.Width(20))) {
					_showMore.boolValue = true;
				}
			}

			if (GUILayout.Button ("Select", GUILayout.Width (45))) {
				if(txt_Subtitle == null)
					txt_Subtitle = GameObject.Find ("txt_Subtitle");
				if (txt_Subtitle) {
					//Debug.Log ("here");
					txt_Subtitle.GetComponent<Text> ().text = _textSub.stringValue;

				}
				UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
			}

// --> ID

			//EditorGUILayout.LabelField (_startPointClip.floatValue.ToString(),GUILayout.Width (40));
			EditorGUILayout.LabelField("ID:",GUILayout.Width (20));
			EditorGUILayout.LabelField (i.ToString(),GUILayout.Width (20));


			// --> Display Text in text area
			if (!_bypasstextLayout.boolValue) {
				string tmpString = "";

				int textLength = Mathf.RoundToInt (_firstLetter.floatValue) + _lastLetter.intValue;
				if (_firstLetter.floatValue + _lastLetter.intValue > textText.Length)
					textLength = textText.Length;


				for (var k = Mathf.RoundToInt (_firstLetter.floatValue); k < textLength; k++) {
					tmpString += textText.Substring (k, 1);
				}
				_textSub.stringValue = GUILayout.TextArea (tmpString, GUILayout.Width (540));




			} else {
				_textSub.stringValue = GUILayout.TextArea (_textSub.stringValue, GUILayout.Width (540));
			}
		EditorGUILayout.EndHorizontal ();
		
			if (_showMore.boolValue) {
				EditorGUILayout.LabelField("",GUILayout.Width (10),GUILayout.Height (1));
				r = GUILayoutUtility.GetLastRect();
// --> First Letter
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("First Letter:", GUILayout.Width (70));
				//_firstLetter.floatValue = EditorGUILayout.Slider (Mathf.RoundToInt (_firstLetter.floatValue), 0, textText.Length, GUILayout.Width (490));
				_firstLetter.floatValue = GUI.HorizontalSlider (new Rect (140, r.y+5, 490, 16),_firstLetter.floatValue, 0.0F, textText.Length);


				if (GUILayout.Button ("<", GUILayout.Width (20))) {
					if (_firstLetter.floatValue > 0)
						_firstLetter.floatValue--;
				}
				if (GUILayout.Button (">", GUILayout.Width (20))) {
					if (_firstLetter.floatValue < textText.Length)
						_firstLetter.floatValue++;
				}
				EditorGUILayout.EndHorizontal ();

// --> last Letter
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Nbr of Char:", GUILayout.Width (70));

				if (GUILayout.Button ("<", GUILayout.Width (20))) {
					if (_lastLetter.intValue > 0)
						_lastLetter.intValue--;
				}
				if (GUILayout.Button (">", GUILayout.Width (20))) {
					if (_lastLetter.intValue < 200)
						_lastLetter.intValue++;
				}

				_lastLetter.intValue = EditorGUILayout.IntSlider (_lastLetter.intValue, 0, 200, GUILayout.Width (490));
				//_lastLetter.intValue = GUI. (new Rect (140, r.y+20, 490, 16),_lastLetter.intValue, 0, 200);


				EditorGUILayout.EndHorizontal ();

// --> Bypass text layout
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Bypass text layout :", GUILayout.Width (110));
				_bypasstextLayout.boolValue = EditorGUILayout.Toggle (_bypasstextLayout.boolValue);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}
			EditorGUILayout.EndVertical ();
		}

	}

	private void AddNewMarker(SerializedProperty textListSubProp,AudioClip _clip, SerializedProperty diaryList,int _i,int _j){
		int howManySub = diaryList.arraySize;
		//Debug.Log (howManySub);

		for (var i = 0; i < howManySub; i++) {
			SerializedProperty sub = diaryList.GetArrayElementAtIndex (i).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (_i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex (_j);
			int arrayLength = sub.FindPropertyRelative ("startPointsClip").arraySize - 1;

			sub.FindPropertyRelative ("showMore").InsertArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("startPointsClip").InsertArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("firstLetter").InsertArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("lastLetter").InsertArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("bypasstextLayout").InsertArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("textSub").InsertArrayElementAtIndex (arrayLength);

			sub.FindPropertyRelative ("showMore").GetArrayElementAtIndex (arrayLength + 1).boolValue = true;

			sub.FindPropertyRelative ("firstLetter").GetArrayElementAtIndex (arrayLength + 1).floatValue +=
				(float)sub.FindPropertyRelative ("lastLetter").GetArrayElementAtIndex (arrayLength).intValue;

			sub.FindPropertyRelative ("lastLetter").GetArrayElementAtIndex (arrayLength + 1).intValue = 30;

			if (sub.FindPropertyRelative ("startPointsClip").GetArrayElementAtIndex (arrayLength + 1).floatValue + 2 < _clip.length)
				sub.FindPropertyRelative ("startPointsClip").GetArrayElementAtIndex (arrayLength + 1).floatValue += 2;

		}
	}

	private void RemoveLastMarker(SerializedProperty textListSubProp, SerializedProperty diaryList,int _i,int _j){

		int howManySub = diaryList.arraySize;

		for (var i = 0; i < howManySub; i++) {
			SerializedProperty sub = diaryList.GetArrayElementAtIndex (i).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (_i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex (_j);

			int arrayLength = sub.FindPropertyRelative ("startPointsClip").arraySize - 1;

			sub.FindPropertyRelative ("showMore").DeleteArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("startPointsClip").DeleteArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("firstLetter").DeleteArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("lastLetter").DeleteArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("bypasstextLayout").DeleteArrayElementAtIndex (arrayLength);
			sub.FindPropertyRelative ("textSub").DeleteArrayElementAtIndex (arrayLength);
		}
	}

	private void generateSpace(int howManySpace){
		for (var i = 0; i < howManySpace; i++) {
			EditorGUILayout.Space ();
		}
	}

    public static void AP_StopAllClips() {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		//-> Debug
		/*for (var i = 0; i < (int)audioUtilClass.GetMethods().Length; i++)
		{
			if (audioUtilClass.GetMethods()[i].Name == "PlayClip")
				Debug.Log(audioUtilClass.GetMethod("PlayClip"));
			Debug.Log(audioUtilClass.GetMethods()[i].Name);
		}*/


		//-> Case 2019.4 to 2020.2.1
		//MethodInfo method = audioUtilClass.GetMethod("StopAllClips",BindingFlags.Static | BindingFlags.Public,null,new System.Type[]{},null);
		//method.Invoke(null,new object[] {});

		//-> Case 2020.2.2
		MethodInfo method = audioUtilClass.GetMethod("StopAllPreviewClips");
		method.Invoke(null, new object[] { });
	}

	public void callStopAudio(){
        AP_StopAllClips ();
	}

 
    public static void AP_PlayClip(AudioClip clip , int startSample) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		//-> Case 2019.4 to 2020.2.1
		//MethodInfo method = audioUtilClass.GetMethod("PlayClip",BindingFlags.Static | BindingFlags.Public,null,new System.Type[] {typeof(AudioClip),typeof(Int32)},null);
		//method.Invoke(null,new object[] {clip,startSample});

		//-> Case 2020.2.2
		MethodInfo method = audioUtilClass.GetMethod("PlayPreviewClip");
		method.Invoke(null, new object[] { clip, startSample, false });
	}

    public static void AP_SetClipSamplePosition(AudioClip clip , int iSamplePosition) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		//-> Case 2019.4 to 2020.2.1
		//MethodInfo method = audioUtilClass.GetMethod("SetClipSamplePosition",BindingFlags.Static | BindingFlags.Public);
		//method.Invoke(null, new object[] { clip, iSamplePosition });

		//-> Case 2020.2.2
		MethodInfo method = audioUtilClass.GetMethod("SetPreviewClipSamplePosition");
		method.Invoke(null,new object[] {clip,iSamplePosition});
	}

    public static bool AP_IsClipPlaying(AudioClip clip) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		//-> Case 2019.4 to 2020.2.1
		//MethodInfo method = audioUtilClass.GetMethod("IsClipPlaying",BindingFlags.Static | BindingFlags.Public);
		//bool playing = (bool)method.Invoke(null,new object[] {clip,});

		//-> Case 2020.2.2
		MethodInfo method = audioUtilClass.GetMethod("IsPreviewClipPlaying");
		bool playing = (bool)method.Invoke(null, new object[] {});
		return playing;
	}

	public static float AP_GetClipPosition(AudioClip clip) {
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		//-> Case 2019.4 to 2020.2.1
		//MethodInfo method = audioUtilClass.GetMethod("GetClipPosition",BindingFlags.Static | BindingFlags.Public);
		//float position = (float)method.Invoke(null,new object[] {clip});

		//-> Case 2020.2.2
		MethodInfo method = audioUtilClass.GetMethod("GetPreviewClipPosition");
		float position = (float)method.Invoke(null, new object[] { });

		return position;
	}

}
#endif