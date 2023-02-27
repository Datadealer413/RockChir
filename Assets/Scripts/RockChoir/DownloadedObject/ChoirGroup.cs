using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class ChoirGroup : DownloadedObject
    {
        [SerializeField] private Text sessionName;
        [SerializeField] private Image bg;
        [SerializeField] private RectTransform rect;
        private SessionManager sessionManager;

        public void SetupSession(SessionManager _sessionManager, JSONObject _choirGroupData, Color bgColor, Color fontColor)
        {
            sessionManager = _sessionManager;

            rect.sizeDelta = new Vector2(Screen.width, Screen.height * heightInPercent);

            rawData = _choirGroupData;
            sessionName.text = rawData["Name"].str;
            bg.color = bgColor;
            sessionName.color = fontColor;
        }

        public void SendSessionRequest()
        {
            sessionManager.ShowGroupSessions(this);
        }
    }
}