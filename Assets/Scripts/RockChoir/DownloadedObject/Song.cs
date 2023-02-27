using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class Song : DownloadedObject
    {
        public string songName { get { return _songName.text; } }
        public int index { get { return _songIndex; } }

        private int _songIndex;
        [SerializeField] private Text _songName;
        [SerializeField] private Image background;
        [SerializeField] private MusicPlayer musicPlayer;
        public AudioClip clip { get; private set; }

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => { musicPlayer.Play(this); });
        }

        public void SetupData(MusicPlayer _musicPlayer, string _SongName, AudioClip _clip, Color bgColor, Color fontColor, int _index)
        {
            background.color = bgColor;
            _songName.color = fontColor;
            musicPlayer = _musicPlayer;
            _songIndex = _index;
            _songName.text = _SongName;
            clip = _clip;
            clip.name = _SongName;
        }

        public void SetAudioClip(AudioClip _clip)
        {
            clip = _clip;
        }
    }
}