using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public enum RequestType { Login, ContactInfo, EmergencyContact, BillingHistory, SessionCalendar, UpdatePersonalInfo, Sessions, Venues, ChoirGroups, SongList, Download, Create}

    public class ServiceManager : MonoBehaviour
    {
        private const string url = "https://api.allaboutonline.co.uk/", api = "api.php?url=", songsList = "songlist.php?url=query%3Fq%3DFROM%2520Document%2520WHERE%2520!String.IsNullOrEmpty(Name)%2520ORDER%2520BY%2520Name%2520ASC",
    token = "&token=", contactQuery = "query%3Fq%3DFROM%2520Force.Force__Contact%2520WHERE%2520Email%3D",
    sessionQuery = "query%3Fq%3DFROM%2520Force.Force__Choir_Session__c%2520WHERE%2520Choir_Group__c",
    venueQuery = "query%3Fq%3DFROM%2520Force.Force__Venue__c", choirGroup = "query%3Fq%3DFROM%2520Force.Force__Choir_Group__c",
    createQuery = "create.php?sessionID=", apiDownload = "download.php?url=https://members.rockchoir.com/rest/2.0/Document/";
        
        public delegate void onLoginComplete();
        private delegate WWW QueryAction(string[] data);
        private delegate object ResponseAction(WWW response);
        
        public static event onLoginComplete loginComplete;
        
        private Dictionary<RequestType, QueryAction> queries;
        private Dictionary<RequestType, ResponseAction> responses;

        public JSONObject userInfo { get; private set; }
        public JSONObject contactInfo { get; private set; }
        
        private void Awake()
        {
            queries = new Dictionary<RequestType, QueryAction>();

            queries.Add(RequestType.Login, (data) => new WWW( url + "login.php?username=" + data[0] + "&password=" +data[1].Replace("&", "%26") ));
            queries.Add(RequestType.ContactInfo, (data) => new WWW( url + api + contactQuery + '"' + data[0] + '"' + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.SongList, (data) => new WWW( url + songsList + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.Sessions, (data) => new WWW( url + api + sessionQuery + "=%2520%22" + data[0] + "%22" + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.Venues, (data) => new WWW(url + api + venueQuery + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.ChoirGroups, (data) => new WWW(url + api + choirGroup + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.Create, (data) => new WWW(url + createQuery + data[0] + "&qrdata=" + data[1] + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.Download, (data) => new WWW(url + apiDownload + data[0] + "&name=" + data[1] + "&url=" + url + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.EmergencyContact, (data) => new WWW(url + api + "query%3Fq%3DFROM%2520Force.Force__Emergency_Contact__c" + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.UpdatePersonalInfo, (data) => new WWW(url + "UpdateData.php?FirstName=" + data[0] + "&LastName=" + data[1] + "&Email=" + data[2] + "&Name=" + data[0] + " " + data[1] + "&OtherState=" + data[3] + "&OtherStreet=" + data[4] + "&Birthdate=" + data[5] + "&OtherCity=" + data[6] + "&OtherCountry=" + data[7] + "&OtherPostalCode=" + data[8] + "&Phone=" + data[9] + "&Id=" + data[10] + "&Voice_Type__c=" + data[11] + "&Gender__c=" + data[12] + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.BillingHistory, (data) => new WWW(url + api + "query%3Fq%3DFROM%2520Force.Force__Payment__c%2520WHERE%2520Status__c%2520!=%2520\"Pending\"" + token + CleanEntry(userInfo["SessionId"]) ));
            queries.Add(RequestType.SessionCalendar, (data) => new WWW( url + "SessionList.php?url=" + "query%3Fq%3DFROM%2520Force.Force__Attendance__c" + token + CleanEntry(userInfo["SessionId"]) ));

            responses = new Dictionary<RequestType, ResponseAction>();

            responses.Add(RequestType.Login, (response) => (userInfo = ValidateJSON(response)));
            responses.Add(RequestType.ContactInfo, (response) => (contactInfo = ValidateJSON(response)));
            responses.Add(RequestType.SongList, (response) => ValidateJSON(response));
            responses.Add(RequestType.Venues, (response) => ValidateJSON(response));
            responses.Add(RequestType.ChoirGroups, (response) => ValidateJSON(response));
            responses.Add(RequestType.EmergencyContact, (response) => ValidateJSON(response));
            responses.Add(RequestType.UpdatePersonalInfo, (response) => ValidateJSON(response));
            responses.Add(RequestType.BillingHistory, (response) => ValidateJSON(response));
            responses.Add(RequestType.SessionCalendar, (response) => ValidateJSON(response));
            responses.Add(RequestType.Sessions, (response) => ValidateJSON(response));
            responses.Add(RequestType.Create, (response) => {
                if(response.error != null && response.error.Length <= 0)
                {
                    if (response.text.Contains("{"))
                    {
                        return new JSONObject('{' + response.text.Split('{')[1]);
                    }
                    else if (response.text.Contains(":"))
                    {
                        JSONObject newJSON = new JSONObject();
                        newJSON.AddField("BadToken", response.text.Split('\"')[1]);

                        return newJSON;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return ValidateJSON(response);
                }
            });
            responses.Add(RequestType.Download, (response) => new JSONObject(response.text)); // Not yet handled correctly
        }

        public IEnumerator MakeRequest(RequestType reqType, Action<object> result, string[] sendInfo = null)
        {
            TopMenuManager.managerInstance.loadingAnimation = true;

            WWW www = queries[reqType].Invoke(sendInfo);

            #if DEBUG || DEVELOPMENT_BUILD
            Debug.Log(www.url);
            #endif

            yield return www;

            TopMenuManager.managerInstance.loadingAnimation = false;

            #if DEBUG || DEVELOPMENT_BUILD
            Debug.Log(www.text);
            #endif
            
            result(responses[reqType].Invoke(www));
        }

        // Public accessor for login
        public static void LoginComplete()
        {
            if (loginComplete != null)
            {
                loginComplete();
            }
        }

        private JSONObject ValidateJSON(WWW www)
        {
            JSONObject newJSON = null;
            if (www.error != null && www.error.Length > 0)
            {
                newJSON = new JSONObject();
                newJSON.AddField("CustomError", "No internet connection");

                // Turn off visually when not connected to internet
                TopMenuManager.managerInstance.internetConnection = false;
            }
            else
            {
                newJSON = new JSONObject(www.text);
            }
            return newJSON;
        }
        
        private string CleanEntry(JSONObject dirty)
        {
            return dirty != null ? dirty.str.Replace("/", "").Replace("\"", "") : string.Empty;
        }
    }
}