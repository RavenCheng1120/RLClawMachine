# RLClawMachine 強化式機器學習期末專題

此專題的目標是製作一個自動夾娃娃機器人，利用 [czazuaga](https://github.com/czazuaga) 所製作的 unity Claw_Machine_Simulator，加上 unity 的 ml-agents，來訓練夾娃娃機自動夾取獎品。可以參考 Unity-Technologies
/ml-agents 的官方文件與線上教學，來進一步了解 ml-agent 的運作方式。    
unity version: 2019.2.0f1    
ml-agent version: ML-Agents Beta 0.12.0     

## Documentation

* 學習從頭打造自動夾娃娃機，[文件在此](BuildFromStart.md)!!!!
* 原始的夾娃娃機遊戲 [Claw_Machine_Simulator](https://github.com/czazuaga/Claw_Machine_Simulator)
* [Ml-agents](https://github.com/Unity-Technologies/ml-agents)官方文件
* 莫凡的講解 [Proximal Policy Optimization](https://morvanzhou.github.io/tutorials/machine-learning/reinforcement-learning/6-4-DPPO/)(PPO)

## 不同 Version 的嘗試

**第一版**：    
使用舊版 Ray Perception 3D，useVectorObs可以開啟(1)或關閉(0)射線。另外會觀察 Agent 的絕對位置。    
```c#
    // 三組射線角度
    float[] rayAngles = { 0f, 30f, 60f, 90f, 120f, 150f, 180f, 210f, 240f, 270f, 300f, 330f };
    float[] rayAngles1 = { 15f, 45f, 75f, 105f, 135f, 165f, 195f, 225f, 255f, 285f, 315f, 345f };
    float[] rayAngles2 = { 5f, 25f, 45f, 65f, 85f, 100f };

    // 需要偵測的物件tag
    string[] detectableObjects = {"goal"};
    AddVectorObs(m_RayPer.Perceive(2.2f, rayAngles, detectableObjects, -3f, -6f));
    AddVectorObs(m_RayPer.Perceive(1.4f, rayAngles1, detectableObjects, -3.5f, -6.5f));
    AddVectorObs(m_RayPer.Perceive(4.7f, rayAngles2, detectableObjects, -3.5f, -6.5f));
```
獎品進到洞口，增加 reward(2)；在不跑動畫時，每步減少 reward(-1/MaxStep)。    
MaxStep 為8000， 未更改 yaml 檔案中的數據。    

**第二版**：    
使用新版 Ray Perception Sensor Component 3D，形狀為倒傘狀，共有兩層，另外在右上方還有額外的偵測射線，去掉偵測 Agent 的絕對位置。    
MaxStep 為16000，每步減少 reward(-1/MaxStep)改成在所有時刻都發生。    
爪子在放下獎品後會回到中央，避免在洞口徘徊過長的時間。    

**第三版**：    
Ray Perception Sensor Component 3D形狀改為三層平面，各層有8條射線，三層各自有不同角度。    
MaxStep 為24000，爪子碰撞獎品時會增加 reward(0.05)，以利 agent 學習到要靠近爪子。    
獎品從各式形狀的物品改成固定的骰子，每次重置場景時，6顆骰子會隨機的在場上落下，以確保訓練的多樣性。

**第四版**：
去掉所有射線，改成收集獎品骰子的位置(x, y, z)，與爪子的位置做相減。    
場上骰子改為5顆(有嘗試1顆骰子，效果較為不優)，並在落下獎品洞口或是掉出場外一定高度後，會再度隨機擺回場上。     
    
<img src="Pictures/V3_Ray.png" align="middle" width="3000"/>    

## 程式碼解析 Code Explanation

### 1. MovimientosClaw.cs 

主要為 **FixedUpdate()** 區塊。若`if (PuedeControlarse)`為true，則可以控制爪子移動，先確認爪子沒有超出邊界，再透過`Input.GetKey(KeyCode)`獲得鍵盤按鍵 input。當爪子到底時，會觸發動畫：    
```c#
    StartCoroutine(CerrarClaw(2.0f));
    PuedeControlarse = false;
```
這會觸發連鎖反應，邊控制動畫邊呼叫不同函式，來完成夾取與投放獎品的動作。    
SoltarPremio -> SoltarPremioEnLaCesta -> bajarGanchoYSoltarPremio -> AbrirClawEnlaCesta -> subirGanchoDeLaCesta -> backToCenter -> goToCenter -> PuedeControlarse -> 完成一次循環。    
(因為夾娃娃機遊戲的作者是西班牙人，許多變數與函式是用西班牙文撰寫)    

### 2. ClawAgent.cs

需要在程式碼上方加上`using MLAgents;`。    
**InitializeAgent()**：一開始先初始化Agent。    
```c#
    public override void InitializeAgent()
    {
        base.InitializeAgent(); //Agent基礎初始化
        m_RayPer = GetComponent<RayPerception>(); //舊版射線使用的宣告
        useVectorObs = false; //關掉舊的射線
        numberOfPrize = 5; //從這裡可以調整獎品數量
        prizeOnStage = numberOfPrize; //紀錄場上的獎品數量
        AgentSphere = GetComponent<MovimientosClaw>();
        m_MyArea = area.GetComponent<AreaReset>();
        ClawOriginPosition = new Vector3(0.1f, 13.4f, -1.1f); //爪子初始位置
        Dice_static = GameObject.FindGameObjectsWithTag("goal"); //取得所有骰子物件
    }
```

**CollectObservations()**：    
觀察環境，收集指定的資訊。以下是沒有射線時觀察的數值。    
```c#
    AddVectorObs(gameObject.transform.position - Claws.transform.position); //觀察爪子跟控制圓球的相對位置
    AddVectorObs(AgentSphere.PuedeControlarse); //觀察是在否可以控制爪子的狀態
    foreach (GameObject dice in Dice_static)
    {
        AddVectorObs(Claws.transform.position - dice.transform.position);
    }
```

**Heuristic()**：    
取得鍵盤輸入，記錄到 float[] vectorAction 中。    
共有六種 action 方式，分別是W, A, S, D, Space, Don't move。    

**AgentAction()**：    
拿取 float[] vectorAction ，呼叫 MoveAgent(vectorAction) 做出動作。    
並且每走一步，reward會減少-1/maxStep，`AddReward(-1f / agentParameters.maxStep);`。    

**MoveAgent(float[] act)**：    
如 MovimientosClaw.cs 的設定，用 act 做出動作，對環境產生影響。    

**AgentReset()**：    
```c#
    Debug.Log("Agent Reset!");
    prizeOnStage = numberOfPrize;
    this.transform.position = ClawOriginPosition; //爪子位置重置
    // m_MyArea.CleanPrizeArea(numberOfPrize); //清空獎品，在version 3使用
    // m_MyArea.CreatePrize(numberOfPrize); //擺放獎品，在version 3使用
    m_MyArea.ResetObjectPosition(); //重設場上骰子的位置
```

### 3.GetPrize.cs

有獎品掉落洞口，與洞口平面閘門觸發Trigger，便呼叫Agent script中的函式。    
```C#
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="goal")
        {
            clawAgent.GetPrize_AddScore();
        }
    }
```

### 4.AreaReset.cs

**CleanPrizeArea(int deleteNumber)**：    
清除所有場上獎品，利用parent與child的關係，把stage裡面還有goal tag的物件刪除。    
```c#
    public void CleanPrizeArea(int deleteNumber)
    {
        parent = GameObject.Find("stage");
        foreach (Transform child in parent.transform)
            if (child.CompareTag("goal"))
            {
                Destroy(child.gameObject);
            }
    }
````

**創造骰子**：    
呼叫韓式順序為：CreatePrize() -> CreateObject() -> PlaceObject()    
`var newObject = Instantiate(desiredObject, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));` 動態生成物件。    
用 Random 的方法決定骰子的重生位置，並測量底盤物件的長寬與位置，避免骰子生成在機台外面。    

### 5.DisplayArea.cs

用GetCumulativeReward()取得目前的reward值，並以GetStepCount()取得Step數量。    
```c#
    void Update()
    {
        ScoreText.text = clawAgent.GetCumulativeReward().ToString("0.000");
        StepText.text = clawAgent.GetStepCount().ToString();
    }
```

## 訓練Agent

在 ml-agents-master 開啟終端機，輸入指令：
```
mlagents-learn config/trainer_config.yaml --run-id=IDNAME --train
```
隨後按下unity的開始按鈕，agnet便會開始訓練，此訓練是經過加速的，約35秒可以訓練一回合。    
trainer_config.yaml檔案中的參數設定會影響訓練，有預設的參數，也可以自行新增。    
在訓練結束後 ml-agents-master/models中會出現訓練結果，將ClawBody Behavior.nn檔複製到專案中，放到`Behavior Parameters`的model中，按下unity的開始按鈕，就能看到訓練的成果。    
