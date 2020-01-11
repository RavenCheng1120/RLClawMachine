using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class AreaReset : MonoBehaviour
{
    public GameObject Dice; //骰子的prefeb
    public GameObject[] spawnAreas; //stage中的floor box collider，總共兩片
    GameObject parent; //取得parent object，用來清除場景中的子物件(獎品)

    //--------new-------
    public GameObject[] Dice_static;
    //------------------

    private void Start()
    {
        Dice_static = GameObject.FindGameObjectsWithTag("goal");
    }

    private void Update()
    {
        foreach (GameObject dice in Dice_static)
        {
            if(dice.transform.position.y < -6.5)
            {
                PlaceObject(dice);
            }
        }
    }


    //清除所有獎品
    public void CleanPrizeArea(int deleteNumber)
    {
        parent = GameObject.Find("stage");
        foreach (Transform child in parent.transform)
            if (child.CompareTag("goal"))
            {
                Destroy(child.gameObject);
            }
    }

    //創造numObjects個骰子
    public void CreatePrize(int numObjects)
    {
        CreateObject(numObjects,Dice);
    }

    //創造物件
    void CreateObject(int numObjects, GameObject desiredObject)
    {
        for (var i = 0; i < numObjects; i++)
        {
            // 生成物件在某物件的位置：Instantiate(要生成的物件, 物件位置, 物件旋轉值)
            var newObject = Instantiate(desiredObject, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
            newObject.transform.parent = parent.transform; //設定在parent的transform之下
            PlaceObject(newObject); //擺放物件
        }
    }

    //擺放物件
    public void PlaceObject(GameObject objectToPlace)
    {
        int i = Random.Range(0, 2); //隨機決定再靠右或靠左的平台上
        var spawnTransform = spawnAreas[i].transform;
        var xRange = spawnTransform.localScale.x / 2.2f;
        var zRange = spawnTransform.localScale.z / 2.2f;

        //在平台上的區間隨機擺放物件
        objectToPlace.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
            + spawnTransform.position;
    }

    //--------new-------
    //重設骰子物件
    public void ResetObjectPosition()
    {
        Dice_static = GameObject.FindGameObjectsWithTag("goal");
        foreach (GameObject dice in Dice_static)
        {
            PlaceObject(dice);
        }
    }
    //------------------
}
