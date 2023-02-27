using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class CalendarSession : DownloadedObject
    {
        [SerializeField] private Text titleAndDate, addressAndTime;
        [SerializeField] private Image background;
        [SerializeField] private RectTransform rect;
        private SessionsCalendarManager sessionCalendarManager;
        
        public void SetCalendarSession(SessionsCalendarManager newManager, JSONObject rawObj, Color bg, Color font)
        {
            sessionCalendarManager = newManager;

            rawData = rawObj;

            Debug.Log(rawData);

            string[] startDate = rawData["Start_Time__c"].str.Split('T');
            string[] startTime = startDate[0].Split('-');

            string[] endDate = rawData["End_Time__c"].str.Split('T');
            string[] endTime = startDate[0].Split('-');

            string output = Regex.Replace(rawData["Venue__c"].str, @"<(.|\n)*?>", string.Empty);

            titleAndDate.text = rawData["Choir_Group_Name__c"].str.Replace(" Session", string.Empty).ToUpper() + "   " + startTime[2] + "/" + startTime[1] + "/" + startTime[0].Substring(2,2);
            addressAndTime.text = output.ToUpper() + " - " + startDate[1].Substring(0, startDate[1].Length - 3) + " - " + endDate[1].Substring(0, startDate[1].Length - 3);

            RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(parentRect.rect.size.x, Screen.height * heightInPercent);
        }

        public void ShowPopUp()
        {
            sessionCalendarManager.ShowPopup(rawData);
        }
    }
}