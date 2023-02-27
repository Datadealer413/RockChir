using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class SessionsCalendarManager : MonoBehaviour, IViewable
    {
        [SerializeField] private ServiceManager serviceManager;
        [SerializeField] private GameObject content, rootObj, templateSessionObj, popup;
        [SerializeField] private ResponsePanel responsePanel;
        [SerializeField] private Text popupText;

        private bool gotSessions = false;

        public bool viewActive
        {
            get
            {
                return content.activeSelf;
            }
        }

        public void SetView(bool viewActive)
        {
            content.SetActive(viewActive);

            if (viewActive)
            {
                TopMenuManager.managerInstance.visible = true;
                TopMenuManager.managerInstance.menuTitle = "SESSION CALENDAR";
                popup.SetActive(false);

                if (!gotSessions)
                {
                    gotSessions = true;
                    StartCoroutine(GetSessionCalendar());
                }
            }
        }

        public void ShowPopup(JSONObject data)
        {
            string[] startDate = data["Start_Time__c"].str.Split('T');
            string[] startTime = startDate[0].Split('-');

            string[] endDate = data["End_Time__c"].str.Split('T');
            string[] endTime = startDate[0].Split('-');

            popup.SetActive(true);
            popupText.text = Regex.Replace(data["Venue_Details__c"].str.Replace("<br>", "\n"), @"<[^>]*>", String.Empty).ToUpper() + "\n" +
                startTime[2] + "/" + startTime[1] + "/" + startTime[0].Substring(2, 2) + "\n \n" +
                startDate[1].Substring(0, startDate[1].Length - 3) + " - " + endDate[1].Substring(0, startDate[1].Length - 3);
        }

        private IEnumerator GetSessionCalendar()
        {
            JSONObject response = null;
            yield return StartCoroutine(serviceManager.MakeRequest(RequestType.SessionCalendar, value => response = value as JSONObject));

            if (!response.IsNull && response.HasField("Records") && response["Records"].Count > 0)
            {
                responsePanel.visible = false;

                for (int i = 0; i < response["Records"].Count; i++)
                {
                    GameObject obj = Instantiate(templateSessionObj, rootObj.transform);
                    obj.GetComponent<CalendarSession>().SetCalendarSession(this, response["Records"][i], Color.black, Color.black);
                }
            }
            else
            {
                responsePanel.response = "No sessions in calendar";
                gotSessions = false;
            }
        }
    }
}