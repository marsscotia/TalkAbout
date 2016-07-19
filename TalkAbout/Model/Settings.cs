using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;

namespace TalkAbout.Model
{

    
    
    /// <summary>
    /// 
    /// A singleton class which represents the app settings. 
    /// 
    /// A user can adjust the settings via the Settings.xaml page.
    /// 
    /// </summary>
    public class Settings
    {


        //The singleton object
        private static Settings _settings;

        //Reference to roaming settings
        private ApplicationDataContainer _roamingSettings;

        //The settings
        private bool _filterPhrases;
        private bool _showShortcuts;
        private bool _useCategories;
        private bool _speakWords;
        private bool _speakSentences;
        private bool _speakPhrases;
        private bool _showNavigation;
        private bool _showSorting;
        private int _fontSize;
        private VoiceInformation _voice;

        //The strings for saving and retrieving the settings
        private const string _filterPhrasesKey = "filter_phrases";
        private const string _showShortcutsKey = "show_shortcuts";
        private const string _useCategoriesKey = "use_categories";
        private const string _speakWordsKey = "speak_words";
        private const string _speakSentencesKey = "speak_sentences";
        private const string _speakPhrasesKey = "speak_phrases";
        private const string _showNavigationKey = "show_navigation";
        private const string _showSortingKey = "show_sorting";
        private const string _fontSizeKey = "font_size";
        private const string _voiceKey = "voice";

        //Properties

        public bool FilterPhrases
        {
            get
            {
                return _filterPhrases;
            }
            set
            {
                _filterPhrases = value;
                _roamingSettings.Values[_filterPhrasesKey] = value;
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
                _showShortcuts = value;
                _roamingSettings.Values[_showShortcutsKey] = value;
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
                _useCategories = value;
                _roamingSettings.Values[_useCategoriesKey] = value;
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
                _speakWords = value;
                _roamingSettings.Values[_speakWordsKey] = value;
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
                _speakSentences = value;
                _roamingSettings.Values[_speakSentencesKey] = value;
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
                _speakPhrases = value;
                _roamingSettings.Values[_speakPhrasesKey] = value;
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
                _showNavigation = value;
                _roamingSettings.Values[_showNavigationKey] = value;
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
                _showSorting = value;
                _roamingSettings.Values[_showSortingKey] = value;
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
                _fontSize = value;
                _roamingSettings.Values[_fontSizeKey] = value;
            }
        }

        public VoiceInformation SettingsVoice
        {
            get
            {
                return _voice;
            }
            set
            {
                _voice = value;
                _roamingSettings.Values[_voiceKey] = value.Id;
            }
        }

        
        /// <summary>
        /// 
        /// Constructor is private to facilitate Singleton object pattern.
        /// 
        /// Constructor attempts to retrieve settings from roaming settings, 
        /// sets them to default if not.
        /// 
        /// </summary>
        private Settings()
        {
            //for each setting, we try to retrieve the value from the roaming store;
            //if it can't be found, we apply the default
            _roamingSettings = ApplicationData.Current.RoamingSettings;

            //Setting for filtering phrases. Default is on.
            object filterPhrases = _roamingSettings.Values[_filterPhrasesKey];
            if(filterPhrases == null)
            {
                _filterPhrases = true;
            }
            else
            {
                _filterPhrases = (bool)filterPhrases;
            }

            //Setting for showing keyboard shortcuts. Default is on.
            object showShortcuts = _roamingSettings.Values[_showShortcutsKey];
            if(showShortcuts == null)
            {
                _showShortcuts = true;
            }
            else
            {
                _showShortcuts = (bool)showShortcuts;
            }

            //Setting for using categories.  Default is on.
            object useCategories = _roamingSettings.Values[_useCategoriesKey];
            if (useCategories == null)
            {
                _useCategories = true;
            }
            else
            {
                _useCategories = (bool)useCategories;
            }

            //Setting for speaking words. Default is off.
            object speakWords = _roamingSettings.Values[_speakWordsKey];
            if (speakWords == null)
            {
                _speakWords = false;
            }
            else
            {
                _speakWords = (bool)speakWords;
            }

            //Setting for speaking sentences.  Default is off.
            object speakSentences = _roamingSettings.Values[_speakSentencesKey];
            if (speakSentences == null)
            {
                _speakSentences = false;
            }
            else
            {
                _speakSentences = (bool)speakSentences;
            }

            //Setting for speaking phrases as they're selected.  Default is off.
            object speakPhrases = _roamingSettings.Values[_speakPhrasesKey];
            if (speakPhrases == null)
            {
                _speakPhrases = false;
            }
            else
            {
                _speakPhrases = (bool)speakPhrases;
            }

            //Setting for showing navigation buttons.  Default is off.
            object showNavigation = _roamingSettings.Values[_showNavigationKey];
            if (showNavigation == null)
            {
                _showNavigation = false;
            }
            else
            {
                _showNavigation = (bool)showNavigation; 
            }

            //Setting for showing sorting buttons.  Default is on.
            object showSorting = _roamingSettings.Values[_showSortingKey];
            if (showSorting == null)
            {
                _showSorting = true;
            }
            else
            {
                _showSorting = (bool)showSorting;
            }

            //Setting for font.  Default is 12.
            object fontSize = _roamingSettings.Values[_fontSizeKey];
            if(fontSize == null)
            {
                _fontSize = 12;
            }
            else
            {
                _fontSize = (int)fontSize;
            }

            //Setting for voice.  Default is default system voice.
            bool found = false;
            object voiceId = _roamingSettings.Values[_voiceKey];
            if (voiceId != null)
            {
                string voiceIdString = (string)voiceId;
                Debug.WriteLine("Settings.cs: Voice id loaded from roaming settings is: " + voiceIdString);
                foreach(VoiceInformation voice in SpeechSynthesizer.AllVoices)
                {
                    if (voice.Id.Equals(voiceIdString))
                    {
                        _voice = voice;
                        found = true;
                    }
                }
            }
            if (!found)
            {
                _voice = SpeechSynthesizer.AllVoices.FirstOrDefault();
                Debug.WriteLine("Settings.cs: Voice id loaded from default is: " + _voice.Id);
            }

        }

        public static Settings Instance
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }
                return _settings;
            }
        }

    }
}
