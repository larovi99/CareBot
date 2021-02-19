using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmInverseKinematics : MonoBehaviour {

    public Transform target;
    public Vector2 armOffset = new Vector2(0.03016952f,0.1671556f);
    public Transform servoMotorShoulder;
    public Transform servoMotorElbow;
    public Transform servoMotorWrist;
    public Transform servoMotorClamp;
    public Transform rightClamp;
    public Transform leftClamp;
    public Transform highestPoint;
    public float speed = 0.2f;
    public float clampSpeed=0.01f;
    float length1=0.20f;
    float length2=0.20f;
    float clampLength = 0.15f;
    public float objectDistance = 0.62f;
    public float objectHeight = 0.28f;
    float angle2;
    float angle1;
    float h;
    float distance;


	// Use this for initialization
	void Start () 
    {
       //StartCoroutine("openClamp"); 
	   //StartCoroutine("moveToObject");	
	}
	// Update is called once per frame
	IEnumerator moveToObject(bool shouldClose) 
    {
        //Debug.Log("Moving to object("+shouldClose+")");
        bool flag=false;
        while (!flag)
        {
            
            distance = objectDistance-clampLength-armOffset.y;
            h = Mathf.Sqrt(Mathf.Pow(distance,2)+Mathf.Pow(objectHeight-armOffset.x,2));
            Debug.Log("height`: "+objectHeight+", "+(objectHeight-armOffset.x));
            //Debug.Log("h: " + h);
            angle2 = Mathf.Acos((Mathf.Pow(h,2)-Mathf.Pow(length1,2)-Mathf.Pow(length2,2))/(2*length1*length2));
            angle1 = Mathf.Atan2(objectHeight,distance)-Mathf.Atan2(length2*Mathf.Sin(angle2),length1+length2*Mathf.Cos(angle2));
            angle2 *= -57.2958f;
            angle1 *= -57.2958f;
            
            servoMotorShoulder.localRotation = Quaternion.Slerp(servoMotorShoulder.localRotation,Quaternion.Euler(angle1,0,0),speed*Time.deltaTime);
            servoMotorElbow.localRotation = Quaternion.Slerp(servoMotorElbow.localRotation,Quaternion.Euler(angle2,0,0),speed*Time.deltaTime);
            servoMotorWrist.localRotation = Quaternion.Slerp(servoMotorWrist.localRotation,Quaternion.Euler(-(angle1+angle2),0,0),speed*Time.deltaTime);
            float difference1 = Mathf.Abs(servoMotorShoulder.localEulerAngles.x-angle1);
            if(difference1>180)
                difference1-=360;
            float difference2 = Mathf.Abs(servoMotorElbow.localEulerAngles.x-angle2);
            if(difference2>180)
                difference2-=360;
            
            float maxDifference;
            maxDifference = Mathf.Max(difference1,difference2);
            if (maxDifference < 4.0f)
                flag = true;

            yield return null;
        }
        if(shouldClose)
            StartCoroutine("closeClamp");
        
        //wait(100);
        //servoMotorClamp.write(0);
	}
    public IEnumerator openClamp()
    {
        float step = 0.0f;
        while(step < 0.00055f)
        {
            rightClamp.localPosition = new Vector3(rightClamp.localPosition.x + Time.deltaTime*clampSpeed, rightClamp.localPosition.y,rightClamp.localPosition.z);
            leftClamp.localPosition = new Vector3(leftClamp.localPosition.x - Time.deltaTime*clampSpeed, leftClamp.localPosition.y, leftClamp.localPosition.z);
            servoMotorClamp.Rotate(0,0,-Time.deltaTime*60, Space.Self);
            step += Time.deltaTime*clampSpeed;
            yield return null;
        }
        StartCoroutine(moveToObject(true));

    }
    IEnumerator closeClamp()
    {
        float step = 0.0f;
        while(step < 0.00055f)
        {
            rightClamp.localPosition = new Vector3(rightClamp.localPosition.x - Time.deltaTime*clampSpeed, rightClamp.localPosition.y,rightClamp.localPosition.z);
            leftClamp.localPosition = new Vector3(leftClamp.localPosition.x + Time.deltaTime*clampSpeed, leftClamp.localPosition.y, leftClamp.localPosition.z);
            servoMotorClamp.Rotate(0,0,Time.deltaTime*60, Space.Self);
            step += Time.deltaTime*clampSpeed;
            yield return null;
        }
        target.parent=servoMotorClamp;
        //target=highestPoint;
        objectDistance = highestPoint.localPosition.z;
        objectHeight = highestPoint.localPosition.y;
        StartCoroutine(moveToObject(false));

    }
}

