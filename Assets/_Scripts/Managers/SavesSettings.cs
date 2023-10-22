using System;
using UnityEngine;

namespace _Scripts.Managers
{
    public class SavesSettings : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            ES3Settings.defaultSettings.path = @"C:\Work\UnityProjects\DriftCars\Assets\Resources\savedata.json";
#else
            ES3Settings.defaultSettings.path = Application.persistentDataPath + "/savedata.json";
#endif
        }
    }
}