using UnityEngine;
using System.Collections;

public class PokerFromImage : MonoBehaviour
{
    private int angle = 0;
    
    private int pokerNo = 6;

    // Call this method to start the rotation
    public void Start()
    {
        float s = 1.5f;
        float ds = 0.05f;

        for (int k = 0; k < pokerNo; k++)
        {
            GameObject poker = new GameObject();
            poker.tag = "front";
            // for empty object, default transform.forward equals (0,0,1), and
            // gameObject.tag = "Untagged".
            // if (gameObject.tag == "Untagged") or
            // poker.CompareTag("Untagged")
            Debug.Log(poker.CompareTag("Untagged"));

            GameObject front = GameObject.CreatePrimitive(PrimitiveType.Quad);
            front.tag = "poker";
            front.transform.localScale = new Vector3(s*0.71f, s*0.96f, s*1);
            front.transform.position = new Vector3(0, 0, -ds);
            // (*Assets 下的所有名稱為 Resources 的目錄(路徑間需要限定使用 slash 分隔)，都會被尋找到。)
            front.GetComponent<Renderer>().material.mainTexture = Resources.Load("pokers/poker" +k) as Texture;
            front.transform.parent = poker.transform;

            GameObject back = GameObject.CreatePrimitive(PrimitiveType.Quad);
            back.tag = "poker";
            back.transform.localScale = new Vector3(s * 0.71f, s * 0.96f, s * 1);
            back.transform.position = new Vector3(0, 0, ds);
            back.transform.rotation = Quaternion.Euler(0, 180, 0);
            // (*Assets 下的所有名稱為 Resources 的目錄(路徑間需要限定使用 slash 分隔)，都會被尋找到。)
            back.GetComponent<Renderer>().material.mainTexture = Resources.Load("pokers/PokerBack-2") as Texture;
            back.transform.parent = poker.transform;

            //poker.transform.position = ranndomPokerPosition();

            poker.transform.position = PokerPosition(k, pokerNo) * s;
        }

    }

    // a layout function just for test, 
    Vector3 PokerPosition(int index, int fromN)
    {
        float start = (10 - (fromN - 0.29f)) / 2 - 5;
        return new Vector3(start + index, 0.48f, 0);
    }

    Vector3 ranndomPokerPosition()
    {
        return new Vector3(Random.Range(-4f, 4f),
                           Random.Range(0.5f, 5f),
                           Random.Range(-5f, 3f)
                          );
    }

}