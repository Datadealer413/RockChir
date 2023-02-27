using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public enum View { Login, OptionalMenu, LeaderMenu, MemberMenu, Session, Scan, Send, SongLibrary, BillingHistory, PersonalInfoMenu, PersonalInfo, EmergencyInfo, SessionCalendar }

    public class SwitchView : MonoBehaviour
    {
        public static SwitchView instance { get { return _switchView; } }

        [SerializeField] private GameObject[] views;
        [SerializeField] private View startingView;
        private static SwitchView _switchView;

        private View currentView;

        private void Awake()
        {
            _switchView = this;
        }

        private void Start()
        {
            ChangeView(startingView);
        }

        public void OverrideCurrentView(string view)
        {
            currentView = Utility.ToEnum<View>(view);

#if DEBUG || DEVELOPMENT_BUILD
            Debug.Log(view);
#endif
        }

        // Change view 
        public void ChangeView(View _viewType)
        {
#if DEBUG || DEVELOPMENT_BUILD
            Debug.Log(_viewType.ToString());
#endif

            for (int i = 0; i < views.Length; i++)
            {
                IViewable viewable = views[i].gameObject.GetComponent<IViewable>();

                if (_viewType.ToString() == views[i].name)
                {
                    if (viewable != null)
                    {
                        viewable.SetView(true);
                        currentView = _viewType;
                    }
                    else
                    {
#if DEBUG || DEVELOPMENT_BUILD
                        Debug.Log("Unable to find IViewable interface on object " + views[i].name);
#endif
                    }
                }
                else
                {
                    viewable.SetView(false);
                }
            }
        }

        public void ChangeView(string _viewType)
        {
#if DEBUG || DEVELOPMENT_BUILD
            Debug.Log(_viewType);
#endif

            View newView = Utility.ToEnum<View>(_viewType);

            for (int i = 0; i < views.Length; i++)
            {
                IViewable viewable = views[i].gameObject.GetComponent<IViewable>();

                if (_viewType.ToString() == views[i].name)
                {
                    if (viewable != null)
                    {
                        viewable.SetView(true);
                        currentView = newView;
                    }
                    else
                    {
#if DEBUG || DEVELOPMENT_BUILD
                        Debug.Log("Unable to find IViewable interface on object " + views[i].name);
#endif
                    }
                }
                else
                {
                    viewable.SetView(false);
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }
        }

        public void GoBack()
        {
            if (currentView == View.LeaderMenu)
            {
                ChangeView(View.OptionalMenu);
            }
            else if (currentView == View.OptionalMenu)
            {
                ChangeView(View.Login);
            }
            else if (currentView == View.MemberMenu)
            {
                LoginType login = Utility.ToEnum<LoginType>(SaveData.saveData.loginType);
                if (login == LoginType.ConfirmedMember)
                {
                    ChangeView(View.Login);
                }
                else if (login == LoginType.ActiveLeader)
                {
                    ChangeView(View.OptionalMenu);
                }
                else
                {
                    ChangeView(View.Login);
                }
            }
            else if (currentView == View.Login)
            {
                Application.Quit();
            }
            else
            {
                if(currentView == View.BillingHistory || currentView == View.SessionCalendar || currentView == View.SongLibrary || currentView == View.PersonalInfoMenu)
                {
                    ChangeView(View.MemberMenu);
                }
                else if (currentView == View.PersonalInfo || currentView == View.EmergencyInfo)
                {
                    ChangeView(View.PersonalInfoMenu);
                }
                else if(currentView == View.Scan || currentView == View.Session || currentView == View.Send)
                {
                    ChangeView(View.LeaderMenu);
                }
                else
                {
                    ChangeView(View.Login);
                }
            }
        }
    }
}