using System.Collections;
using System.IO;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    public static Paintable inst;

    void Awake()
    {
        inst = this;
    }

    public GameObject BlueBrush, GreenBrush, OrangeBrush;
    private GameObject currentBrush;
    private Car currentCar;
    public float BrushSize = 0.1f;
    public RenderTexture RTexture;

    public void ChooseBlueBrush(Car car)
    {
        currentBrush = BlueBrush;
        currentCar = car;
    }

    public void ChooseGreenBrush(Car car)
    {
        currentBrush = GreenBrush;
        currentCar = car;
    }

    public void ChooseOrangeBrush(Car car)
    {
        currentBrush = OrangeBrush;
        currentCar = car;
    }

    [SerializeField] bool choosedCar;
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //cast a ray to the plane
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(Ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Plane")
                {
                    if (currentBrush == null || !choosedCar)
                        return;

                    var clickPos = hit.point + Vector3.up * 0.1f;
                    WayPointsManager.inst.CreateWaypoint(currentCar, clickPos, currentBrush);
                }
                else if (hit.transform.gameObject.tag == "Car")
                {
                    hit.transform.gameObject.GetComponent<Car>().Choose();
                    choosedCar = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            choosedCar = false;
        }
    }

    public void Save()
    {
        StartCoroutine(CoSave());
    }

    private IEnumerator CoSave()
    {
        //wait for rendering
        yield return new WaitForEndOfFrame();
        Debug.Log(Application.dataPath + "/savedImage.png");

        //set active texture
        RenderTexture.active = RTexture;

        //convert rendering texture to texture2D
        var texture2D = new Texture2D(RTexture.width, RTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, RTexture.width, RTexture.height), 0, 0);
        texture2D.Apply();

        //write data to file
        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/savedImage.png", data);


    }
}
