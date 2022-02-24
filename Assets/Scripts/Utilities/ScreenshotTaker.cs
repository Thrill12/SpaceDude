using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenshotTaker : MonoBehaviour
{
    public void TakeScreenshot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            ScreenCapture.CaptureScreenshot(System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) +
                        @"\" + DateTime.UtcNow.Hour + "-" + DateTime.UtcNow.Minute + "-" + DateTime.UtcNow.Second + " Haven Screenshot.bmp", 24);
            Debug.Log("Saved screenshot at " + System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) +
                @"\" + DateTime.UtcNow.Hour + "-" + DateTime.UtcNow.Minute + "-" + DateTime.UtcNow.Second + " Haven Screenshot.bmp");
        }       
    }
}
