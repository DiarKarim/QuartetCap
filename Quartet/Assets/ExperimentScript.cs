using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class Data
{
    public List<string> objectName = new List<string>();
    public List<float> xPos = new List<float>();
    public List<float> yPos = new List<float>();
    public List<float> zPos = new List<float>();
    public List<float> xRot = new List<float>();
    public List<float> yRot = new List<float>();
    public List<float> zRot = new List<float>();
    public List<float> time = new List<float>();

    public Data()
    {
        objectName = new List<string>();
        xPos = new List<float>();
        yPos = new List<float>();
        zPos = new List<float>();
        xRot = new List<float>();
        yRot = new List<float>();
        zRot = new List<float>();
        time = new List<float>();
    }

}

public class ExperimentScript : MonoBehaviour
{

    private string path;

    public InputField pathField;
    public InputField trialDurationField;
    public InputField trialName;
    public Text infoText;
    public TMP_Dropdown dropd_setuptype;

    Data data = new Data();

    Coroutine experimentSeq;

    SoundRecorder soundRecorder;

    private int recordingDuration = 1;

    public Transform[] objectsToTrack;
    bool recording;
    float startTime;
    public bool startRecording;
    int trialCounter = 0;


    void Start()
    {
        soundRecorder = FindObjectOfType<SoundRecorder>();
        StartCoroutine(DataCollector());
        //path = Application.persistentDataPath;

        if (string.IsNullOrEmpty(pathField.text))
        {
            path = Application.persistentDataPath;
            pathField.text = path;
        }

        dropd_setuptype.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(dropd_setuptype);
        });
    }

    void DropdownValueChanged(TMP_Dropdown m_Dropdown)
    {
        if (m_Dropdown.value == 1) // Single Player Without Skeleton
        {
            DeactivateAllObjects(objectsToTrack);
            objectsToTrack[0].gameObject.SetActive(true); // Violin 1 
            objectsToTrack[4].gameObject.SetActive(true); // Bow 1
        }
        if (m_Dropdown.value == 2) // Single Player With Skeleton
        {
            DeactivateAllObjects(objectsToTrack);
            objectsToTrack[0].gameObject.SetActive(true); // Violin 1 
            objectsToTrack[4].gameObject.SetActive(true); // Bow 1
            objectsToTrack[8].gameObject.SetActive(true); // Skeleton Marker 1
            objectsToTrack[9].gameObject.SetActive(true); // Skeleton Marker 2
            objectsToTrack[10].gameObject.SetActive(true); // Skeleton Marker 3
            objectsToTrack[11].gameObject.SetActive(true); // Skeleton Marker 4
            objectsToTrack[12].gameObject.SetActive(true); // Skeleton Marker 5
            objectsToTrack[13].gameObject.SetActive(true); // Skeleton Marker 6
            objectsToTrack[14].gameObject.SetActive(true); // Skeleton Marker 7
            objectsToTrack[15].gameObject.SetActive(true); // Skeleton Marker 8
            objectsToTrack[16].gameObject.SetActive(true); // Skeleton Marker 9
        }
        if (m_Dropdown.value == 3) // Quartet without skeleton
        {
            DeactivateAllObjects(objectsToTrack);
            objectsToTrack[0].gameObject.SetActive(true); // Violin 1 
            objectsToTrack[1].gameObject.SetActive(true); // Violin 2
            objectsToTrack[2].gameObject.SetActive(true); // Violin 3
            objectsToTrack[3].gameObject.SetActive(true); // Violin 4

            objectsToTrack[4].gameObject.SetActive(true); // Bow 1
            objectsToTrack[5].gameObject.SetActive(true); // Bow 2
            objectsToTrack[6].gameObject.SetActive(true); // Bow 3
            objectsToTrack[7].gameObject.SetActive(true); // Bow 4
        }
    }

    void DeactivateAllObjects(Transform[] objectsOfInterest)
    {
        foreach (Transform objay in objectsOfInterest)
        {
            objay.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S) | startRecording)
        if (startRecording)
        {
            if (string.IsNullOrEmpty(trialDurationField.text))
                infoText.text = "Please type in a trial duration.";
            else
                recordingDuration = int.Parse(trialDurationField.text);

            if (experimentSeq != null)
                StopCoroutine(experimentSeq);
            experimentSeq = StartCoroutine(ExperimentSequence());

            startRecording = false;
        }

    }

    IEnumerator DataCollector()
    {
        while (true)
        {
            if (recording)
            {
                foreach (Transform trf in objectsToTrack)
                {
                    if (trf.gameObject.activeInHierarchy == true)
                    {
                        data.objectName.Add(trf.gameObject.name);
                        data.xPos.Add(trf.position.x);
                        data.yPos.Add(trf.position.y);
                        data.zPos.Add(trf.position.z);
                        data.xRot.Add(trf.eulerAngles.x);
                        data.yRot.Add(trf.eulerAngles.y);
                        data.zRot.Add(trf.eulerAngles.z);
                        data.time.Add(Time.time - startTime);
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator ExperimentSequence()
    {
        soundRecorder.RecordAudio(recordingDuration);
        recording = true;
        //Debug.Log("Recording...");
        startTime = Time.time;

        while (Time.time - startTime < recordingDuration) //(Microphone.IsRecording(null))
        {
            //print("Timer: " + (Time.time - startTime).ToString());
            infoText.text = "Test No.: " + trialCounter.ToString() + " Timer: " + (recordingDuration - (Time.time - startTime)).ToString();

            yield return null;
        }

        soundRecorder.StopAudioRecording();
        //Debug.Log("Stopped recording...");
        recording = false;

        if (string.IsNullOrEmpty(pathField.text))
        {
            path = Application.persistentDataPath;
            pathField.text = path;
        }
        string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
        //string fileName = Time.time.ToString() + "_" + trialName.text;

        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        string fileName = cur_time.ToString() + "_" + trialName.text;

        soundRecorder.SaveAudio(path + "/" + fileName);
        File.WriteAllText(path + "/" + fileName + ".json", jsonString);

        infoText.text = "Data saved!!!";
        yield return null;
    }

    public void ButtonStart()
    {
        startRecording = true;
    }

    //private void OnGUI()
    //{
    //    //Case the 'Record' button gets pressed  
    //    if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Record"))
    //    {
    //        startRecording = true;
    //    }
    //}

}
