using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private Text songName, currentTimeTxt, totalTimeTxt;
        [SerializeField] private Slider durationSlider;
        [SerializeField] Button playBtn, previousBtn, nextBtn;
        [SerializeField] Sprite play, pause;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<Song> songsList = new List<Song>();
        [SerializeField] private GameObject templateSong, rootObject;
        [SerializeField] private ColorSet[] colors;
        private bool loadedSongs = false, isPlaying = false, holdingSlider = false;
        private int currentIndex;

        [SerializeField] private AudioClip currentClip;

        #region UI
        public void Play(Song song)
        {
            StopCoroutine("SliderAnimation");
            
            currentClip = song.clip;
            songName.text = song.songName;
            
            currentTimeTxt.text = "0";

            totalTimeTxt.text = ConvertToTime(song.clip.length);
            durationSlider.value = 0;
            currentIndex = song.index;

            audioSource.clip = currentClip;
            audioSource.Play();
            isPlaying = true;
            playBtn.GetComponent<Image>().sprite = pause;
            StartCoroutine("SliderAnimation");
        }

        public void TogglePlay()
        {
            if(currentClip != null)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                    playBtn.GetComponent<Image>().sprite = play;
                }
                else
                {
                    audioSource.UnPause();
                    playBtn.GetComponent<Image>().sprite = pause;
                }
            }
        }

        public void Next()
        {
            if(currentIndex + 1 < songsList.Count)
            {
                currentIndex++;
                Play(songsList[currentIndex]);
            }
            else
            {
                currentIndex = 0;
                Play(songsList[currentIndex]);
            }
        }

        public void Previous()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                Play(songsList[currentIndex]);
            }
            else
            {
                currentIndex = songsList.Count - 1;
                Play(songsList[currentIndex]);
            }
        }

        public void BeginDrag()
        {
            holdingSlider = true;
            audioSource.Pause();
        }

        public void EndDrag()
        {
            audioSource.time = currentClip.length * durationSlider.value;
            currentTimeTxt.text = ConvertToTime(audioSource.time);
            audioSource.UnPause();
            holdingSlider = false;
        }

        public void Dragged()
        {
            currentTimeTxt.text = ConvertToTime(currentClip.length * durationSlider.value);
        }
        #endregion

        public void GetSongs()
        {
            if (!loadedSongs)
            {
                loadedSongs = true;
                StartCoroutine(GetSongsProcess());
            }
            
        }

        private IEnumerator GetSongsProcess()
        {
            var info = new DirectoryInfo(Application.persistentDataPath + "/Downloads/");
            FileInfo[] fileInfo = info.GetFiles();

            int iColor = 0;
            for (int i=0; i < fileInfo.Length; i++)
            {
                WWW www = new WWW(@"file://" + fileInfo[i].FullName);
                yield return www;
                
                GameObject obj = Instantiate(templateSong, rootObject.transform);
                AudioClip clp = www.GetAudioClip(false);

                Song song = obj.GetComponent<Song>();

                song.SetupData(this, fileInfo[i].Name, clp, colors[iColor].background, colors[iColor].font, i);
                iColor = iColor + 1 < colors.Length ? iColor + 1 : 0;

                songsList.Add(song);
            }
        }

        private IEnumerator SliderAnimation()
        {
            while (isPlaying)
            {
                if (!holdingSlider)
                {
                    durationSlider.value = audioSource.time / currentClip.length;
                    currentTimeTxt.text = ConvertToTime(audioSource.time);
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }

        private string ConvertToTime(float duration)
        {
            int minutes = Mathf.FloorToInt(duration / 60);
            int seconds = Mathf.FloorToInt(duration % 60);
            
            return seconds > 9 ? minutes + ":" + seconds: minutes + ":" + "0" + seconds;
        }
    }
}