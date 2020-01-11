using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ClawAgent : Agent
{
    // 設定外來變數
    public bool useVectorObs; //是否使用射線(舊)
    MovimientosClaw AgentSphere; //控制爪子的agent，由此呼叫MovimientosClaw.cs的函式
    public GameObject Claws; //爪子
    AreaReset m_MyArea; //場景stage，由此呼叫AreaReset.cs來控制物件重生
    public GameObject area; //stage

    // 設定內部變數
    RayPerception m_RayPer; //射線(舊)
    int prizeOnStage; //檯面上剩下多少獎品
    int numberOfPrize; //總共獎品數量
    Vector3 ClawOriginPosition; //爪子初始位置(3維)
    public GameObject[] Dice_static;//紀錄骰子物件


    // 初始化ml-agent
    public override void InitializeAgent()
    {
        base.InitializeAgent(); //Agent基礎初始化
        m_RayPer = GetComponent<RayPerception>();//舊版射線使用的宣告
        //useVectorObs = false; //關掉舊的射線
        numberOfPrize = 5; //從這裡可以調整獎品數量
        prizeOnStage = numberOfPrize;
        AgentSphere = GetComponent<MovimientosClaw>();
        m_MyArea = area.GetComponent<AreaReset>();
        ClawOriginPosition = new Vector3(0.1f, 13.4f, -1.1f);//爪子初始位置
        Dice_static = GameObject.FindGameObjectsWithTag("goal"); //取得所有骰子物件
    }

    //--------------------------觀察環境--------------------------//
    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            // 三組射線角度
            float[] rayAngles = { 0f, 30f, 60f, 90f, 120f, 150f, 180f, 210f, 240f, 270f, 300f, 330f };
            float[] rayAngles1 = { 15f, 45f, 75f, 105f, 135f, 165f, 195f, 225f, 255f, 285f, 315f, 345f };
            float[] rayAngles2 = { 5f, 25f, 45f, 65f, 85f, 100f };

            // 需要偵測的物件tag
            string[] detectableObjects = {"goal"};
            AddVectorObs(m_RayPer.Perceive(2.2f, rayAngles, detectableObjects, -3f, -6f));
            AddVectorObs(m_RayPer.Perceive(1.4f, rayAngles1, detectableObjects, -3.5f, -6.5f));
            AddVectorObs(m_RayPer.Perceive(4.7f, rayAngles2, detectableObjects, -3.5f, -6.5f));
        }
        AddVectorObs(gameObject.transform.position - Claws.transform.position); //觀察爪子跟控制圓球的相對位置
        AddVectorObs(AgentSphere.PuedeControlarse); //觀察是在否可以控制爪子的狀態
        foreach (GameObject dice in Dice_static)
        {
            AddVectorObs(Claws.transform.position - dice.transform.position);
        }
    }


    //--------------------------移動ACTION--------------------------//
    // agent移動
    public void MoveAgent(float[] act)
    {
        // 從Heuristic()獲得移動act
        var action = Mathf.FloorToInt(act[0]);

        if (AgentSphere.PuedeControlarse)
        {
            // 每移動一步就減少reward
            //AddReward(-1f / agentParameters.maxStep);


            //----往前
            if (transform.position.z < AgentSphere.limitadorBack.transform.position.z)
            {
                if (action == 1)
                {
                    transform.Translate(0, 0, AgentSphere.speed * Time.deltaTime);
                }
            }

            //----往後
            if (transform.position.z > AgentSphere.limitadorFront.transform.position.z)
            {
                if (action == 2)
                {
                    transform.Translate(0, 0, AgentSphere.speed * -1 * Time.deltaTime);
                }
            }

            //----往左
            if (transform.position.x > AgentSphere.limitadorLeft.transform.position.x) // 在左邊邊界之內
            {
                if (action==4) // 按下A鍵
                {
                    transform.Translate(AgentSphere.speed * -1 * Time.deltaTime, 0, 0); //sphere往左移動
                }
            }

            //----往右
            if (transform.position.x < AgentSphere.limitadorRight.transform.position.x)
            {
                if (action==3)
                {
                    transform.Translate(AgentSphere.speed * Time.deltaTime, 0, 0);
                }
            }

            //----空白鍵下鉤
            if (AgentSphere.AlturaGancho.position.y > AgentSphere.alturaMinima)
            {
                if (action==5)
                {
                    transform.Translate(0, AgentSphere.speed_Claw * -1 * Time.deltaTime, 0);
                }
            }
            //下鉤碰到底部，開始跑夾起物品的動畫
            else
            {
                StartCoroutine(CerrarClaw(2.0f));
                AgentSphere.PuedeControlarse = false;
            }
        }
    }

    // 決定action
    public override float[] Heuristic()
    {
        if (Input.GetKey(KeyCode.W))
        {
            return new float[] { 1 };
        }
        if (Input.GetKey(KeyCode.S))
        {
            return new float[] { 2 };
        }
        if (Input.GetKey(KeyCode.D))
        {
            return new float[] { 3 };
        }
        if (Input.GetKey(KeyCode.A))
        {
            return new float[] { 4 };
        }
        if (Input.GetKey(KeyCode.Space))
        {
            return new float[] { 5 };
        }
        return new float[] { 0 };
    }

    //夾起獎品
    IEnumerator CerrarClaw(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        AgentSphere.CerrarClaw(); //關爪

        yield return new WaitForSeconds(waitTime);

        AgentSphere.SoltarPremioTrue(); //到MovimientosClaw.cs執行夾娃娃到洞口的程序

    }


    //--------------------------改變REWARD--------------------------//
    public override void AgentAction(float[] vectorAction)
    {
        // 每走一步，reward會減少-1/maxStep
        AddReward(-1f / agentParameters.maxStep);
        MoveAgent(vectorAction); // 做出動作
    }

    //獎品進到洞口，增加reward
    public void GetPrize_AddScore()
    {
        AddReward(2f); //reward增加1
        prizeOnStage--; // 檯面上的獎品減少一個

        if (prizeOnStage == 0) //如果獎品都被夾完，要rest
        {
            Done();
        }
    }

    public void TouchThePrize()
    {
        AddReward(0.05f);
        Debug.Log("Hit!!!");
    }


    //--------------------------重設夾娃娃機--------------------------//
    public override void AgentReset()
    {
        Debug.Log("Agent Reset!");
        prizeOnStage = numberOfPrize; //重置獎品數量
        this.transform.position = ClawOriginPosition; //爪子位置重置
        //m_MyArea.CleanPrizeArea(numberOfPrize); //清空獎品，在version 3使用
        //m_MyArea.CreatePrize(numberOfPrize); //擺放獎品，在version 3使用
        m_MyArea.ResetObjectPosition(); //重設場上骰子的位置
    }
}
