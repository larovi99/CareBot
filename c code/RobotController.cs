using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {

	public float movementSpeed=0.3f;
	public float rotationSpeed=10f;
	public SpeechRecognizer speech;
	public ArmInverseKinematics arm;
	public string command="";
	public string lastMovementCommand="";
	public bool shouldMove = false;
	public bool shouldSearchTarget = false;
	bool shouldGrabTarget = false;
	public string objectToTake = "";
	public int visionMode = 2;
	public Transform bottleObject;
	public Transform appleObject;
    public bool foundObject = false;
    public bool grabbingObject = false;
	public float distanceToObject = 0;
	public float objectRelativeVerticalPosition = 0;
	public float objectRelativeHorizontalPosition = 0;
	public float relativeAngle = 0;

	//bool shouldPython=true;

	float bottleRatio = 31.17691f*150*1024/36*1.1f; // focal length*object size*image size/sensor size * correctorValue. All in mm
	float appleRatio = 31.17691f*50*1024/36*1.1f; // focal length*object size*image size/sensor size. All in mm
	int bottleSize = 136;
	int appleSize = 50;
	int size;
	float ratio;
	Transform objectTaken;

	float cameraVerticalPosition = 0.35f;
	float armLength = 0.5f;

	Coroutine lastRoutine = null;
	bool isRotating;
	bool isMoving;
	
	// Update is called once per frame
	void Update () 
	{
		if (command!="")
		{
			switch(command)
			{
				case "move":
					visionMode=1;
					shouldSearchTarget=true;
					break;
				case "stop":
					shouldMove=false;
					shouldSearchTarget=false;
					shouldGrabTarget = false;

					break;
				case "take bottle":
				case "bottle":
					objectToTake="bottle";
					shouldSearchTarget=true;
					shouldGrabTarget=false;
					shouldMove = false;
					visionMode=2;
					Debug.Log("BottleMode");
				break;
				case "take apple":
				case "apple":

					objectToTake="apple";
					shouldSearchTarget=true;
					shouldGrabTarget=false;
					shouldMove = false;
					visionMode=2;
					Debug.Log("AppleMode");
				break;
			}
			command="";
		}
		if(foundObject && !grabbingObject)
		{
			//Debug.Log("Found");
			//Debug.Log((Mathf.Pow(distanceToObject-arm.armOffset.x,2) + Mathf.Pow(cameraVerticalPosition+objectRelativeVerticalPosition-arm.armOffset.y,2))+","+Mathf.Pow(armLength,2)+"/"+Mathf.Abs(objectRelativeHorizontalPosition));
			if(Mathf.Pow(distanceToObject-arm.armOffset.x,2) + Mathf.Pow(cameraVerticalPosition+objectRelativeVerticalPosition-arm.armOffset.y,2) <= Mathf.Pow(armLength,2) && Mathf.Abs(objectRelativeHorizontalPosition)<0.03)
			{
				//Debug.Log("Grabbing");
				shouldGrabTarget = true;
				shouldMove = false;
				StopCoroutine(lastRoutine);
			}
			else
			{
				shouldMove	= true;
				relativeAngle = 57.2958f*Mathf.Atan2(objectRelativeHorizontalPosition, distanceToObject);
				//StartCoroutine(move(distanceToObject));
				if(Mathf.Abs(objectRelativeHorizontalPosition)>0.03)
				{
					if(!isRotating && !isMoving)
						lastRoutine = StartCoroutine(rotate(relativeAngle));
				}
				else
				{
					if(!isMoving)
						lastRoutine = StartCoroutine(move());
				}
				//Debug.Log("Angle is "+relativeAngle);

			}
				
		}
		if(shouldSearchTarget && !foundObject)
		{
			//Debug.Log("Searching");
			shouldMove=false;
			string coordinates = PythonExecuter.values[1];
			string[] items = coordinates.Split(',');

			/*Debug.Log(coordinates);
			Debug.Log(items.Length);
			for(int cnt=0; cnt < items.Length;cnt++)
			{
				Debug.Log(cnt+","+items[cnt]);
			}*/
			int height = 0;
			int yBottom;
			if((items.Length-1)%9==0)
			{
				bool found=false;
				int i = 0;
				while(!found && i<items.Length)
				{
					string foundName=items[i];
					if(foundName=="sports_ball")
						foundName="apple";
					if(foundName==objectToTake)
						found=true;
						i+=9;
				}
				i-=9;
				if(found)
				{

					switch(objectToTake	)
					{
						case "bottle":
							size = bottleSize;
							ratio = bottleRatio;
							objectTaken	= bottleObject;
						break;
						case "apple":
							size = appleSize;
							ratio = appleRatio;
							objectTaken	= appleObject;
						break;
						default:
							size = bottleSize;
							ratio = bottleRatio;
							objectTaken	= bottleObject;
							break;
					}
					int ytop = (int)float.Parse(items[i+2]);
					yBottom = (int)float.Parse(items[i+8]);
					int right = (int)float.Parse(items[i+7]);
					int left = (int)float.Parse(items[i+1]);
					foundObject = true;
					shouldSearchTarget	= false;
					grabbingObject	= false;

					height = yBottom-ytop;
					distanceToObject=ratio/height;
					distanceToObject/=1000f;//mm to meters
					objectRelativeVerticalPosition = -size*ytop/height - size;
					objectRelativeVerticalPosition/=1000f;//mm to meters
					objectRelativeHorizontalPosition = size*((right+left)/2)/height;
					objectRelativeHorizontalPosition/=1000f;//mm to meters
					Debug.Log(height+", "+distanceToObject+", "+objectRelativeHorizontalPosition+", "+objectRelativeVerticalPosition);
					PythonExecuter.values[1] = "";

				}
			}

			if(!PythonExecuter.threads[1] && !foundObject)
			{
				//Debug.LogWarning("Getting new image results");
				if (visionMode==1)
					StartCoroutine(PythonExecuter.ExecuteScript("cam_track_object_NOSERVER.py",1));
				else
					StartCoroutine(PythonExecuter.ExecuteScript("object_DetectRecog.py",1));
			}	
		}
		

		if(shouldGrabTarget)
		{
				grabbingObject = true;
				arm.target = objectTaken;//Only for animation purposes
				arm.objectDistance = distanceToObject;
				arm.objectHeight = cameraVerticalPosition+objectRelativeVerticalPosition;
				Debug.Log(arm.objectHeight);
				StartCoroutine(arm.openClamp()); 
				shouldGrabTarget=false;
			
		}

		if(speech.word != "")
		{
			command=speech.word;
			speech.word="";
		}
	}
	IEnumerator move()
	{
		isMoving = true;
		//Debug.Log("moving for real");
		float current = 0;
		float distance = distanceToObject;
		float value = 0;
		while(current < distance && shouldMove)
		{
			value = Time.deltaTime*movementSpeed;
			transform.Translate(transform.forward*value);
			current	+= value;
			distanceToObject -= value;
			objectRelativeHorizontalPosition -= Mathf.Tan(relativeAngle/57.2958f)*value;
			if(current > distance/3)
			{
				shouldMove=false;
				foundObject=false;
				shouldSearchTarget=true;
				Debug.Log("Stoppping, searching again");
			}
			yield return null;
		}
		isMoving = false;
	}
	IEnumerator rotate(float angle)
	{
		isRotating = true;
		float value= 0;
		float originalAngle = transform.eulerAngles.y;
		float angleDifference = 0;
		if(Mathf.Abs(angle)>1.0f)
		{
			if(angle >0)
			{
				while(angleDifference < angle)
				{
					value = Time.deltaTime*rotationSpeed;
					transform.Rotate(0,value,0);
					angleDifference = transform.eulerAngles.y - originalAngle;
					if(angleDifference>180)
						angleDifference-=360;
					else if(angleDifference < -180)
						angleDifference+=360;
					//Debug.Log("Rotation "+ angleDifference+ "   "+angle);
					yield return null;
				}
			}
			else if(angle < 0)
			{
				while(angleDifference > angle)
				{
					value=-Time.deltaTime*rotationSpeed;
					transform.Rotate(0,value,0, Space.World);
					angleDifference = transform.eulerAngles.y - originalAngle;
					if(angleDifference>180)
						angleDifference-=360;
					else if(angleDifference < -180)
						angleDifference+=360;
					//Debug.Log("Rotation "+ angleDifference+ "   "+angle);
					yield return null;

				}
			}
			//Debug.Log("Starting to move");	
			lastRoutine = StartCoroutine(move());
			isRotating = false;
		}
	}
}
