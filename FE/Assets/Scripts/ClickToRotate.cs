using System.Collections;
using UnityEngine;

public class ClickToRotate : MonoBehaviour
{
    Quaternion front, back;
    float duration = 0.6f;

    GameObject g = null;
    bool rotating = false;

    // Start is called before the first frame update
    void Start()
    {
        // for rotation:
        front = Quaternion.Euler(0, 0, 0);
        back  = Quaternion.Euler(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (! rotating)
        {
            if (Input.GetMouseButtonDown(0))  // 0: press mouse's left button, 0 for right button
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    Debug.Log("In Update()");
                    if (hit.collider.gameObject.tag != "poker")
                    {
                        return;
                    }

                    // empty gameobject enclosed by 2 quads.
                    g = hit.collider.gameObject.transform.parent.gameObject;  

                    if (g != null)
                    {
                        rotating = true;

                        StartCoroutine(RotateOverTime(g));  // have to use StartCoroutine(.) to run
                        Debug.Log("find g");
                    }
                }
            }
        }

    }

    IEnumerator RotateOverTime(GameObject g)
    {
        Debug.Log("In RotateOverTime()");
        float elapsedTime = 0f;
        float t;

        // 計算目前進度的百分比 (0 到 1 之間):
        while (elapsedTime < duration)
        {
            t = elapsedTime / duration;

            // after defining the tag 'front', one can use following two methods to check,
            //if (g.CompareTag("front"))
            if (g.tag == "front")
                g.transform.rotation = Quaternion.Lerp(front, back, t);
            else
                g.transform.rotation = Quaternion.Lerp(back, front, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 確保旋轉精準結束在目標值:
        if (g.CompareTag("front"))
        //if (g.tag == "front")
        {
            g.transform.rotation = back;
            g.tag = "back";
        }
        else
        {
            g.transform.rotation = front;
            g.tag = "front";
        }

        rotating = false;
    }
}
