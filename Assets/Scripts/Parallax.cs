using UnityEngine;

public class Parallax : MonoBehaviour
{

    private float length;
    private float startposX;
    private float startposY;
    private float dist;
    private float temp;
    public GameObject cam;
    public float parallaxEffect;



    void Start()
    {
        startposX = transform.position.x;
        startposY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    void Update()
    {
        temp = (cam.transform.position.x * (1 - parallaxEffect));
        dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startposX + dist, startposY, transform.position.z);

        if (temp > startposX + length)
        {
            startposX += length;
        }
        else if (temp < startposX - length)
        {
            startposX -= length;
        }
    }
}
