using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;

namespace ITCafe.Editor
{
    [InitializeOnLoad]
    public static class IdeSetuper
    {
        private const string RIDER_PATH = @"D:\Applications\JetBrains\JetBrains Rider 2025.2.0.1\bin\rider64.exe";
        private const string VS_CODE_PATH = @"D:\Applications\Microsoft VS Code\Code.exe";
        
        static IdeSetuper()
        {
            // Срабатывает при загрузке редактора Unity
            EditorApplication.delayCall += OnProjectOpened;
            EditorApplication.quitting += OnProjectClosing;
            
            // Альтернативный способ - через изменение состояния проекта
            // EditorApplication.projectChanged += OnProjectChanged;
        }
        
        private static void OnProjectOpened()
        {
            EditorApplication.delayCall += () =>
            {
                UseRider();
            };
        }
        
        private static void OnProjectClosing()
        {
            UseVSCode();
        }
        
        private static void OnProjectChanged()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UseRider();
            }
        }
        
        public static void UseRider()
        {
            if (System.IO.File.Exists(RIDER_PATH))
            {
                CodeEditor.SetExternalScriptEditor(RIDER_PATH);
                EditorPrefs.SetString("kScriptsDefaultApp", RIDER_PATH);
                Debug.Log("Используется Rider");
            }
        }
        
        public static void UseVSCode()
        {
            if (System.IO.File.Exists(VS_CODE_PATH))
            {
                CodeEditor.SetExternalScriptEditor(VS_CODE_PATH);
                EditorPrefs.SetString("kScriptsDefaultApp", VS_CODE_PATH);
                Debug.Log("Используется VS Code");
            }
        }
    }
}