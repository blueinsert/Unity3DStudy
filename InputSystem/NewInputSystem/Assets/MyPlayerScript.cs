using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerScript : MonoBehaviour
{
    private void Start()
    {
       
    }

    public void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire!");
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        Debug.Log(string.Format("{0 }Scroll {1}", Time.frameCount, context.ReadValue<Vector2>()));
    }
}
