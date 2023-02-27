using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections.ObjectModel;
using System;

namespace RockChoir
{
    [Serializable]
    public class SessionData
    {
        public string choirGroupId;
        public string choirGroupName;
        public string choirGroupTime;
        public string choirGroupSessionId;
        public string userEmail;
        public string loginType;
        public List<string> scans = new List<string>();
    }

    public class SaveData : MonoBehaviour
    {
        public delegate void DataLoadComplete(SessionData _sessionData);
        public static event DataLoadComplete onDataLoadComplete;
        public static event DataLoadComplete onDataUpdated;

        // Dirty singletons, but there ya go
        public static SaveData saveData
        {
            get
            {
                if(_saveData == null)
                {
                    GameObject obj = new GameObject("SaveDataController");
                    _saveData = obj.AddComponent<SaveData>();
                    _saveData.LoadData();
                }

                return _saveData;
            }
        }
        
        public string choirGroupId { get { return sessionData.choirGroupId; } set { sessionData.choirGroupId = value; UpdateSaveData(); } }
        public string choirGroupName { get { return sessionData.choirGroupName; } set { sessionData.choirGroupName = value; UpdateSaveData(); } }
        public string chroiGroupTime { get { return sessionData.choirGroupTime; } set { sessionData.choirGroupTime = value; UpdateSaveData(); } }
        public string choirGroupSessionId { get { return sessionData.choirGroupSessionId; } set { sessionData.choirGroupSessionId = value; UpdateSaveData(); } }
        public string userEmail { get { return sessionData.userEmail; } set { sessionData.userEmail = value; UpdateSaveData(); } }
        public string loginType { get { return sessionData.loginType; } set { sessionData.loginType = value.Replace("Choir ", "").Replace(" ", ""); UpdateSaveData(); } }
        public bool hasExistingData { get { return sessionData.choirGroupSessionId != null ? true : false; } }
        public ReadOnlyCollection<string> scans { get { return sessionData.scans.AsReadOnly(); } }

        private const string fileName = "/savedData.gd";
        private static SaveData _saveData;
        [SerializeField] private SessionData sessionData;
        
        public bool AddScan(string scan)
        {
            if (!sessionData.scans.Contains(scan))
            {
                sessionData.scans.Add(scan);
                UpdateSaveData();
                return true;
            }
            return false;
        }

        public bool RemoveScan(string scan)
        {
            if(sessionData.scans.Contains(scan))
            {
                sessionData.scans.Remove(scan);
                UpdateSaveData();
                return true;
            }
            return false;
        }
        
        public void ClearAllScans()
        {
            sessionData.scans.Clear();
            UpdateSaveData();
        }
        
        private void LoadData()
        {
            if (File.Exists(Application.persistentDataPath + fileName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
                sessionData = (SessionData)bf.Deserialize(file);
                file.Close();
            }
            else
            {
                sessionData = new SessionData();
            }
            
            // Fire event when loaded previous data
            if (onDataLoadComplete != null)
            {
                onDataLoadComplete(sessionData);
            }
        }
        
        private void UpdateSaveData()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + fileName);
            bf.Serialize(file, sessionData);
            file.Close();

            if(onDataUpdated != null)
            {
                onDataUpdated(sessionData);
            }
        }
        
        // BinaryFormatter requires this in Unity projects
        private void Awake()
        {
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        }

        public void WakeMe()
        {
            // No action, just something to wake him into action. Like the lazy slug he is.
        }

        public void ClearSavedData()
        {
            sessionData = new SessionData();
            UpdateSaveData();
        }
    }
}