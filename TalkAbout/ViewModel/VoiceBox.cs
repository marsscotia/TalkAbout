using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace TalkAbout.ViewModel
{
    public class VoiceBox
    {
        private MediaElement _media;
        private SpeechSynthesizer _speech;
        private Settings _settings;
        private Pronunciations _pronunciations;
        private Dictionary<string, string> _pronunciationDictionary;

        public MediaElement Media
        {
            private get
            {
                return _media;
            }
            set
            {
                _media = value;
            }
        }

        public VoiceBox()
        {
            _settings = Settings.Instance;
            _pronunciations = Pronunciations.Instance;
            _pronunciationDictionary = new Dictionary<string, string>();
            _speech = new SpeechSynthesizer();
            _speech.Voice = _settings.SettingsVoice;
            _loadPronunciations();
        }

        private void _loadPronunciations()
        {
            foreach (var pronunciation in _pronunciations.PronunciationList)
            {
                _pronunciationDictionary.Add(pronunciation.Word.ToLower(), pronunciation.Sound);
            }
        }

        private string _parsePronunciations(string utterance)
        {
            string result = "";

            int wordStart = 0;
            int wordEnd = 0;
            int index = 0;
            bool inWord = false;
            bool wordFound = false;
            while (index < utterance.Length)
            {
                if (index == utterance.Length - 1)
                {
                    //we're at the end of the utterance
                    if (inWord)
                    {
                        wordEnd = index;
                        inWord = false;
                        wordFound = true;
                    }
                    else
                    {
                        index++;
                    }
                }
                else if (Char.IsLetterOrDigit(utterance[index])
                    || utterance[index].Equals('-')
                    || utterance[index].Equals('\''))
                {
                    if (!inWord)
                    {
                        wordStart = index;
                        inWord = true;
                        
                    }
                    index++;
                }
                else
                {
                    if (inWord)
                    {
                        wordEnd = index - 1;
                        inWord = false;
                        wordFound = true;
                    }
                    else
                    {
                        index++;
                    }
                }

                if (wordFound)
                {
                    string word = utterance.Substring(wordStart, (wordEnd - wordStart) + 1);
                    if (_pronunciationDictionary.ContainsKey(word.ToLower()))
                    {
                        string pronunciation = _pronunciationDictionary[word.ToLower()];
                        utterance = utterance.Remove(wordStart, word.Length);
                        utterance = utterance.Insert(wordStart, pronunciation);
                        index = index - word.Length + pronunciation.Length;
                        wordFound = false;
                    }
                    else
                    {
                        wordFound = false;
                        index++;
                    }
                }
            }
            result = utterance;
            return result;
        }

        public async Task Speak(string utterance)
        {
            if (!string.IsNullOrWhiteSpace(utterance))
            {
                if (_pronunciationDictionary.Count > 0)
                {
                    utterance = _parsePronunciations(utterance);
                }
                SpeechSynthesisStream stream = await _speech.SynthesizeTextToStreamAsync(utterance);

                if (_media != null)
                {
                    Debug.WriteLine("VoiceBox.cs: About to set stream and play " + utterance);
                    _media.AutoPlay = true;
                    _media.SetSource(stream, stream.ContentType);
                    _media.Volume = _settings.VoiceVolume / 100;
                    _media.Play();
                }
            }
        }



    }
}
