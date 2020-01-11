using UnityEngine;
using MLAgents;

// 在每個方形迷宮中放入此script
public class PyramidArea : Area
{
    // 引入兩個GameObject，分別是目標金字塔與一般金字塔
    public GameObject pyramid;
    public GameObject stonePyramid;
    // 有9個spawnArea，9個地方可以生成金字塔
    public GameObject[] spawnAreas;
    public int numPyra; // 1
    public float range; // 45

    // 建立目標金字塔
    public void CreatePyramid(int numObjects, int spawnAreaIndex)
    {
        CreateObject(numObjects, pyramid, spawnAreaIndex);
    }

    // 建立石像金字塔，無法被推倒
    public void CreateStonePyramid(int numObjects, int spawnAreaIndex)
    {
        CreateObject(numObjects, stonePyramid, spawnAreaIndex);
    }

    // 建立兩種金字塔皆使用此funciton，差別在於傳入GameObject參數不同，numObjects是建造數量
    void CreateObject(int numObjects, GameObject desiredObject, int spawnAreaIndex)
    {
        for (var i = 0; i < numObjects; i++)
        {
            // 生成物件在某物件的位置：Instantiate(要生成的物件, 物件位置, 物件旋轉值)
            var newObject = Instantiate(desiredObject, Vector3.zero,
                Quaternion.Euler(0f, 0f, 0f), transform);
            PlaceObject(newObject, spawnAreaIndex);
        }
    }

    //擺放生成的物件，有一定範圍的隨機值
    public void PlaceObject(GameObject objectToPlace, int spawnAreaIndex)
    {
        var spawnTransform = spawnAreas[spawnAreaIndex].transform;
        var xRange = spawnTransform.localScale.x / 2.1f;
        var zRange = spawnTransform.localScale.z / 2.1f;

        objectToPlace.transform.position = new Vector3(Random.Range(-xRange, xRange), 2f, Random.Range(-zRange, zRange))
            + spawnTransform.position;
    }

    // attached且有"pyramid"tag的game object會被刪除
    public void CleanPyramidArea()
    {
        foreach (Transform child in transform)
            if (child.CompareTag("pyramid"))
            {
                Destroy(child.gameObject);
            }
    }

    public override void ResetArea()
    {
    }
}
