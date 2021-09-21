using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundRecorder : MonoBehaviour
{
    public int recordDuration = 5; 

    // Boolean flags shows if the microphone is connected 
    private bool micConnected = false;

    //The maximum and minimum available recording frequencies  
    private int minFreq;
    private int maxFreq;

    //A handle to the attached AudioSource  
    private AudioSource goAudioSource;

    //ExperimentScript experimentScript;

    void Start()
    {

        //experimentScript = FindObjectOfType<ExperimentScript>(); 

        //Debug.Log("My devices: " + Microphone.devices.Length);
        //int i = 0; 
        //foreach (var device in Microphone.devices)
        //{
        //    Debug.Log("Name: " + i + " " + device);
        //    i++;
        //}

        //Check if there is at least one microphone connected  
        if (Microphone.devices.Length <= 0)
        {
            //Throw a warning message at the console if there isn't  
            Debug.LogWarning("Microphone not connected!");
        }
        else //At least one microphone is present  
        {
            //Set our flag 'micConnected' to true  
            micConnected = true;

            //Get the default microphone recording capabilities  
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (minFreq == 0 && maxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate  
                maxFreq = 44100;
            }

            //Get the attached AudioSource component  
            goAudioSource = this.GetComponent<AudioSource>();
        }
    }

    void OnGUI()
    {
        //If there is a microphone  
        if (micConnected)
        {
            //If the audio from any microphone isn't being captured  
            if (!Microphone.IsRecording(null))
            {
                ////Case the 'Record' button gets pressed  
                //if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Record"))
                //{
                //    //Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource  
                //    goAudioSource.clip = Microphone.Start(Microphone.devices[0], true, recordDuration, maxFreq);

                //    //experimentScript.startRecording = true; 
                //}
            }
            else //Recording is in progress  
            {
                //Case the 'Stop and Play' button gets pressed  
                if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Stop and Play!"))
                {
                    Microphone.End(null); //Stop the audio recording  
                    goAudioSource.Play(); //Playback the recorded audio  
                }

                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 25, 200, 50), "Recording in progress...");
            }
        }
        else // No microphone  
        {
            //Print a red "Microphone not connected!" message at the center of the screen  
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Microphone not connected!");
        }

    }


    public void RecordAudio(int duration)
    {
        goAudioSource.clip = Microphone.Start(Microphone.devices[0], true, duration, maxFreq);
    }
    public void StopAudioRecording()
    {
        Microphone.End(null);
    }

    public void SaveAudio(string pathFile)
    {

        //string fileName = Time.time.ToString(); 
        //SavWav.Save(Application.persistentDataPath + "/" + fileName, goAudioSource.clip);
        SavWav.Save(pathFile, goAudioSource.clip);
    }
}