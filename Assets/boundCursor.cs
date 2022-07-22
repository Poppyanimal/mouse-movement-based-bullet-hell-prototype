using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class boundCursor : MonoBehaviour
{
    public Rigidbody2D puppet;
    float maxDistanceFromPuppet = 2f;
    Rigidbody2D leash;
    LineRenderer tether_line;
    Vector2 centerVector;

    void Start()
    {
        leash = gameObject.GetComponent<Rigidbody2D>();
        tether_line = gameObject.GetComponent<LineRenderer>();
        
        updateTether();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        centerVector = new Vector2(Mathf.Round(Screen.safeArea.center.x), Mathf.Round(Screen.safeArea.center.y));
        centerCursor();
    }

    bool isEscapeLocked = true; //TODO: better name
    bool needToReHideCursor = false;
    void FixedUpdate()
    {
        if(Application.isFocused && isEscapeLocked)
        {
            if(needToReHideCursor)
            {
                Cursor.visible = false;
                needToReHideCursor = false;
            }
            doEscapeLockCheck();

            leash.position = leash.position + (getCursorMovement()/250);
            if((puppet.position - leash.position).magnitude > maxDistanceFromPuppet)
                leash.position = puppet.position + ((leash.position - puppet.position).normalized * maxDistanceFromPuppet);
            
            centerCursor();
        }
        else if(!Application.isFocused)
        {
            isEscapeLocked = true; //TODO: Find better impl later
            needToReHideCursor = true;
        }
    }

    void doEscapeLockCheck()
    {
        if(Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.O))
            isEscapeLocked = false;
    }

    void Update()
    {
        updateTether();
    }

    void centerCursor()
    {
        Mouse.current.WarpCursorPosition(centerVector);
        InputState.Change(Mouse.current.position, centerVector);
    }

    Vector2 getCursorMovement()
    {
        return new Vector2(Mouse.current.position.x.ReadValue() - centerVector.x, Mouse.current.position.y.ReadValue() - centerVector.y);
    }

    void updateTether()
    {
        tether_line.SetPosition(0, puppet.position);
        tether_line.SetPosition(1, leash.position);
    }

    public float getMaxDistance()
    {
        return maxDistanceFromPuppet;
    }

    //TODO: check how cursor will be effected in rotated space and as the parent object is displaced
}
