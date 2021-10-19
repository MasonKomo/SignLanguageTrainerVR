using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;



// Struct for Gestures
[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour { 

    // Handles Scene Changing.
    public void ChanceScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    // Booleans to keep track of what mode is currently active.
    [SerializeField]
    public Boolean mainMenu;
    public Boolean learnMode;
    public Boolean practiceMode;
    public Boolean freestyleMode;
    public bool debugMode = true;

    // Reference to Gesture detector for right hand.
    public GestureDetector RightHandDetector;

    int k = 0; // USED TO TRACK CURRENT GIVEN LETTER IN PRACTICE MODE

    // How accurate the gesture recognition should be.
    [Header("Threshold value")]
    public float threshold = 0.1f;
    
    // References for OVR the skeleton hand ("OVRCustomHandPrefab_R" or "OVRCustomHandPrefab_L")
    [Header("Hand Skeleton")]
    public OVRSkeleton skeleton;
    public OVRHand hand;

    // List to store gestures.
    [Header("List of Gestures")]
    public List<Gesture> gestures;
    private Gesture previousGesture;

    // List of bones took from the OVRSkeleton
    private List<OVRBone> fingerbones = null;

    // Other boolean to check if Oculus VR initializations are done. 
    private bool hasStarted = false;
    private bool hasRecognize = false;
    private bool done = false;


    // Letter variables
    public List<GameObject> Letters;
    public Transform letterSpawn; // The letter that the user is signing.
    public Transform signGO; // Used to determine position of learn mode prompts.
    public Transform givenLetterSpawn; // The letter that is given by the game.
    public GameObject LetterGO;
    public List<char> currentLetter;
    public int letterIndex;

    // Letter Materials
    public Material inputLetterMaterial;
    public Material givenLetterMaterial;

    // Event for when gesture isn't reconized (debugging)
    [Header("Not Recognized Event")]
    public UnityEvent notRecognize;

    void Start()
    {
        // Delay to ensure all Oculus initializeation finish before application begins.
        StartCoroutine(DelayRoutine(2.5f, Initialize));

        // Get reference to OVRHand component.
        hand = skeleton.GetComponent<OVRHand>();
    }

    // Coroutine used for delay.
    public IEnumerator DelayRoutine(float delay, Action actionToDo)
    {
        yield return new WaitForSeconds(delay);
        actionToDo.Invoke();
    }

    public void Initialize()
    {
        // Used to populate skeleton.
        SetSkeleton();

        // Boolean to know when initializations are complete.
        hasStarted = true;
    }
    public void SetSkeleton()
    {
        // Populate the private list of fingerbones from the current hand in OVR skeleton
        fingerbones = new List<OVRBone>(skeleton.Bones);
    }

    // Called once per frame in game.
    void Update()
    {
        // Check for gesture to return to main menu if not currently at main menu.
        if (!mainMenu)
        {
            if (OVRInput.Get(OVRInput.Button.Start))
            {
                ChanceScene("MainMenu");
            }
        }

        // Save gesture if in debug mode and spacebar is pressed.
        if (debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            fingerbones = new List<OVRBone>(skeleton.Bones);
            Save();
        }

        //if the initialization was successful
        if (hasStarted.Equals(true))
        {
            // Check if gesture being made is recognized.
            Gesture currentGesture = Recognize();

            // associate gesture to boolean.
            hasRecognize = !currentGesture.Equals(new Gesture());

            // If gesture is recognized:
            if (hasRecognize)
            {
                // boolean used to avoid looping while hold a recognized gesture.
                done = true;

                // Debug output.
                Debug.Log("New Gesture Found: " + currentGesture.name);
                


                // [FREESTYLE MODE]
                if (freestyleMode) {
                    int i = 0; // used to track the current LetterGameObject (LetterGO)

                    // Associate LetterGO variable with the current Letter GameObject.
                    LetterGO = Letters[i].gameObject;

                    // Check if current gesture is correct.
                    if (currentGesture.name == Letters[i].name)
                    {
                        // If so, progress the letters list.
                        i++;
                    }

                    // Whe n end of alphabetis reached.
                    if (i == 25)
                    {
                        // Reset back to letter 'A'
                        i = 0;
                    }
                    
                    // Below is conversion of char to int so that we can compare given and input gestures.
                    // Get current gesture name and convert it to integer.
                    char a = char.Parse(currentGesture.name);
                    int intVal = a - '0';
                    intVal = intVal - 17;

                    // Loop through all letters.
                    for (int j = 0; j < 25; j++)
                    {
                        if (j != intVal)
                        {
                            letterSpawn.GetChild(j).gameObject.SetActive(false); // Despawn letters.
                        }
                    }

                    letterSpawn.GetChild(intVal).gameObject.SetActive(true); // Spawn the correct letter.
                }



                // [PRACTICE MODE]
                if (practiceMode)
                {
                    int i = 0; // used to track the current LetterGameObject (LetterGO)

                    // Associate LetterGO variable with the current Letter GameObject.
                    LetterGO = Letters[i].gameObject;

                    // Check if current gesture is correct.
                    if (currentGesture.name == Letters[i].name)
                    {
                        // If so, progress the letters list.
                        i++;
                    }

                    // When end of alphabet is reached.
                    if (i == 25)
                    {
                        // Reset back to letter 'A'
                        i = 0;
                    }
                    //letterSpawn.GetChild(i).gameObject.SetActive(true);

                    // Below is conversion of char to int so that we can compare given and input gestures.
                    // Get current gesture name and convert it to integer.
                    char a = char.Parse(currentGesture.name);
                    int intVal = a - '0';
                    intVal = intVal - 17;

                    // Loop through all letters.
                    for (int j = 0; j < 25; j++)
                    {
                        if (j != intVal)
                        {
                            letterSpawn.GetChild(j).gameObject.SetActive(false); // Despawn letters.
                        }
                    }

                    // Set the current letter active on screen.
                    letterSpawn.GetChild(intVal).gameObject.SetActive(true);

                    // Change color of letter to indicate correct.
                    if (currentGesture.name == givenLetterSpawn.GetChild(k).gameObject.name)
                    {
                        givenLetterMaterial.DOColor(Color.green, 1f);

                        // Attempt at pausing for color change... Not currently working.
                        StartCoroutine(wait());
                        givenLetterSpawn.GetChild(k).gameObject.SetActive(false);
                        k++;
                        givenLetterSpawn.GetChild(k).gameObject.SetActive(true);
                    }
                }


                if (learnMode)
                {
                    int i = 0; // used to track the current LetterGameObject (LetterGO)

                    // Associate LetterGO variable with the current Letter GameObject.
                    LetterGO = Letters[i].gameObject;

                    // Check if current gesture is correct.
                    if (currentGesture.name == Letters[i].name)
                    {
                        // If so, progress the letters list.
                        i++;
                    }

                    // When end of alphabet is reached.
                    if (i == 25)
                    {
                        // Reset back to letter 'A'
                        i = 0;
                    }
                    //letterSpawn.GetChild(i).gameObject.SetActive(true);

                    // Below is conversion of char to int so that we can compare given and input gestures.
                    // Get current gesture name and convert it to integer.
                    char a = char.Parse(currentGesture.name);
                    int intVal = a - '0';
                    intVal = intVal - 17;

                    // Loop through all letters.
                    for (int j = 0; j < 25; j++)
                    {
                        if (j != intVal)
                        {
                            letterSpawn.GetChild(j).gameObject.SetActive(false); // Despawn letters.
                        }
                    }

                    // Set the current letter active on screen.
                    letterSpawn.GetChild(intVal).gameObject.SetActive(true);

                    // Change color of letter to indicate correct.
                    if (currentGesture.name == givenLetterSpawn.GetChild(k).gameObject.name)
                    {
                        givenLetterMaterial.DOColor(Color.green, 1f);

                        // Attempt at pausing for color change... Not currently working.
                        StartCoroutine(wait());

                        // Hide current letter and instruction.
                        givenLetterSpawn.GetChild(k).gameObject.SetActive(false);
                        signGO.GetChild(k).gameObject.SetActive(false);
                        k++;

                        // Display next letter and instruction.
                        givenLetterSpawn.GetChild(k).gameObject.SetActive(true);
                        signGO.GetChild(k).gameObject.SetActive(true);
                    }
                }

                previousGesture = currentGesture;
                currentGesture.onRecognized?.Invoke(); // Unity event for debug.
            }
            // Gesture is no longer recognized.
            else
            {
                if (done)
                {
                    Debug.Log("Not Recognized");
                    done = false;

                    // Unity event for debug.
                    notRecognize?.Invoke();

                    // Hide all letters.
                    for (int i = 0; i < 26; i++)
                    {
                        letterSpawn.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    void Save()
    {
        // We create a new Gesture struct
        Gesture g = new Gesture();
        g.name = "New Gesture";

        // list to hold finger data.
        List<Vector3> data = new List<Vector3>();

        // loop through all bones, setting position data.
        foreach (var bone in fingerbones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        // associate data with gesture variable.
        g.fingerDatas = data;

        // add gesture to list of gestures.
        gestures.Add(g);
    }

    public Gesture Recognize()
    {
        // Create new gesture.
        Gesture currentGesture = new Gesture();

        float currentMin = Mathf.Infinity;

        // Check each gesture in gestures list.
        for (int j = 0; j < gestures.Count; j++)
        {
            // float to store distance
            float sumDistance = 0;

            // bool to track if current gesture is discarded [not a match to known gestures].
            bool isDiscarded = false;

            // check the list of bones.
            for (int i = 0; i < fingerbones.Count; i++)
            {
                // convert current hand data from global position to local position. 
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerbones[i].Transform.position);

                // find distance between current gesture and all saved gestures.
                float distance = Vector3.Distance(currentData, gestures[j].fingerDatas[i]);

                // if the distance is larger than the threshold amount, discard the gesture.
                if (distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }

                // if the distance is correct, add it to the first sumDistance
                sumDistance += distance;
            }

            // if the gesture is within threshold and not larger than infinity (currentMin)
            if (!isDiscarded && sumDistance < currentMin)
            {
                // set currentMin to the distance.
                currentMin = sumDistance;

                // associate the gesture we are currently making with the correct gesture that we have just found from the list of gestures.
                currentGesture = gestures[j];
                letterIndex = j;
            }
        }

        // return the found gesture. (Hope it's right lol)...
        return currentGesture;
    }

    // Function to wait for UI events.
    public IEnumerator wait()
    {
        yield return new WaitForSeconds(1);

        givenLetterMaterial.DOColor(Color.red, 1f);
    }

    public IEnumerator waitfor5Seconds()
    {
        yield return new WaitForSeconds(5f);
    }
}
