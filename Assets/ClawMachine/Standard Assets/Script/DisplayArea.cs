using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayArea : MonoBehaviour
{
    public Text ScoreText;
    public Text StepText;
    public GameObject claw;
    ClawAgent clawAgent;

    // Start is called before the first frame update
    void Start()
    {
        clawAgent = claw.GetComponent<ClawAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = clawAgent.GetCumulativeReward().ToString("0.000");
        StepText.text = clawAgent.GetStepCount().ToString();
    }
}
