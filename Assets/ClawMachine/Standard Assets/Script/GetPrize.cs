using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPrize : MonoBehaviour
{
    public GameObject claw;
    ClawAgent clawAgent;

    private void Start()
    {
        clawAgent = claw.GetComponent<ClawAgent>();
    }

    // 有獎品掉落洞口，與洞口平面閘門觸發Trigger，呼叫Agent script中的函式
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Enter:"+other.name);
        if (other.gameObject.tag=="goal")
        {
            Debug.Log("Get the prize");
            clawAgent.GetPrize_AddScore();
        }
    }
}
