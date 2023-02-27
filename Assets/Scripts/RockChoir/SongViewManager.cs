using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class SongViewManager : MonoBehaviour, IViewable
    {
        [SerializeField] private ServiceManager serviceManager;
        [SerializeField] private ResponsePanel responsePanel;
        [SerializeField] private RectTransform rootDownloadsObj;
        [SerializeField] private GameObject content, downloadableObjTemplate;
        [SerializeField] private GameObject songLibrary, downadedLibrary;
        [SerializeField] private ColorSet[] colors;
        //[SerializeField] private List<DownloadableSong> songsLibrary = new List<DownloadableSong>(); // For now this is commented out to avoid console warnings, however it might be used later.
        
        private bool loadedSongs = false;

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
                TopMenuManager.managerInstance.menuTitle = "SONG LIBRARY";
                downadedLibrary.SetActive(true);

                if (!loadedSongs)
                {
                    responsePanel.response = "Loading";
                    StartCoroutine(DisplaySongs());
                }
            }
        }

        public void SwitchSongLibraryView()
        {

        }
        
        private void Awake()
        {
            ServiceManager.loginComplete += GetSongs;
        }

        private void GetSongs()
        {
            if (Utility.ToEnum<LoginType>(SaveData.saveData.loginType) == LoginType.ConfirmedMember && !loadedSongs)
            {
                TopMenuManager.managerInstance.loadingAnimation = true;
                StartCoroutine(DisplaySongs());
            }
        }

        private IEnumerator DisplaySongs()
        {
            loadedSongs = true;
            JSONObject response = null;
            yield return StartCoroutine(serviceManager.MakeRequest(RequestType.SongList, value => response = value as JSONObject));

            if(response != null && response.Count > 0)
            {
                if (!response.HasField("CustomError"))
                {
                    responsePanel.visible = false;

                    int iColor = 0;
                    for (int i = 0; i < response.Count; i++)
                    {
                        GameObject obj = Instantiate(downloadableObjTemplate, rootDownloadsObj);
                        obj.GetComponent<DownloadableSong>().SetSong(response[i], colors[iColor].background, colors[iColor].font);
                        iColor = iColor + 1 < colors.Length ? iColor + 1 : 0;
                        yield return new WaitForFixedUpdate();
                    }
                }
                else
                {
                    responsePanel.response = "NO INTERNET CONNECTION";
                }                
            }
            else
            {
                responsePanel.response = "NO SONGS AVAILABLE";
            }

            TopMenuManager.managerInstance.loadingAnimation = false;
        }

        public void DownloadSong(string id, string fileName)
        {
            StartCoroutine(DownloadSongProcess(id, fileName));
        }
        
        private IEnumerator DownloadSongProcess(string id, string fileName)
        {
            AudioClip clip = null;
            yield return serviceManager.MakeRequest(RequestType.Download, value => clip = value as AudioClip, new string[] { id, fileName } );

#if DEBUG || DEVELOPMENT_BUILD
            Debug.Log(clip);
#endif
        }
    }
}
