using UnityEngine;
using System.Collections;


public sealed class Helper
{
    private static int screenshotCount = 0;

    public static void TakeScreenshot()
    {
        string screenshotFilename;
        do
        {
            screenshotCount++;
            screenshotFilename = "diablo_screenshot_" + screenshotCount + ".png";

        } while (System.IO.File.Exists(screenshotFilename));

        Application.CaptureScreenshot(Application.dataPath + "/../" + screenshotFilename);
    }

    // SystemInfo graphicsDeviceID graphicsDeviceVendorID 
}