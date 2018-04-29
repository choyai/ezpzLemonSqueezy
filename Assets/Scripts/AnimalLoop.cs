using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using System;


public class AnimalLoop : MonoBehaviour {

    public static SerialPort stream = new SerialPort("COM5", 9600);



    // Use this for initialization
    void Start () {
        stream.ReadTimeout = 50;
        stream.Open();
    }
	
	// Update is called once per frame
	void Update () {
        StartCoroutine
            (
                    AsynchronousReadFromArduino
                    ((string s) => Debug.Log(s),     // Callback
                    () => Debug.LogError("Error!"), // Error callback
                    10000f                          // Timeout (milliseconds)
            )
        );
        WriteToArduino("yo");
    }


    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }
    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }
}
