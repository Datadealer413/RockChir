using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public enum LoginType { ConfirmedMember, ActiveLeader, InactiveTester, InactiveMember, Unknown }

    public class Login : MonoBehaviour, IViewable {
    
        [SerializeField] private Animation logoAnim;
        [SerializeField] private Button loginButton;
        [SerializeField] private Animator anim;
        [SerializeField] public Button scanBtn, sendBtn;
        [SerializeField] private InputField username, password;
        [SerializeField] private ServiceManager serviceManager;
        private bool loggingIn = false;

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
                TopMenuManager.managerInstance.visible = false;
            }

            gameObject.SetActive(viewActive);
        }

        // Starts process. Buttons cannot call Einumerators
        public void TryLogin()
        {
            if (!loggingIn)
            {
                //serviceManager.GetComponent<FontManager>()
                StartCoroutine(LoginProcess());
            }
        }
        
        // Used by other classes to pop up a message in login view
        public void Message(string msg)
        {
            StartCoroutine(LoginFailedAnim(msg));
        }

        private IEnumerator LoginProcess()
        {
            // Aesthetics only
            if (logoAnim && loginButton)
            {
                logoAnim.Play("Spin");

                Color _color = new Color();
                ColorUtility.TryParseHtmlString("C8C8C8FF", out _color);
                loginButton.GetComponent<Image>().color = _color;
            }

#if DEBUG || DEVELOPMENT_BUILD
            // Test Login - For debugging only
            if (username.text.Length < 1)
            {
                username.text = "john.wilson@rockchoir.com";
                password.text = "badPassword!";
            }
#endif

            // Login request
            JSONObject loginData = null;
            yield return StartCoroutine( serviceManager.MakeRequest(RequestType.Login, value => loginData = value as JSONObject, new string[] { username.text, password.text }));

            JSONObject contactData = null;
            // Contact info request
            if (loginData != null && loginData.HasField("SessionId") && !loginData["SessionId"].IsNull && loginData["IsSuccess"] == true)
            {
                yield return StartCoroutine( serviceManager.MakeRequest(RequestType.ContactInfo, value => contactData = value as JSONObject, new string[] { username.text }) );
            }

            // Aesthetics only
            while (logoAnim.isPlaying)
            {
                yield return new WaitForFixedUpdate();
            }
            
            // Aesthetics and event call
            if (loginData && loginData.HasField("SessionId") && !loginData["SessionId"].IsNull && loginData["IsSuccess"] == true)
            {
                if(contactData["Records"].Count > 0)
                {
                    HandleMemberStatus(contactData["Records"][0]["Member_Status__c"].str);
                }
                else
                {
                    StartCoroutine(LoginFailedAnim("USER DETAILS NOT FOUND"));
                }
            }
            else
            {
                if (loginData != null && loginData.HasField("CustomError"))
                {
                    StartCoroutine(LoginFailedAnim(loginData["CustomError"].str.ToUpper()));
                }
                else
                {
                    StartCoroutine(LoginFailedAnim("PLEASE CHECK USERNAME & PASSWORD"));
                }
            }
        }

        private void HandleMemberStatus(string loginType)
        {

            SaveData.saveData.loginType = loginType;

            switch (loginType)
            {
                case "Active Choir Leader":
                    {
                        SwitchView.instance.ChangeView(View.OptionalMenu);
                        loginButton.GetComponent<Image>().color = Color.white;

                        #if DEBUG || DEVELOPMENT_BUILD
                        Debug.Log("Created user records");
                        #endif

                        // Start him on his thinking process, before any other classes does checks against saved data
                        SaveData.saveData.WakeMe();

                        // Call for any post login events
                        ServiceManager.LoginComplete();
                        loggingIn = false;
                        break;
                    }
                case "Confirmed Member":
                    {
                        SwitchView.instance.ChangeView(View.MemberMenu);
                        loginButton.GetComponent<Image>().color = Color.white;

                        #if DEBUG || DEVELOPMENT_BUILD
                        Debug.Log("Created user records");
                        #endif

                        // Start him on his thinking process, before any other classes does checks against saved data
                        SaveData.saveData.WakeMe();

                        // Call for any post login events
                        ServiceManager.LoginComplete();
                        loggingIn = false;
                        break;
                    }
                case "Active Tester":
                    {
                        StartCoroutine(LoginFailedAnim("EXCLUSIVE TO MEMBERS"));
                        break;
                    }
                case "Inactive Tester":
                    {
                        StartCoroutine(LoginFailedAnim("PLEASE RE-ACTIVATE MEMBERSHIP"));
                        break;
                    }
                case "Inactive Member":
                    {
                        StartCoroutine(LoginFailedAnim("PLEASE RE-ACTIVATE MEMBERSHIP"));
                        break;
                    }
                default:
                    {
                        StartCoroutine(LoginFailedAnim("CHECK USER STATUS"));
                        break;
                    }
            }
        }

        private IEnumerator LoginFailedAnim(string message)
        {
            anim.GetComponent<Text>().text = message;

            anim.SetBool("LoginFailed", true);

            yield return new WaitForSeconds(2f);

            anim.SetBool("LoginFailed", false);
        }
    }

}