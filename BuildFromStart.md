# Claw Machine - How To Build From Start

## 步驟一：下載環境與 ml-agent

#### 先將遊戲環境clone下來
`git clone https://github.com/czazuaga/Claw_Machine_Simulator.git`
將 Claw Machine Proyect 解壓縮，`Claw Machine Proyect/Assets`放進新建的 Unity 專案的 Assets之下。    
#### 下載最新版本的ml-agents
`git clone --branch latest_release https://github.com/Unity-Technologies/ml-agents.git`  
⁨把`ml-agents-master⁩/UnitySDK⁩/⁨Assets⁩/ML-Agents⁩`放進新建的 Unity 專案的 Assets之下。


## 步驟二：重建 Claw_Machine_Simulator

[Original Claw_Machine_Simulator](https://github.com/czazuaga/Claw_Machine_Simulator) By czazuaga    
因原作者使用的unity版本與我們使用的不相符，開啟後會產生很多錯誤，如動畫消失、模型分離、碰撞失效，而且之後要將環境改造成適合ml-agents使用，因此要先將遊戲稍作修復與修改。 

* **Animator & Animation 動畫製作**    
<img src="Pictures/Animator.png" align="middle" width="3000"/>
當爪子關閉時，`SetBool("Abrir", false)`且`SetBool("Cerrar", true)`，`Claw_close_ani`會被呼叫，而該動畫的配置如下，開始為爪子張開的狀態(三根爪子的rotation為x:135)，0.5秒後爪子是閉合的狀態(三根爪子的rotation為x:95)。     
<img src="Pictures/Animation.png" align="middle" width="3000"/>