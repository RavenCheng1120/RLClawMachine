using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawTouchPrize : MonoBehaviour
{
    public GameObject claw;
    ClawAgent clawAgent;

    // Start is called before the first frame update
    void Start()
    {
        clawAgent = claw.GetComponent<ClawAgent>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "goal")
        {
            clawAgent.TouchThePrize();
        }
    }
}
