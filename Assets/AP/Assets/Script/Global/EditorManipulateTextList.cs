// Description : EditorManipulateTextList.cs : Methods to manipulate TextList scriptable Object
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[System.Serializable]
public class EditorManipulateTextList{
// --> Remove a language
	public void Remove_A_Language (SerializedProperty _listOfLanguage,SerializedProperty diaryList,int i){
		_listOfLanguage.DeleteArrayElementAtIndex (i);
		diaryList.DeleteArrayElementAtIndex (i);
	}

// --> Add a new language
	public void Add_A_Language (SerializedProperty _listOfLanguage, SerializedProperty diaryList){
		
		_listOfLanguage.InsertArrayElementAtIndex (_listOfLanguage.arraySize);
		_listOfLanguage.GetArrayElementAtIndex(_listOfLanguage.arraySize-1).stringValue = "New Language";

		//diaryList.InsertArrayElementAtIndex (diaryList.arraySize);

		diaryList.GetArrayElementAtIndex (0).DuplicateCommand ();
		diaryList.MoveArrayElement (1, diaryList.arraySize - 1);

	}

	// --> Add a new language
	public void Add_A_Language (SerializedProperty _listOfLanguage, SerializedProperty diaryList,string languageName){

		_listOfLanguage.InsertArrayElementAtIndex (_listOfLanguage.arraySize);
		_listOfLanguage.GetArrayElementAtIndex(_listOfLanguage.arraySize-1).stringValue = languageName;

		//diaryList.InsertArrayElementAtIndex (diaryList.arraySize);

		diaryList.GetArrayElementAtIndex (0).DuplicateCommand ();
		diaryList.MoveArrayElement (1, diaryList.arraySize - 1);

	}


// --> Add new text section
	public void AddTextEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i,string newText,string newTitle){

        Debug.Log("newTitle : " + newTitle + "diaryList.arraySize : " + diaryList.arraySize);

		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);

		for (var j = 0; j < diaryList.arraySize; j++) {
			SerializedProperty languageSlot = diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot");

            if (i == -1)
                i = languageSlot.arraySize - 1;

			languageSlot.InsertArrayElementAtIndex (i + 1);

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryTitle").ClearArray ();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryTextDisplayState").ClearArray ();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryText").ClearArray ();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryAudioClip").ClearArray ();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySprite").ClearArray ();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").ClearArray ();

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryTitle").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryTextDisplayState").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryText").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryAudioClip").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySprite").InsertArrayElementAtIndex (0);

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").InsertArrayElementAtIndex (0);



            languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex (0).stringValue = newTitle;
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryTextDisplayState").GetArrayElementAtIndex (0).boolValue = true;
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryText").GetArrayElementAtIndex (0).stringValue = newText;

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diaryAudioClip").GetArrayElementAtIndex (0).objectReferenceValue = null;

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("refGameObject").objectReferenceValue = null;

			Debug.Log ("AddTextEntry");
	

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("showMore").ClearArray();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("startPointsClip").ClearArray();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("firstLetter").ClearArray();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("lastLetter").ClearArray();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("bypasstextLayout").ClearArray();
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("textSub").ClearArray();


			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("showMore").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("startPointsClip").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("firstLetter").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("lastLetter").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("bypasstextLayout").InsertArrayElementAtIndex (0);
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("textSub").InsertArrayElementAtIndex (0);



			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySprite").GetArrayElementAtIndex (0).objectReferenceValue = null;


			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(0).FindPropertyRelative("lastLetter").GetArrayElementAtIndex(0).intValue = 30;

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("prefabSizeInViewer").floatValue = .5f;
			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("showInInventory").boolValue = false;

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("itemType").intValue = 0;

			int newUniqueID = 0;

// --> generate a unique ID for the new Item
			for (var m = 0; m < languageSlot.arraySize; m++) {
				if(languageSlot.GetArrayElementAtIndex (m).FindPropertyRelative ("uniqueItemID").intValue > newUniqueID)
					newUniqueID = languageSlot.GetArrayElementAtIndex (m).FindPropertyRelative ("uniqueItemID").intValue;
			}

			languageSlot.GetArrayElementAtIndex (i + 1).FindPropertyRelative ("uniqueItemID").intValue = newUniqueID+1;



		}
	}

// --> Remove a Text Section
	public void removeTextEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i){
		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		for(var j = 0;j<diaryList.arraySize;j++){
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").DeleteArrayElementAtIndex (i);

		}
	}


// --> Move down a text section
	public void moveNextTextEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i){
		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		for (var j = 0; j < diaryList.arraySize; j++) {
			SerializedProperty languageSlot = diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot");
			languageSlot.MoveArrayElement (i, i + 1);
		}
	}

// --> Move up a text section
	public void moveLastTextEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i){
		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		for (var j = 0; j < diaryList.arraySize; j++) {
			SerializedProperty languageSlot = diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot");
			languageSlot.MoveArrayElement (i,i-1);
		}

	}

// --> Add a new page in Text section
	public void AddPageEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i, int j){
		/*if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);

		diaryList.GetArrayElementAtIndex(i).FindPropertyRelative ("diaryText").InsertArrayElementAtIndex (j+1);
		diaryList.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryText").GetArrayElementAtIndex (j+1).stringValue = "New Page";

		diaryList.GetArrayElementAtIndex(i).FindPropertyRelative ("diaryAudioClip").InsertArrayElementAtIndex (j+1);
		diaryList.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryAudioClip").GetArrayElementAtIndex (j+1).objectReferenceValue = null;

		diaryList.GetArrayElementAtIndex(i).FindPropertyRelative ("diarySprite").InsertArrayElementAtIndex (j+1);
		diaryList.GetArrayElementAtIndex (i).FindPropertyRelative ("diarySprite").GetArrayElementAtIndex (j+1).objectReferenceValue = null;*/
	}


// --> remove a page in a page section
	public void removePageEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i, int j){
		/*if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		diaryList.GetArrayElementAtIndex(i).FindPropertyRelative ("diaryText").DeleteArrayElementAtIndex (j);*/
	}

// --> Move down a page
	public void moveNextPageEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i, int j){
		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		for (var k = 0; k < diaryList.arraySize; k++) {
			SerializedProperty languageSlot = diaryList.GetArrayElementAtIndex (k).FindPropertyRelative ("_languageSlot");
			languageSlot.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryText").MoveArrayElement (j,j+1);
		}
	}

// --> Move up a page
	public void moveLastPageEntry(MonoBehaviour myScript, SerializedProperty diaryList, int i, int j){
		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		for (var k = 0; k < diaryList.arraySize; k++) {
			SerializedProperty languageSlot = diaryList.GetArrayElementAtIndex (k).FindPropertyRelative ("_languageSlot");
			languageSlot.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryText").MoveArrayElement (j,j-1);
		}
	}

// --> Test a Text 
/*	public void TestText(manageDiary myScript, SerializedProperty diaryList, int i, int j){
		if (myScript != null) {
			Undo.RegisterFullObjectHierarchyUndo (myScript.displayDiaryTxt, myScript.displayDiaryTxt.name);
			myScript.displayDiaryTxt.text = myScript.textList.diaryList [i]._languageSlot [0].diaryText [j];
		}
		UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
	}
*/		

// --> Add a new page in multi page Item
	public void AddNewPageEntry_In_MultiPages_Item(MonoBehaviour myScript, SerializedProperty diaryList, int i, int k){
		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		//Debug.Log (diaryList.arraySize);
		for(var j = 0;j<diaryList.arraySize;j++){
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative ("diaryText").InsertArrayElementAtIndex (k+1);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative ("diaryText").GetArrayElementAtIndex (k+1).stringValue = "New Page";

			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative ("diaryAudioClip").InsertArrayElementAtIndex (k+1);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diaryAudioClip").GetArrayElementAtIndex (k+1).objectReferenceValue = null;

			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative ("diarySprite").InsertArrayElementAtIndex (k+1);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySprite").GetArrayElementAtIndex (k+1).objectReferenceValue = null;

			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").InsertArrayElementAtIndex (k+1);

			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("showMore").ClearArray();
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("startPointsClip").ClearArray();
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("firstLetter").ClearArray();
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("lastLetter").ClearArray();
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("bypasstextLayout").ClearArray();
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("textSub").ClearArray();


			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("showMore").InsertArrayElementAtIndex (0);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("startPointsClip").InsertArrayElementAtIndex (0);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("firstLetter").InsertArrayElementAtIndex (0);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("lastLetter").InsertArrayElementAtIndex (0);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("bypasstextLayout").InsertArrayElementAtIndex (0);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("textSub").InsertArrayElementAtIndex (0);

			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").GetArrayElementAtIndex(k+1).FindPropertyRelative("lastLetter").GetArrayElementAtIndex(0).intValue = 30;

		}
	}

// --> remove a page in multi page Item
	public void RemovePageEntry_In_MultiPages_Item(MonoBehaviour myScript, SerializedProperty diaryList, int i, int k){
		if(myScript!=null)Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);


		for(var j = 0;j<diaryList.arraySize;j++){
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative ("diaryText").DeleteArrayElementAtIndex (k);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative ("diaryAudioClip").DeleteArrayElementAtIndex (k);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative ("diarySprite").DeleteArrayElementAtIndex (k);
			diaryList.GetArrayElementAtIndex (j).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySub").DeleteArrayElementAtIndex (k);

		
		}
	}


}
#endif