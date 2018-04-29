using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using System;



public class stream_open : MonoBehaviour {
	public static SerialPort stream = new SerialPort("COM4", 15200);

	// Use this for initialization
	void Start () {
		stream.Open ();
		stream.ReadTimeout = 50;

	}


	
	// Update is called once per frame
	void Update () {
		StartCoroutine(AsyncReadFromArduino((string s) => Debug.Log(s), () => Debug.LogError("Error"), 10000f));
	}

	public IEnumerator AsyncReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity){
		DateTime initialTime = DateTime.Now;
		DateTime nowTime;
		TimeSpan diff = default(TimeSpan);

		string dataString = null;

		do{
			try{
				dataString = stream.ReadLine();
			}
			catch(TimeoutException){
				dataString = null;
			}

			if(dataString != null){
				callback(dataString);
				yield return null;
			}else
			{
				yield return new WaitForSeconds(0.05F);
				nowTime = DateTime.Now;
				diff = nowTime - initialTime;
			}
			}while (diff.Milliseconds < timeout);

			if(fail != null){
				fail();
				yield return null;
			}
	}
	public void WriteToArduino(string message){
		stream.WriteLine(message);
		stream.BaseStream.Flush();
	}
}

