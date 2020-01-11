using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAgents;

public class PyramidAgent : Agent
{
    public GameObject area; // AreaPB
    public GameObject areaSwitch; //Switch按鈕
    public bool useVectorObs;
    Rigidbody m_AgentRb;
    RayPerception m_RayPer;
    // 用PyramidArea.cs和PyramidSwitch.cs建立物件
    PyramidArea m_MyArea;
    PyramidSwitch m_SwitchLogic;

    // 取得變數
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<PyramidArea>();
        m_RayPer = GetComponent<RayPerception>();
        m_SwitchLogic = areaSwitch.GetComponent<PyramidSwitch>();
    }

    // 使用RayPerception(射線)偵測物體
    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            // 三組射線角度
            const float rayDistance = 35f;
            float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
            float[] rayAngles1 = { 25f, 95f, 165f, 50f, 140f, 75f, 115f };
            float[] rayAngles2 = { 15f, 85f, 155f, 40f, 130f, 65f, 105f };

            // 需要偵測的物件tag
            string[] detectableObjects = { "block", "wall", "goal", "switchOff", "switchOn", "stone" };
            AddVectorObs(m_RayPer.Perceive(rayDistance, rayAngles, detectableObjects));
            AddVectorObs(m_RayPer.Perceive(rayDistance, rayAngles1, detectableObjects, 0f, 5f));
            AddVectorObs(m_RayPer.Perceive(rayDistance, rayAngles2, detectableObjects, 0f, 10f));
            AddVectorObs(m_SwitchLogic.GetState());
            AddVectorObs(transform.InverseTransformDirection(m_AgentRb.velocity));
        }
    }

    // agent移動
    public void MoveAgent(float[] act)
    {
        // 方向與旋轉(x, y, z)
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(act[0]);
        switch (action)
        {
            // transform.up, transform.right, transform.forward對應xyz三個座標軸，transform的屬性是物體自身座標軸
            // 前進
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            // 後退
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            // 左轉
            case 3:
                rotateDir = transform.up * 1f;
                break;
            // 右轉
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void AgentAction(float[] vectorAction)
    {
        // 每走一步，reward會減少-1/5000
        AddReward(-1f / agentParameters.maxStep);
        MoveAgent(vectorAction);
    }

    // 輸出action
    public override float[] Heuristic()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return new float[] { 3 };
        }
        if (Input.GetKey(KeyCode.W))
        {
            return new float[] { 1 };
        }
        if (Input.GetKey(KeyCode.A))
        {
            return new float[] { 4 };
        }
        if (Input.GetKey(KeyCode.S))
        {
            return new float[] { 2 };
        }
        return new float[] { 0 };
    }

    public override void AgentReset()
    {
        var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(9);
        var items = enumerable.ToArray(); //從 IEnumerable<T> 建立陣列。

        m_MyArea.CleanPyramidArea();

        m_AgentRb.velocity = Vector3.zero;
        m_MyArea.PlaceObject(gameObject, items[0]); //隨機放置agnet
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        // 隨機挑選地區建立金字塔和按鈕
        m_SwitchLogic.ResetSwitch(items[1], items[2]); // items[1]是按鈕位置，items[2]是目標金字塔位置
        m_MyArea.CreateStonePyramid(1, items[3]);
        m_MyArea.CreateStonePyramid(1, items[4]);
        m_MyArea.CreateStonePyramid(1, items[5]);
        m_MyArea.CreateStonePyramid(1, items[6]);
        m_MyArea.CreateStonePyramid(1, items[7]);
        m_MyArea.CreateStonePyramid(1, items[8]);
    }

    // 碰到金字塔上的目標物，reward增加2
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            SetReward(2f);
            Done();
        }
    }

    public override void AgentOnDone()
    {
    }
}
