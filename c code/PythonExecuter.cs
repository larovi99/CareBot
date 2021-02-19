using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class PythonExecuter : MonoBehaviour {
	public static string path;
	public static string pythonSource = "C:/Users/hp/AppData/Local/Programs/Python/Python37/python.exe";
	public static string[] values = new string[] {"","","","","","","","","",""};
	public static bool[] threads = new bool[] {false, false, false, false,false,false, false, false, false,false};
	void Awake () 
	{
		
        string[] res = System.IO.Directory.GetFiles(Application.dataPath, "PythonExecuter.cs", SearchOption.AllDirectories);
		if (res.Length == 0)
		{
		    UnityEngine.Debug.LogError("Python folder not found");
		}
		path = res[0].Replace("PythonExecuter.cs", "Python/").Replace("\\", "/");
		//ExecuteScript("test.py");
		//UnityEngine.Debug.Log(path);
	}
	
	// Update is called once per frame
	public static IEnumerator ExecuteScript(string name, int value)
	{
		PythonExecuter.threads[value]=true;
		ProcessStartInfo start = new ProcessStartInfo();
	    start.FileName = pythonSource;
	    start.Arguments = string.Format("{0}", path+name);
	    start.UseShellExecute = false;
	    start.RedirectStandardOutput = true;
	    string fullresult="";
	    using(Process process = Process.Start(start))
	    {
	    	
	        using(StreamReader reader = process.StandardOutput)
	        {
	        	yield return null;
	            string result = reader.ReadToEnd();
	            fullresult+=result;
	            
	            
	        }
	        
	    }

	    fullresult=fullresult.Replace("\n",",");
	    fullresult=fullresult.Replace("\r","");
	    PythonExecuter.values[value]=fullresult;
	    PythonExecuter.threads[value]=false;
	}
}
