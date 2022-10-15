using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using System.Threading; 

//using UnityEngine.Networking;

public class DataClassing
{
    [SerializeField] public string conditionInfo = string.Empty;
    [SerializeField] public string[] dataTable = new string[1001];
}

public class DataClass
{
    public List<int> frameNum = new List<int>();
    public List<string> gameObjectName = new List<string>();
    public List<float> xPos = new List<float>();
    public List<float> yPos = new List<float>();
    public List<float> zPos = new List<float>();
    public List<float> xRot = new List<float>();
    public List<float> yRot = new List<float>();
    public List<float> zRot = new List<float>();
    public List<string> targetID = new List<string>();
    public List<float> xTPos = new List<float>();
    public List<float> yTPos = new List<float>();
    public List<float> zTPos = new List<float>();
    public List<float> time = new List<float>();
}

public class Experiment : MonoBehaviour
{
    public Slider timerSlider;

    private Thread dataSaveThread; 

    private DataClassing expData = new DataClassing();
    private DataClass expTrialData = new DataClass();

    private float startTime = 0f;
    private bool startOfTrial = false;
    private int frameNum = 0;
    //private string conditionPost;
    public string userIDPost;
    private int trialnum = -1;
    private List<string> recordDataList = new List<string>();

    public GameObject startingPosition;
    private StartReach startReach;
    public Transform[] TrackedObjects;

    public float trialDuration = 5f;
    private int numberOfTrials;
    //public GameObject targetManager; 
    public Transform[] targets;
    public TargetHitScript[] targetHitScript;
    public Pause pauseScript;

    [SerializeField]
    private string path;

    string trialName;
    int trialNumber = -1;
    float currTime = 0f;
    bool recording;
    int randTargetIndex = 0;
    int[] targetIndicies;
    public float beatsPM; 
    public bool metro;
    public int reps = 4; 

    public AudioSource[] sounds;
    
    private void Awake()
    {
        path = Application.persistentDataPath;
    }

    void Start()
    {
        startReach = startingPosition.GetComponent<StartReach>();

        timerSlider.value = 1.0f;
        timerSlider.gameObject.SetActive(false);
        numberOfTrials = targets.Length * reps;

        int[] targ_indies_01 = Enumerable.Range(0, targets.Length).ToArray();
        int[] targ_indies_02 = Enumerable.Range(0, targets.Length).ToArray();
        int[] targ_indies_03 = Enumerable.Range(0, targets.Length).ToArray();
        int[] targ_indies_04 = Enumerable.Range(0, targets.Length).ToArray();

        int[] targIndiesAll = new int[targ_indies_01.Length + targ_indies_02.Length + targ_indies_03.Length + targ_indies_04.Length];
        targ_indies_01.CopyTo(targIndiesAll, 0);
        targ_indies_02.CopyTo(targIndiesAll, targ_indies_01.Length);
        targ_indies_03.CopyTo(targIndiesAll, targ_indies_01.Length+ targ_indies_01.Length);
        targ_indies_04.CopyTo(targIndiesAll, targ_indies_01.Length + targ_indies_01.Length + targ_indies_01.Length);

        //int[] tempIndicies = new int[targets.Length];
        //for(int i = 0; i < targets.Length; i++)
        //{
        //    tempIndicies[i] = i;
        //}
        //targetIndicies = new int[targIndiesAll.Length];
        targetIndicies = targ_indies_01;
        //targetIndicies = targIndiesAll;
        Shuffle(targetIndicies);

        for (int i = 0; i < targets.Length; i++)
        {
            //Debug.Log(i);
            targets[i].GetComponent<Collider>().enabled = false;
            //targets[i].GetComponent<MeshRenderer>().enabled = false;
            targetHitScript[i].isHit = false;
        }

        //Invoke("StartTrialNow", 5f);

        Init();
    }

    private void Init()
    {
        dataSaveThread = new Thread(new ThreadStart(RecordDataFunc));
        dataSaveThread.IsBackground = true;
        dataSaveThread.Start();
    }

    void StartTrialNow()
    {
        startOfTrial = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            StartTrialNow(); 
        }

        if (trialNumber < numberOfTrials & startOfTrial)
        {
            trialNumber++;
            StartCoroutine(TargetrecordingSequence());
            frameNum = 0;
            startOfTrial = false;
        }
        else
        {
            Debug.Log("Experiment has ended! TrialNum: " + trialNumber.ToString());
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(Metronome(100f, sounds[1]));
        }

        // Test file saving and data integrety
        if(Input.GetKeyDown(KeyCode.X))
        {
            startTime = Time.time; 
            recording = true; 
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            recording = false;
            StartCoroutine(Upload2());
        }
    }

    void RecordDataFunc()
    {
        // ... record data as fast as possible while waiting for participant to hit the target 
        if (recording)
        {
            currTime = Time.time - startTime;

            foreach (Transform trckdObj in TrackedObjects)
            {
                // Repeated numbers 
                expTrialData.time.Add(currTime);
                expTrialData.frameNum.Add(frameNum);
                expTrialData.targetID.Add(targets[randTargetIndex].gameObject.name);
                expTrialData.xTPos.Add(targets[randTargetIndex].position.x);
                expTrialData.yTPos.Add(targets[randTargetIndex].position.y);
                expTrialData.zTPos.Add(targets[randTargetIndex].position.z);

                // Frame-by-frame numbers 
                expTrialData.gameObjectName.Add(trckdObj.gameObject.name);
                expTrialData.xPos.Add(trckdObj.position.x);
                expTrialData.yPos.Add(trckdObj.position.y);
                expTrialData.zPos.Add(trckdObj.position.z);
                expTrialData.xRot.Add(trckdObj.eulerAngles.x);
                expTrialData.yRot.Add(trckdObj.eulerAngles.y);
                expTrialData.zRot.Add(trckdObj.eulerAngles.z);
            }
            frameNum++;
        }
    }

    IEnumerator TargetrecordingSequence()
    {
        //for (int i = 0; i < numberOfTrials; i++)
        //{
        // Make sure participant is in starting position
        while (!startReach.inStartPosition)
        {
            Debug.Log("Go to starting position!!!");
            yield return null;
        }

        sounds[0].Play(); // Start trial sound 
        metro = true;
        Coroutine metronoming = StartCoroutine(Metronome(beatsPM, sounds[1]));


        // Show random target and ...  
        //randTargetIndex = Random.Range(0, targets.Length);

        // Make sure we go through all the targets 
        int currTargetIndex = trialNumber;
        currTargetIndex = currTargetIndex % targets.Length; 
        randTargetIndex = targetIndicies[currTargetIndex];
        
        Debug.Log("Index: " + randTargetIndex);
        Debug.Log("Target: " + targets[randTargetIndex]);

        targets[randTargetIndex].GetComponent<Collider>().enabled = true;
        MeshRenderer targetRend = targets[randTargetIndex].GetComponent<MeshRenderer>();
        targetRend.material.color = new Color(1f, 0f, 0f);

        targetHitScript[randTargetIndex].isHit = false;

        recording = true;
        while (startReach.inStartPosition)
        {
            frameNum = 0;
            Debug.Log("Start to reach forward!!!");
            yield return null;
        }

        //frameNum = 0;
        startTime = Time.time;
        // ... record data while waiting for participant to hit the target 
        while (!targetHitScript[randTargetIndex].isHit)
        {
            //recording = true;
            yield return null;
        }
        //frameNum++;

        // Animate and delay indicator to make sure Quest hand tracking and mocap delays are accounted for
        float waitTime = 1.0f;
        timerSlider.transform.position = targets[randTargetIndex].position;
        timerSlider.gameObject.SetActive(true);
        timerSlider.value = 1.0f;
        Coroutine sliderAnim = StartCoroutine(HitSliderSequence(waitTime));

        yield return new WaitForSecondsRealtime(waitTime);
        timerSlider.gameObject.SetActive(false);

        sounds[1].Play();
        recording = false;
        metro = false; // Stop metronome sound 
        StopCoroutine(metronoming);

        yield return null;

        targets[randTargetIndex].GetComponent<Collider>().enabled = false;
        targetHitScript[randTargetIndex].isHit = false;

        trialName = userIDPost + "_Target_" +
                        targets[randTargetIndex].gameObject.name +
                        "_Trial_" + trialNumber + "_.json";

        // Record data and reset End-of-trial variables 
        StartCoroutine(Upload2());

        yield return new WaitForSecondsRealtime(0.15f);

        while (pauseScript.paused)
        {
            yield return null;
        }

        Debug.Log("End of trial!!!");

        // Waitime breather between trials 
        yield return new WaitForSecondsRealtime(1f);

        StopCoroutine(sliderAnim); 

        startOfTrial = true; // Start next trial 
        //}
    }

    IEnumerator HitSliderSequence(float waitTimer)
    {
        float waitSteps = 500f; 
        float currTime = waitTimer;
        float waitFraction = waitTimer/ waitSteps; 

        while(currTime > 0f)
        {
            timerSlider.value = currTime;
            currTime -= waitFraction;
            yield return new WaitForSecondsRealtime(waitFraction); 
        }
    }

    private IEnumerator Metronome(float bpm, AudioSource click)
    {
        float waitTime = 60.0f / bpm; 
    
        while(metro)
        {
            click.Play();
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private IEnumerator Upload2()
    {
        // Convert to json and send to another site on the server
        //expData.conditionInfo = userIDPost;
        //expData.dataTable = recordDataList.ToArray();

        //string jsonString = JsonConvert.SerializeObject(expData, Formatting.Indented);
        string jsonString = JsonConvert.SerializeObject(expTrialData, Formatting.Indented);

        // Save jsonString variable in a file 
        File.WriteAllText(path + "/" + trialName, jsonString);
        yield return new WaitForSecondsRealtime(1.5f);

        // Empty text fields for next trials (potential for issues with next trial)
        //recordDataList.Clear();
        expTrialData = new DataClass(); // Clear class 

        yield return null;
    }


    static System.Random _random = new System.Random();
    public static void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < (n - 1); i++)
        {
            int r = i + _random.Next(n - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }

}

