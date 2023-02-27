using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class ScanViewManager : MonoBehaviour, IViewable
    {
        [SerializeField] private ServiceManager serviceManager;
        [SerializeField] private SessionManager sessionManager;
        [SerializeField] private Text sessionText;
        [SerializeField] private RectTransform rootScanObj;
        [SerializeField] private Scanner scanner;
        [SerializeField] private GameObject scanTemplate;
        [SerializeField] private ColorSet[] colors;
        [SerializeField] private GameObject content;
        private List<GameObject> scans = new List<GameObject>();

        public bool viewActive
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public void SetView(bool viewActive)
        {
            if (viewActive)
            {
                TopMenuManager.managerInstance.visible = true;
                TopMenuManager.managerInstance.menuTitle = "SEND";
                content.SetActive(true);
                sessionText.text = SaveData.saveData.choirGroupName + "\n" + SaveData.saveData.chroiGroupTime;
                for(int i=0; i < scans.Count; i++)
                {
                    Destroy(scans[i]);
                }
                scans.Clear();

                int iColor = 0;
                for (int i = 0; i < SaveData.saveData.scans.Count; i++)
                {
                    RectTransform obj = Instantiate(scanTemplate, rootScanObj).GetComponent<RectTransform>();
                    obj.sizeDelta = new Vector2(rootScanObj.rect.size.x, (Screen.height * 0.145f));
                    scans.Add(obj.gameObject);
                    obj.GetComponent<Scan>().SetupScanObject(SaveData.saveData.scans[i], colors[iColor].background, colors[iColor].font);

                    iColor = iColor + 1 < colors.Length ? iColor + 1 : 0;
                }
            }
            else
            {
                content.SetActive(false);
            }
        }

        public void SendScans()
        {
            StartCoroutine(ProcessScans());
        }

        private IEnumerator ProcessScans()
        {
            bool cleanSession = true;

            for (int i=0; i < scans.Count; i++)
            {
                JSONObject response = null;
                yield return StartCoroutine(serviceManager.MakeRequest(RequestType.Create, value => response = value as JSONObject, new string[] { SaveData.saveData.choirGroupSessionId, SaveData.saveData.scans[i] } ));
                
                if (response != null)
                {
                    if (response.HasField("BadToken"))
                    {
                        GetComponent<SwitchView>().ChangeView(View.Login);
                        GetComponent<Login>().Message("LOGIN REQUIRED");
                        cleanSession = false;
                    }

                    scans[i].GetComponent<Animator>().SetTrigger("Success");
                }
            }

            if (cleanSession)
            {
                scans.Clear();
                SaveData.saveData.ClearSavedData();
            }
        }
    }
}