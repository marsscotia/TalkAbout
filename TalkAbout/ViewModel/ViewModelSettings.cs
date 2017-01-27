using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;
using Windows.Media.SpeechSynthesis;

namespace TalkAbout.ViewModel
{

    /// <summary>
    /// 
    /// Class provides a view model for a settings view.
    /// 
    /// </summary>
    public class ViewModelSettings: BindableBase
    {
        private Settings _settings;
        private List<VoiceInformation> _voices;
        private List<int> _fontSizes;

        private int[] _fontSizeRange = { 12, 14, 16, 18, 20, 24, 26, 28, 30, 32, 36 };

        private bool _filterPhrases;
        private bool _showShortcuts;
        private bool _useCategories;
        private bool _speakWords;
        private bool _speakSentences;
        private bool _speakPhrases;
        private bool _showNavigation;
        private bool _showSorting;
        private int _fontSize;
        private int _voiceVolume;
        private int _showIconsAndLabels;
        private VoiceInformation _voice;

        private const int _iconsAndLabels = 1;
        private const int _iconsOnly = 2;
        private const int _labelsOnly = 3;

        public bool FilterPhrases
        {
            get
            {
                return _settings.FilterPhrases;
            }
            set
            {
                SetProperty(ref _filterPhrases, value);
                _settings.FilterPhrases = value;
            }
        }

        public bool ShowShortcuts
        {
            get
            {
                return _showShortcuts;
            }
            set
            {
                SetProperty(ref _showShortcuts, value);
                _settings.ShowShortcuts = value;
            }
        }

        public bool UseCategories
        {
            get
            {
                return _useCategories;
            }
            set
            {
                SetProperty(ref _useCategories, value);
                _settings.UseCategories = value;
            }
        }

        public bool SpeakWords
        {
            get
            {
                return _speakWords;
            }
            set
            {
                SetProperty(ref _speakWords, value);
                _settings.SpeakWords = value;
            }
        }

        public bool SpeakSentences
        {
            get
            {
                return _speakSentences;
            }
            set
            {
                SetProperty(ref _speakSentences, value);
                _settings.SpeakSentences = value;
            }
        }

        public bool SpeakPhrases
        {
            get
            {
                return _speakPhrases;
            }
            set
            {
                SetProperty(ref _speakPhrases, value);
                _settings.SpeakPhrases = value;
            }
        }

        public bool ShowNavigation
        {
            get
            {
                return _showNavigation;
            }
            set
            {
                SetProperty(ref _showNavigation, value);
                _settings.ShowNavigation = value;
            }
        }

        public bool ShowSorting
        {
            get
            {
                return _showSorting;
            }
            set
            {
                SetProperty(ref _showSorting, value);
                _settings.ShowSorting = value;
            }
        }

        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                SetProperty(ref _fontSize, value);
                _settings.FontSize = value;
            }
        }

        public int VoiceVolume
        {
            get
            {
                return _voiceVolume;
            }
            set
            {
                SetProperty(ref _voiceVolume, value);
                _settings.VoiceVolume = value;
            }
        }

        public VoiceInformation Voice
        {
            get
            {
                return _voice;
            }
            set
            {
                SetProperty(ref _voice, value);
                _settings.SettingsVoice = value;
            }
        }

        public int ShowIconsAndLabels
        {
            get
            {
                return _showIconsAndLabels;
            }
            set
            {
                SetProperty(ref _showIconsAndLabels, value);
                OnPropertyChanged("ValueAsIconsAndLabels");
                OnPropertyChanged("ValueAsIconsOnly");
                OnPropertyChanged("ValueAsLabelsOnly");
                _settings.ShowIconsAndLabels = value;
            }
        }

        public bool ValueAsIconsAndLabels
        {
            get
            {
                return ShowIconsAndLabels.Equals(_iconsAndLabels);
            }
            set
            {
                ShowIconsAndLabels = _iconsAndLabels;
            }
        }

        public bool ValueAsIconsOnly
        {
            get
            {
                return ShowIconsAndLabels.Equals(_iconsOnly);
            }
            set
            {
                ShowIconsAndLabels = _iconsOnly;
            }
        }

        public bool ValueAsLabelsOnly
        {
            get
            {
                return ShowIconsAndLabels.Equals(_labelsOnly);
            }
            set
            {
                ShowIconsAndLabels = _labelsOnly;
            }
        }

        public string VoiceId
        {
            get
            {
                return _voice.Id;
            }
            set
            {
                Voice = (from VoiceInformation voice in SpeechSynthesizer.AllVoices
                         where voice.Id == value
                         select voice).DefaultIfEmpty(SpeechSynthesizer.DefaultVoice).First();


            }
        }

        public List<VoiceInformation> Voices
        {
            get
            {
                return _voices;
            }
        }

        public List<int> FontSizes
        {
            get
            {
                return _fontSizes;
            }
        }


        public ViewModelSettings()
        {
            _settings = Settings.Instance;
            _voices = new List<VoiceInformation>(SpeechSynthesizer.AllVoices);
            _fontSizes = new List<int>(_fontSizeRange);

            _filterPhrases = _settings.FilterPhrases;
            _showShortcuts = _settings.ShowShortcuts;
            _useCategories = _settings.UseCategories;
            _speakWords = _settings.SpeakWords;
            _speakSentences = _settings.SpeakSentences;
            _speakPhrases = _settings.SpeakPhrases;
            _showNavigation = _settings.ShowNavigation;
            _showSorting = _settings.ShowSorting;
            _fontSize = _settings.FontSize;
            _voice = _settings.SettingsVoice;
            _voiceVolume = _settings.VoiceVolume;
            _showIconsAndLabels = _settings.ShowIconsAndLabels;

            
        }


    }
}
