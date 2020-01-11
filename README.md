# RLClawMachine 強化式機器學習期末專題
此專題的目標是製作一個自動夾娃娃機器人，利用@czazuaga所製作的unity Claw_Machine_Simulator，加上unity的ml-agent，來訓練夾娃娃機自動夾取獎品。可以參考ml-agent的官方文件與線上教學，來進一步了解ml-agent的運作方式。    
此專題使用unity版本為"2019.2.0f1"，ml-agent版本為"ML-Agents Beta 0.12.0"。    
## 步驟一：重建Claw_Machine_Simulator
Original Claw_Machine_Simulator(https://github.com/czazuaga/Claw_Machine_Simulator) By czazuaga    
因原作者使用的unity版本與我們使用的不相符，開啟後會產生很多錯誤，如動畫消失、模型分離、碰撞失效，而且之後要將環境改造成適合ml-agent使用，因此要先將遊戲稍作修復與修改。    
