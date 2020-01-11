using UnityEngine;

public class PyramidSwitch : MonoBehaviour
{
    // 碰到前，按鈕是橘色；碰到後，按鈕變綠色
    public Material onMaterial;
    public Material offMaterial;
    public GameObject myButton;
    bool m_State;
    GameObject m_Area;
    PyramidArea m_AreaComponent;
    int m_PyramidIndex;

    // 知道現在是否有碰過按鈕
    public bool GetState()
    {
        return m_State;
    }

    void Start()
    {
        m_Area = gameObject.transform.parent.gameObject; //拿到parent物件：場景(AreaPB)
        m_AreaComponent = m_Area.GetComponent<PyramidArea>(); //拿到PyramidArea物件：在場景的script中
    }

    // 利用PyramidArea的PlaceObject函式，放置按鈕。switch物件設置為"switchOff"狀態。
    public void ResetSwitch(int spawnAreaIndex, int pyramidSpawnIndex)
    {
        m_AreaComponent.PlaceObject(gameObject, spawnAreaIndex);
        m_State = false;
        m_PyramidIndex = pyramidSpawnIndex;
        tag = "switchOff";
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        myButton.GetComponent<Renderer>().material = offMaterial; //按鈕顏色為橘色
    }

    // 與其他物件產生碰撞
    void OnCollisionEnter(Collision other)
    {
        // 若產生碰撞的對象是agent，而且m_State是false(已經重新設置過按鈕)
        if (other.gameObject.CompareTag("agent") && m_State == false)
        {
            myButton.GetComponent<Renderer>().material = onMaterial; //按鈕顏色為綠色
            m_State = true;
            m_AreaComponent.CreatePyramid(1, m_PyramidIndex); // 建立一個新的目標金字塔
            tag = "switchOn";
        }
    }
}
