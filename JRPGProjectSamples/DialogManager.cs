using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
   
    public TMP_Text nameText;
    public TMP_Text dialogText;

    public GameObject[] walls;

    public float letterSpeed = 0.05f;
    public GameObject finaleIllustation;
    public bool playCombat1 = false;
    public bool playCombat2 = false;
    public bool playCombat3 = false;

    public int currentCombat = 0;

    public GameObject dialogUI;

    public GameObject[] pnjs;

    public Queue<string> sentences;

    public SimpleBlit _simpleBlit;
    public bool combatAlreadyLauched = false;

    private void Awake()
    {
        instance = this;
        sentences = new Queue<string>();
        dialogUI.SetActive(false);
    }
    public void StartDialog(Dialog dialog, GameObject pnj)
    {
        ZoomCamera.instance.zoomActive = true;
        if (pnj == pnjs[0])
            playCombat1 = true;
        if (pnj == pnjs[1])
            playCombat2 = true;
        if (pnj == pnjs[2])
            playCombat3 = true;

        PlayerMovement.instance.moveSpeed = 0;
        PlayerMovement.instance.animator.enabled = false;
        nameText.text = dialog.name;
        dialogUI.SetActive(true);
        sentences.Clear();

        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public LaunchFight(bool whichFight, int sentencesLeft, int currentCombat)
    {
        if (whichFight)
        {
            if (sentences.Count == sentencesLeft)
            {
                currentCombat = currentCombat;
                EndDialogWithCombat();
                whichFight = false;
                return;
            }
        }
    }
    public void DisplayNextSentence()
    {
        LaunchFight(playCombat1, 3, 1);
        LaunchFight(playCombat2, 2, 2);
        LaunchFight(playCombat3, 3, 3);

        if (sentences.Count == 0)
        {
            combatAlreadyLauched = false;
            EndDialog();
            if (currentCombat == 3)
            {
                finaleIllustation.SetActive(true);
                StartCoroutine(GoToCredits());
            }
            return;
        }
        
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(letterSpeed);
        }
    }
    IEnumerator GoToCredits()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Credits");
    }

    public void EndDialog()
    {
        ZoomCamera.instance.zoomActive = false;
        dialogUI.SetActive(false);
        PlayerMovement.instance.moveSpeed = PlayerMovement.instance.initMoveSpeed;
        PlayerMovement.instance.animator.enabled = true;
    }
    public void EndDialogWithCombat()
    {
        if (playCombat1)
            StartCoroutine(WaitOneFrame(2, "Fight1"));
        if (playCombat2)
            StartCoroutine(WaitOneFrame(2, "Fight2"));
        if (playCombat3)
            StartCoroutine(WaitOneFrame(2, "Fight3"));

        IEnumerator WaitOneFrame(float timeToWait, string scene)
        {
            _simpleBlit.transitionIsActive = true;
            yield return new WaitForSeconds(timeToWait);
            _simpleBlit.cutoffVal = 0f;
            _simpleBlit.TransitionMaterial.SetFloat("_Cutoff", _simpleBlit.cutoffVal);
            _simpleBlit.transitionIsActive = false;
            if(!combatAlreadyLauched)
            {
                AudioManager.instance.Play("FightLaunch");
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
                AudioManager.instance.Stop("Exploration");
                //AudioManager.instance.Play("Combat");
                combatAlreadyLauched = true;
            }
        }
    }

    private void Start()
    {
        _simpleBlit.cutoffVal = 0f;
        _simpleBlit.TransitionMaterial.SetFloat("_Cutoff", _simpleBlit.cutoffVal);
        _simpleBlit.transitionIsActive = false;
    }
}
