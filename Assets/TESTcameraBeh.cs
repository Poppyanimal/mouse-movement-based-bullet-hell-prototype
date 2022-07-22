using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTcameraBeh : MonoBehaviour
{
    public Rigidbody2D player;
    public Rigidbody2D testTarget;
    Rigidbody2D camera;

    void Start()
    {
        camera = gameObject.GetComponent<Rigidbody2D>();
        updateZoomMath();
    }

    void Update()
    {
        if(testTarget == null)
        {
            camera.position = player.position;
        }
        else
        {
            Vector2 midpoint = new Vector2(
                (((testTarget.position.x - player.position.x)/2f) + player.position.x),
                (((testTarget.position.y - player.position.y)/2f) + player.position.y));

            if(Mathf.Abs(midpoint.x - player.position.x) > maxDistance_X)
            {
                if(midpoint.x - player.position.x > 0)
                    midpoint.x = player.position.x + maxDistance_X; 
                else
                    midpoint.x = player.position.x - maxDistance_X; 
            }
            if(Mathf.Abs(midpoint.y - player.position.y) > maxDistance_Y)
            {
                if(midpoint.y - player.position.y > 0)
                    midpoint.y = player.position.y + maxDistance_Y; 
                else
                    midpoint.y = player.position.y - maxDistance_Y; 
            }

            camera.position = midpoint;
        }

        //later disengage logic
        //later cursor around target (that attempts to stay in camera)
        //later switching between targets based on player distance
    }

    
    float maxDistance_X;
    float maxDistance_Y;
    //TODO: make both x and y always same distance from camera?
    public void updateZoomMath()
    {
        float xToYRatio = gameObject.GetComponent<Camera>().aspect;
        float zoomModifier = 0.8f;

        Debug.Log("cam aspect ratio: " + xToYRatio);
        Debug.Log("cam orth size: " + gameObject.GetComponent<Camera>().orthographicSize);

        maxDistance_Y = gameObject.GetComponent<Camera>().orthographicSize * zoomModifier;
        maxDistance_X = maxDistance_Y * xToYRatio;
    }
}
