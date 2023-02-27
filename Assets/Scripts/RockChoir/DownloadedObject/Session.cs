using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class Session : DownloadedObject
    {
        [SerializeField] private Text sessionName;
        [SerializeField] private Image bg;
        [SerializeField] private RectTransform rect;
        private SessionManager sessionManager;

        // When you press the session button, it becomes the selected session
        public void Selected()
        {
            sessionManager.SetSession(this);
        }

        public string DateTime()
        {
            return sessionName.text;
        }

        public void SetupSession(SessionManager _sessionManager, JSONObject _sessionData, Color bgColor, Color fontColor)
        {
            sessionManager = _sessionManager;

            rect.sizeDelta = new Vector2(Screen.width, Screen.height * heightInPercent);

            rawData = _sessionData;
            sessionName.text = FormatDate(rawData["Start_time__c"].str);
            bg.color = bgColor;
            sessionName.color = fontColor;
        }

        private string FormatDate(string unformatted)
        {
            string formatted = "_DATE_\n_TIME_";

            if (unformatted.Contains("-"))
            {
                string[] splitDate = unformatted.Split('-');
                string[] splitTime = splitDate[2].Split('T');

                formatted = formatted.Replace("_DATE_", splitTime[0] + "/" + splitDate[1] + "/" + splitDate[0]);

                string newTime = splitTime[1];

                if(splitTime[1][0] == '0')
                {
                    newTime = splitTime[1].Substring(1, splitTime[1].Length - 4) + "am";
                }
                else
                {
                    newTime = splitTime[1].Substring(0, splitTime[1].Length - 3);
                }

                formatted = formatted.Replace("_TIME_", newTime);
            }

            return formatted;
        }
    }
}