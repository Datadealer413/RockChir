using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    [System.Serializable]
    public class ColorSet
    {
        public Color background, font;
    }

    public class SessionManager : MonoBehaviour, IViewable
    {
        public Session selectedSession { get; private set; }
        public ChoirGroup selectedChoirGroup { get; private set; }
        
        [SerializeField] private GameObject popUp; 
        [SerializeField] private ServiceManager serviceManager;
        [SerializeField] private SwitchView switchView;
        [SerializeField] private GameObject content;
        [SerializeField] private ResponsePanel responsePanel;
        [SerializeField] private Text sessionMenuText;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Animator anim;
        [SerializeField] private GameObject templateChoirGroup, templateSession, choirGroupsRootObj, sessionsRootObj;
        [SerializeField] private ColorSet[] colors;
        private bool sessionsSet = false, isSessionsDisplayed = false, isSliding = false, _forceView = false;
        private Vector2 origCGPos, origSessionPos;

        public bool viewActive { get { return content.activeSelf; } }

        public void forceView(bool shouldForce)
        {
            _forceView = shouldForce;
        }

        private void Awake()
        {
            SaveData.onDataLoadComplete += SetupInfo;
            ServiceManager.loginComplete += SetSessions;
        }

        public void SetView(bool viewActive)
        {
            if (viewActive)
            {
                if (SaveData.saveData.hasExistingData)
                {
                    if (_forceView)
                    {
                        ShowView();
                        ClearData();
                    }

                    popUp.transform.parent.gameObject.SetActive(true);
                    popUp.SetActive(true);
                }
                else
                {
                    ShowView();
                }
            }
            else
            {
                content.SetActive(false);
            }
        }

        private void ShowView()
        {
            TopMenuManager.managerInstance.visible = true;
            TopMenuManager.managerInstance.menuTitle = "Sessions";
            _forceView = false;
            content.SetActive(true);
        }
        
        public void BackButton()
        {
            if (isSessionsDisplayed)
            {
                if (!isSliding)
                {
                    StartCoroutine(SlideWindow());
                }
            }
            else
            {
                switchView.ChangeView(View.LeaderMenu);
            }
        }

        public void ClearData()
        {
            selectedSession = null;
            selectedChoirGroup = null;
            SaveData.saveData.ClearSavedData();
        }

        public void SetSession(Session session)
        {
            selectedSession = session;
            sessionMenuText.text = selectedChoirGroup.rawData["Name"].str;

            // Save in persistent file
            SaveData.saveData.choirGroupSessionId = session.rawData["Id"].str; 
            SaveData.saveData.choirGroupId = selectedChoirGroup.rawData["Id"].str;
            SaveData.saveData.choirGroupName = selectedChoirGroup.rawData["Name"].str;
            SaveData.saveData.chroiGroupTime = selectedSession.DateTime();

            switchView.ChangeView(View.LeaderMenu);
        }

        private void SetSessions()
        {
            if(Utility.ToEnum<LoginType>(SaveData.saveData.loginType) == LoginType.ActiveLeader)
            {
                anim.SetBool("Loading", true);

                if (!sessionsSet)
                {
                    StartCoroutine(DisplayChoirGroups());
                }
            }
        }

        public void ShowGroupSessions(ChoirGroup cg)
        {
            if (!isSliding)
            {
                anim.SetBool("Loading", true);
                isSliding = true;
                selectedChoirGroup = cg;
                StartCoroutine(DisplaySessions(cg.rawData["Id"].str));
            }
        }
        
        private void SetupInfo(SessionData data)
        {
            sessionMenuText.text = data.choirGroupName;
        }

        private IEnumerator DisplayChoirGroups()
        {
            JSONObject choirGroupData = null;
            yield return StartCoroutine(serviceManager.MakeRequest(RequestType.ChoirGroups, value => choirGroupData = value as JSONObject));
            
            if (!choirGroupData.IsNull && choirGroupData.HasField("Records") && choirGroupData["Records"].Count > 0)
            {
                responsePanel.visible = false;

                int iColor = 0;
                for (int i = 0; i < choirGroupData["Records"].Count; i++)
                {
                    GameObject obj = Instantiate(templateChoirGroup, choirGroupsRootObj.transform);

                    obj.GetComponent<RectTransform>().position = obj.GetComponent<RectTransform>().position + new Vector3((obj.GetComponent<RectTransform>().rect.size.y * (i + 1)), 0, 0);
                    obj.GetComponent<ChoirGroup>().SetupSession(this, choirGroupData["Records"][i], colors[iColor].background, colors[iColor].font);
                    iColor = iColor + 1 < colors.Length ? iColor + 1 : 0;

                    sessionsSet = true;
                    yield return null;
                }
            }
            else
            {
                responsePanel.response = "NO CHOIRS AVAILABLE";
            }
            

            anim.SetBool("Loading", false);
            isSliding = false;
        }
        
        private IEnumerator DisplaySessions(string _id)
        {
            RectTransform[] oldSessions = sessionsRootObj.GetComponentsInChildren<RectTransform>();

            for(int i=0; i < oldSessions.Length; i++)
            {
                if (oldSessions[i].gameObject != sessionsRootObj.gameObject)
                {
                    Destroy(oldSessions[i].gameObject);
                }
            }
            
            JSONObject sessionData = null;
            yield return StartCoroutine(serviceManager.MakeRequest(RequestType.Sessions, value => sessionData = value as JSONObject, new string[] { _id } ));

            yield return StartCoroutine(SlideWindow());

            int iColor = 0;
            for (int i = 0; i < sessionData["Records"].Count; i++)
            {
                // Stop process if they hit back early
                if (!isSessionsDisplayed)
                {
                    break;
                }

                GameObject obj = Instantiate(templateSession, sessionsRootObj.transform);

                obj.GetComponentInChildren<Session>().SetupSession(this, sessionData["Records"][i], colors[iColor].background, colors[iColor].font);
                iColor = iColor + 1 < colors.Length ? iColor + 1 : 0;

                sessionsSet = true;
                yield return new WaitForSeconds(0.1f);
            }

            anim.SetBool("Loading", false);
        }

        private IEnumerator SlideWindow()
        {
            isSliding = true;
            isSessionsDisplayed = !isSessionsDisplayed;

            RectTransform rectChoirGroupWindow = choirGroupsRootObj.transform.parent.GetComponent<RectTransform>();
            RectTransform rectSessionWindow = sessionsRootObj.transform.parent.GetComponent<RectTransform>();
            rectSessionWindow.position = rectChoirGroupWindow.position + new Vector3(rectChoirGroupWindow.rect.size.x, 0, 0);

            // Set control of scrollbar correctly
            rectChoirGroupWindow.GetComponent<ScrollRect>().verticalScrollbar = isSessionsDisplayed ? null : scrollbar;
            rectSessionWindow.GetComponent<ScrollRect>().verticalScrollbar = isSessionsDisplayed ? scrollbar : null;

            float moveAmount = rectChoirGroupWindow.rect.size.x;
            Vector2 origChoirGroupPosition = rectChoirGroupWindow.position;
            Vector2 origSessionPosition = rectSessionWindow.position;

            Vector2 finalChoirGroupPos = isSessionsDisplayed ? new Vector2((origChoirGroupPosition.x - moveAmount), origChoirGroupPosition.y) : new Vector2((origChoirGroupPosition.x + moveAmount), origChoirGroupPosition.y);
            Vector2 finalSessionPos = isSessionsDisplayed ? new Vector2((origSessionPosition.x - moveAmount), origSessionPosition.y) : new Vector2((origSessionPosition.x + moveAmount), origSessionPosition.y);

            float time = 1;
            for(float t=0; t < time; t += Time.deltaTime)
            {
                float timeElapsed = t / time;

                rectChoirGroupWindow.position = Vector2.Lerp(origChoirGroupPosition, finalChoirGroupPos, timeElapsed);
                rectSessionWindow.position = Vector2.Lerp(origSessionPosition, finalSessionPos, timeElapsed);
                
                yield return new WaitForFixedUpdate();
            }

            isSliding = false;

            yield return null;
        }
    }
}