using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSimulator : MonoBehaviour {
	public string screenshotFilename;
	// Use this for initialization
	void Start () {
		screenshotFilename = PythonExecuter.path+"cameraImage.png";
		Time.captureFramerate=15;
	}
	
	// Update is called once per frame
	void Update () {
		
		ScreenCapture.CaptureScreenshot(screenshotFilename);
		
	}
}
