#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITCafe.Editor
{
    public class Screenshoter : MonoBehaviour
    {
        [SerializeField] private string _path;
        [SerializeField] private string _imageName = "Screenshot";
        
        private void Update()
        {
            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                var fileName = $"{_imageName}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.png";

                ScreenCapture.CaptureScreenshot(Path.Combine(_path, fileName));
                Debug.Log($"Screenshoter: {fileName} Captured");
            }
        }
    }
}
#endif