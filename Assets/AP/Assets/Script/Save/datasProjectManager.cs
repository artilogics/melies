// Description : datasProjectManager : datas use in w_ProjectManager 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class datasProjectManager : ScriptableObject 
{
	public bool 		helpBoxEditor = false;							// Show helpbox in the window tab
	public string 		currentDatasProjectFolder = "Default";			// The folder where are the datas for the game (inventory, infos, feedback,diary...)
	public int 			int_CurrentDatasProjectFolder = 0;
	public int 			int_CurrentDatasSaveSystem = 0;
	public string 		s_newProjectName = "New Project";
	public List<string> languageList = new List<string> (){ "Default" };
	public string 		s_newLanguageName = "New Language";
	public int 			firstSceneBuildInIndex = 1;						// The scene loaded when the a new game slot is created
	public List<string> buildinList = new List<string> (){};
    public string       newSceneName = "DefaultScene";
    public string       specificChar = "";
}
	
