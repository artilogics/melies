//Descritpion : windowMethods : A collection of methods use in multiple window tab.
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine.SceneManagement;

public class windowMethods {

    public bool bool_UpdateProcessDone = false;         // use to launch game in the editor mode (Button : Update + play)

    public void updateIDs()
    {
        int numberTotal = 0;
        int number = 0;
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject go in allObjects)
        {
            Transform[] Children = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in Children)
            {
                if (child.GetComponent<TextProperties>())
                {
                    Find_UniqueId_In_TextProperties(child);
                    number++;
                }
                if (child.GetComponent<VoiceProperties>())
                {
                    Find_UniqueId_In_VoiceProperties(child);
                    number++;
                }
                if (child.GetComponent<objTranslateRotate>())
                {
                    Find_UniqueId_In_objRotateTranslate(child);
                    number++;
                }
                if (child.GetComponent<conditionsToAccessThePuzzle>())
                {
                    Find_UniqueId_In_Puzzle(child);
                    number++;
                }
                numberTotal++;
            }
        }

        GameObject tmpObj = GameObject.Find("UpdateManually");

        if (tmpObj)
        {
            Selection.activeGameObject = tmpObj;
        }

        Debug.Log("Info : " + number + " ID have been updated. There are " + numberTotal + " gameObjects in the scene. " +
            "\n!!! Don't forget to do this operation on each project Scene !!!");



    }


    //--> Return the TextList that needs to be loaded
    private string returnTextListType(int value){
        string result = "";
        if (value == 0){
            result = "wTextnVoices"; }
        else if (value == 1){
            result = "wItem";}
        else if (value == 2){
            result = "wUI";}
        else
            result = "wFeedback"; 
        
        return result;
    }




    // --> Update Id for a specific gameObject
    public void Find_UniqueId_In_TextProperties(Transform child)
    {
        int HowManyEntry = 0;

        TextList _textList;

        if (child.GetComponent<TextProperties>().textList == null)
        {
            _textList = loadTextList(returnTextListType(child.GetComponent<TextProperties>().editorType));

            HowManyEntry = _textList.diaryList[0]._languageSlot.Count;
        }
        else{
            _textList = child.GetComponent<TextProperties>().textList;
        }


        HowManyEntry = _textList.diaryList[0]._languageSlot.Count;

            for (var i = 0; i < HowManyEntry; i++)
            {
            if (_textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<TextProperties>().uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);

                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<TextProperties>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("managerID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();

                    break;
                }
            }
    }

    // --> Update Id for a specific gameObject
    public void Find_UniqueId_In_VoiceProperties(Transform child)
    {
        int HowManyEntry = 0;

        TextList _textList;

        if (child.GetComponent<VoiceProperties>().textList == null)
        {
            _textList = loadTextList(returnTextListType(child.GetComponent<VoiceProperties>().editorType));
            Debug.Log(_textList.name);

            HowManyEntry = _textList.diaryList[0]._languageSlot.Count;
        }
        else
        {
            _textList = child.GetComponent<VoiceProperties>().textList;
        }


        HowManyEntry = _textList.diaryList[0]._languageSlot.Count;

            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<VoiceProperties>().textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<VoiceProperties>().uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);

                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<VoiceProperties>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("managerID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();

                    break;
                }
            }
    }

    public TextList loadTextList(string s_Load)
    {
        TextList _textList;

        string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
        datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath(objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;

        string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/" + s_Load + ".asset";

        //Debug.Log("objectPath : " + objectPath);

        _textList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as TextList;
        return _textList;
    }

    // --> Update Id for a specific gameObject
    public void Find_UniqueId_In_objRotateTranslate(Transform child)
    {
        TextList _textList;

        //-> Update : Player Object needed in the Inventory 
        _textList = loadTextList("wItem");

        int HowManyEntry = _textList.diaryList[0]._languageSlot.Count;

            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<objTranslateRotate>().inventoryIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<objTranslateRotate>().inventoryIDList[0].uniqueID)
                {
                    //Debug.Log ("Here : " + i);

                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<objTranslateRotate>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("inventoryIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
     

        //-> Update : Voice lock and Unlock
        _textList = loadTextList("wTextnVoices");

             HowManyEntry = _textList.diaryList[0]._languageSlot.Count;

            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<objTranslateRotate>().diaryIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<objTranslateRotate>().diaryIDList[0].uniqueID)
                {

                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<objTranslateRotate>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("diaryIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<objTranslateRotate>().diaryIDListUnlock.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<objTranslateRotate>().diaryIDListUnlock[0].uniqueID)
                {

                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<objTranslateRotate>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("diaryIDListUnlock").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
      

        //-> Update : Feedback lock and Unlock
        _textList = loadTextList("wFeedback");


            HowManyEntry = _textList.diaryList[0]._languageSlot.Count;

            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<objTranslateRotate>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<objTranslateRotate>().feedbackIDList[0].uniqueID)
                {

                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<objTranslateRotate>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<objTranslateRotate>().feedbackIDListUnlock.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<objTranslateRotate>().feedbackIDListUnlock[0].uniqueID)
                {

                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<objTranslateRotate>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDListUnlock").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }

    }


    public void Find_UniqueId_In_Puzzle(Transform child)
    {
        TextList _textList;

        //-> Update : Player Object needed in the Inventory 
        _textList = loadTextList("wItem");

      
            int HowManyEntry = _textList.diaryList[0]._languageSlot.Count;

            for (var i = 0; i < HowManyEntry; i++)
            {
                for (var j = 0; j < child.GetComponent<conditionsToAccessThePuzzle>().inventoryIDList.Count;j++){
                    if (child.GetComponent<conditionsToAccessThePuzzle>().inventoryIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<conditionsToAccessThePuzzle>().inventoryIDList[j].uniqueID)
                    {
                        Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                        SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<conditionsToAccessThePuzzle>());
                        SerializedProperty m_managerID = serializedObject2.FindProperty("inventoryIDList").GetArrayElementAtIndex(j).FindPropertyRelative("ID");

                        serializedObject2.Update();
                        m_managerID.intValue = i;
                        serializedObject2.ApplyModifiedProperties();
                        break;
                    } 
                }

            }
      
        //-> Update : Feedback lock and Unlock
        _textList = loadTextList("wFeedback");

      
            HowManyEntry = _textList.diaryList[0]._languageSlot.Count;

            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<conditionsToAccessThePuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<conditionsToAccessThePuzzle>().feedbackIDList[0].uniqueID)
                {

                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<conditionsToAccessThePuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }

        if(child.GetComponent<LogicsPuzzle>()){
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<LogicsPuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<LogicsPuzzle>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<LogicsPuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        if (child.GetComponent<PipesPuzzle>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<PipesPuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<PipesPuzzle>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<PipesPuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        if (child.GetComponent<GearsPuzzle>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<GearsPuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<GearsPuzzle>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<GearsPuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        if (child.GetComponent<cylinderPuzzle>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<cylinderPuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<cylinderPuzzle>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<cylinderPuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        if (child.GetComponent<DigicodePuzzle>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<DigicodePuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<DigicodePuzzle>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<DigicodePuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        if (child.GetComponent<LeverPuzzle>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<LeverPuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<LeverPuzzle>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<LeverPuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        if (child.GetComponent<SlidingPuzzle>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<SlidingPuzzle>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<SlidingPuzzle>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<SlidingPuzzle>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        if (child.GetComponent<focusOnly>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<focusOnly>().feedbackIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<focusOnly>().feedbackIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<focusOnly>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }

        //-> Update : Feedback lock and Unlock
        _textList = loadTextList("wTextnVoices");
        HowManyEntry = _textList.diaryList[0]._languageSlot.Count;
        if (child.GetComponent<focusOnly>())
        {
            for (var i = 0; i < HowManyEntry; i++)
            {
                if (child.GetComponent<focusOnly>().diaryIDList.Count > 0
                    && _textList.diaryList[0]._languageSlot[i].uniqueItemID == child.GetComponent<focusOnly>().diaryIDList[0].uniqueID)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child, child.name);
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(child.GetComponent<focusOnly>());
                    SerializedProperty m_managerID = serializedObject2.FindProperty("diaryIDList").GetArrayElementAtIndex(0).FindPropertyRelative("ID");

                    serializedObject2.Update();
                    m_managerID.intValue = i;
                    serializedObject2.ApplyModifiedProperties();
                    break;
                }
            }
        }
    }




   // [MenuItem("Tools/AP/Update/Save System/Create a list of objects to save in the current scene", false, 2)]
    public void saveLevelInfos()
    {
        List<GameObject> listGameObject = new List<GameObject>();
        List<string> listString = new List<string>();

        int numberTotal = 0;
        int number = 0;
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject go in allObjects)
        {
            Transform[] Children = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in Children)
            {
                if (child.GetComponent<SaveData>())
                {
                    //Find_UniqueId_In_The_TextList (child);
                    listGameObject.Add(child.gameObject);
                    listString.Add(child.GetComponent<SaveData>().R_SaveData());

                    number++;
                }
                numberTotal++;
            }
        }
        GameObject tmp = GameObject.Find("LevelManager");
        string tmpString = "";
        if (tmp)
        {
            Undo.RegisterFullObjectHierarchyUndo(tmp, tmp.name);

            LevelManager levelManager = tmp.GetComponent<LevelManager>();

            levelManager.listOfGameObjectForSaveSystem.Clear();
            levelManager.listState.Clear();

            for (var i = 0; i < listGameObject.Count; i++)
            {
                levelManager.listOfGameObjectForSaveSystem.Add(listGameObject[i]);
                levelManager.listState.Add(false);
                tmpString += listString[i];
            }

        }
        else
        {
            tmp = GameObject.Find("MM_NoLevelManager");

            if(!tmp)
                //Debug.Log ("Info : You need a LevelManager in your scene to be allowed to save data for this level");
                if (EditorUtility.DisplayDialog("Info : This action is not possible."
                    , "You need an object LevelManager in your scene to record data for this level. LevelManager need to have LevelManager.cs attached to it."
                    , "Continue")) { }



        }
        Debug.Log("Level Info Ok");
    }

    //--> Create a list of scenes used in the Save System
    //[MenuItem("Tools/AP/Update/Save System/Create a list of build in scenes", false, 2)]
    public void builinScene(bool b_LaunchScene, bool b_UpdateProcessDone)
    {
        string objectPath = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
        datasProjectManager _ProjectManagerDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as datasProjectManager;

        if (_ProjectManagerDatas)
        {
            Undo.RegisterFullObjectHierarchyUndo(_ProjectManagerDatas, _ProjectManagerDatas.name);


            _ProjectManagerDatas.buildinList.Clear();
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                string scenName = EditorBuildSettings.scenes[i].path;
                scenName = scenName.Replace(".unity", "");

                for (var j = scenName.Length - 1; j > 1; j--)
                {
                    char tmp = scenName[j];

                    if (tmp == '/')
                    {
                        scenName = scenName.Substring(j + 1, scenName.Length - 1 - j);
                        break;
                    }
                }

                _ProjectManagerDatas.buildinList.Add(scenName);

            }
        }
        else
        {
            if (EditorUtility.DisplayDialog("Info : This action is not possible."
                , "You need an object datasProjectManager in your Project Tab : Assets/AP/Assets/Datas/ProjectManagerDatas.asset."
                , "Continue")) { }

        }
        Debug.Log("Build In Scene Ok");
        if(b_LaunchScene)
            bool_UpdateProcessDone = true;
    }
}
#endif