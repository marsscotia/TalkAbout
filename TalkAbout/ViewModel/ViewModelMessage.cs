using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;

namespace TalkAbout.ViewModel
{
    public class ViewModelMessage: BindableBase, ITaskCompleteNotifier
    {
        private string _message;
        private string _oldMessage;
        private string _workingMessage;
        private string _lastWord;
        private string _lastSentence;
        private int _selectionStart;
        private int _oldSelectionStart;
        private int _selectionLength;
        private bool _enabled;
        private bool _filtered;
        private TaskNotifier _notifier;
        private Abbreviations _abbreviations;
        private Settings _settings;
        private Dictionary<String, String> _abbreviationDictionary;
        private VoiceBox _voiceBox;

        public String Message
        {
            get
            {
                return _message;
            }
            set
            {
                _oldMessage = _message;
                _workingMessage = value;
                SetProperty(ref _message, value);
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
        }

        public bool Filtered
        {
            get
            {
                return _filtered;
            }
            set
            {
                SetProperty(ref _filtered, value);
            }
        }


        public int CaretIndex
        {
            get
            {
                return _selectionStart;
            }
            set
            {
                _oldSelectionStart = _selectionStart;
                SetProperty(ref _selectionStart, value);
            }
        }

        public int SelectionLength
        {
            get
            {
                return _selectionLength;
            }
            set
            {
                
                SetProperty(ref _selectionLength, value);
            }
        }

        public string LastWord
        {
            get
            {
                return _lastWord;
            }
        }

        public string LastSentence
        {
            get
            {
                return _lastSentence;
            }
        }
        
        public VoiceBox VoiceBox
        {
            get
            {
                return _voiceBox;
            }
        }

        public ViewModelMessage()
        {
            _notifier = new TaskNotifier(this);
            _selectionStart = 0;
            _selectionLength = 0;
            _message = "";
            _oldMessage = "";
            _oldSelectionStart = 0;
            _abbreviations = Abbreviations.Instance;
            _settings = Settings.Instance;
            _abbreviationDictionary = new Dictionary<string, string>();
            _loadAbbreviations();
            _voiceBox = new VoiceBox();
        }

        public void Update()
        {
            _update();
        }

        public void PhraseSelected(ViewModelPhrase phrase)
        {
            _phraseSelected(phrase);
        }

        public void DeleteWord()
        {
            _deleteWord();
        }

        public async Task Speak()
        {
            await _speakMessage();
        }

        public void Clear()
        {
            _clear();
        }

        public void Insert(string text)
        {
            _insert(text);
        }

        private void _loadAbbreviations()
        {
            _notifier.Execute(_abbreviations.LoadAbbreviationsFromFile(), "abbreviations");
        }

        private void _populateAbbreviations()
        {
            foreach (var abbreviation in _abbreviations.AbbreviationList)
            {
                _abbreviationDictionary.Add(abbreviation.Shortcut, abbreviation.Expansion);
            }
        }

        private bool _abbreviationExists(string shortcut)
        {
            bool result = false;
            if (_abbreviationDictionary.ContainsKey(shortcut))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Method adds a space immediately before the cursor if there is not one already
        /// </summary>
        private void _ensureLastSpace()
        {
            if (!_workingMessage[_selectionStart - 1].Equals(' '))
            {
                _workingMessage = _workingMessage.Insert(_selectionStart, " ");
                _selectionStart = _selectionStart + 1;
            }
        }

        private void _update()
        {
            _findLastWord();
            _findLastSentence();

            //we only update if there is no selection in the message window
            if (_selectionLength == 0)
            {
               if ((_selectionStart == _oldSelectionStart + 1) && (_workingMessage.Length == _oldMessage.Length + 1))
                {
                    //the message has been incremented by one
                    
                    if (_workingMessage[_selectionStart - 1].Equals(' '))
                    {
                        //if the last character entered was a space, we check for abbreviations
                        if (_abbreviationExists(_lastWord.Trim()))
                        {
                            Debug.WriteLine("ViewModelMessage.cs: Abbreviation found");
                            string expansion = _abbreviationDictionary[_lastWord.Trim()];
                            _workingMessage = _workingMessage.Replace(_lastWord, expansion);
                            _selectionStart = _selectionStart + (expansion.Length - _lastWord.Length);                            
                            _ensureLastSpace();
                            _message = _workingMessage;
                            Debug.WriteLine("ViewModelMessage.cs: New message is " + _workingMessage);
                            OnPropertyChanged("Message");
                            OnPropertyChanged("CaretIndex");
                        }
                    }
                    else
                    {
                        if (_isTerminationCharacter(_workingMessage[_selectionStart - 1]))
                        {
                            //if the last character entered was a termination character, we might need to speak the sentence
                            //TODO: Speak the sentence
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Method deletes the word immediately in front of the cursor.
        /// </summary>
        private void _deleteWord()
        {
            if (!String.IsNullOrEmpty(_message))
            {
                _oldMessage = _message;
                _oldSelectionStart = _selectionStart;
                _findLastWord();
                int wordStart = _message.LastIndexOf(_lastWord);
                _workingMessage = _message.Remove(wordStart, _lastWord.Length);
                _message = _workingMessage;
                _selectionStart = _selectionStart - _lastWord.Length;
                OnPropertyChanged("Message");
                OnPropertyChanged("CaretIndex");
            }
        }

        /// <summary>
        /// Method clears the message window.
        /// </summary>
        private void _clear()
        {
            if (!String.IsNullOrEmpty(_message))
            {
                _oldMessage = _message;
                _oldSelectionStart = _selectionStart;
                _message = "";
                _selectionStart = 0;
                _lastSentence = "";
                _lastWord = "";
                OnPropertyChanged("Message");
                OnPropertyChanged("CaretIndex");
                OnPropertyChanged("LastWord");
                OnPropertyChanged("LastSentence");
            }
        }

        /// <summary>
        /// Method inserts the string passed as a parameter
        /// into the message at the current cursor position
        /// </summary>
        /// <param name="text"></param>
        private void _insert(String text)
        {
            _oldMessage = _message;
            _oldSelectionStart = _selectionStart;
            _workingMessage = _message;
            _workingMessage = _workingMessage.Insert(_selectionStart, text);
            _selectionStart = _selectionStart + text.Length;
            _ensureLastSpace();
            _message = _workingMessage;
            OnPropertyChanged("Message");
            OnPropertyChanged("CaretIndex");
        }


        /// <summary>
        /// Method finds the last word before the caret in the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private void _findLastWord()
        {

            if (!String.IsNullOrWhiteSpace(_workingMessage))
            {
                //we'll use this in the loop to determine whether we've found the start of the word
                bool startFound = false;
                //we'll use this in the loop to determine whether we've found the end of the word
                bool endFound = false;
                int index = _selectionStart - 1;
                //this will be the index at which the last word begins, we'll use it to trim it off the message
                int wordStart = 0;
                while (!endFound && index >= 0)
                {
                    if (Char.IsLetterOrDigit(_workingMessage[index]) || index == 0)
                    {
                        endFound = true;
                        Debug.WriteLine("ViewModelMessage.cs: End of word found at character " + index);
                    }
                    if (index > 0)
                    {
                        index--;
                    }
                }
                if (endFound)
                {
                    while (!startFound && index >= 0)
                    {
                        if (!Char.IsLetterOrDigit(_workingMessage[index]) || index == 0)
                        {
                            startFound = true;
                            //if we're not at the start of the text box, we add one back to start on the first letter of the last word
                            if (index > 0)
                            {
                                wordStart = index + 1;
                            }
                            else
                            {
                                wordStart = index;
                            }
                            Debug.WriteLine("ViewModelMessage.cs: Start of word found at character " + index);
                        }
                        if (index > 0)
                        {
                            index--;
                        }
                    }
                }
                if (startFound)
                {
                    _lastWord = _workingMessage.Substring(wordStart, _selectionStart - wordStart);
                    Debug.WriteLine("ViewModel.cs: Last word is " + _lastWord);
                } 
            }
            else
            {
                _lastWord = "";
            }
        }

        /// <summary>
        /// Method finds the last sentence before the caret in the message
        /// </summary>
        private void _findLastSentence()
        {
            bool startFound = false;
            bool endFound = false;
            int index = _selectionStart - 1;
            int sentenceStart = 0;
            while (!endFound && index >= 0)
            {
                if (!_isTerminationCharacter(_workingMessage[index]) || index == 0)
                {
                    endFound = true;
                }
                if(index > 0)
                {
                    index--; 
                }
            }
            if (endFound)
            {
                while (!startFound && index >= 0)
                {
                    if (_isTerminationCharacter(_workingMessage[index]) || index == 0)
                    {
                        startFound = true;
                        if (index > 0)
                        {
                            sentenceStart = index + 1; 
                        }
                        else
                        {
                            sentenceStart = index;
                        }
                    }
                    if (index > 0)
                    {
                        index--;
                    }
                }
            }
            if (startFound)
            {
                _lastSentence = _workingMessage.Substring(sentenceStart, _selectionStart - sentenceStart);
                Debug.WriteLine("ViewModelMessage.cs: Last Sentence is " + _lastSentence);
            }
        }

        private void _phraseSelected(ViewModelPhrase phrase)
        {
            if (_settings.FilterPhrases && Filtered)
            {
                if (!string.IsNullOrWhiteSpace(_lastSentence) 
                    && phrase.Name.Contains(_lastSentence))
                {
                    
                    _replaceLastSentence(phrase.Name);
                    
                }
                else
                {
                    if (phrase.Name.Contains(_lastWord))
                    {
                        _replaceLastWord(phrase.Name);
                    }
                }
            }
            else
            {
                _insert(phrase.Name);
            }
            
            if (_settings.SpeakPhrases)
            {
                //TODO: speak the phrase
            }
        }

        private void _replaceLastWord(string replacement)
        {
            _oldMessage = _message;
            _oldSelectionStart = _selectionStart;
            _workingMessage = _message;
            int pos = _workingMessage.LastIndexOf(_lastWord, _selectionStart - 1);
            _workingMessage = _workingMessage.Remove(pos, _lastWord.Length);
            _workingMessage = _workingMessage.Insert(pos, replacement);
            _selectionStart = _selectionStart - _lastWord.Length + replacement.Length;
            _ensureLastSpace();
            _message = _workingMessage;
            OnPropertyChanged("Message");
            OnPropertyChanged("CaretIndex");
        }

        private void _replaceLastSentence(string replacement)
        {
            _oldMessage = _message;
            _oldSelectionStart = _selectionStart;
            _workingMessage = _message;
            int pos = _workingMessage.LastIndexOf(_lastSentence, _selectionStart - 1);
            _workingMessage = _workingMessage.Remove(pos, _lastSentence.Length);
            _workingMessage = _workingMessage.Insert(pos, replacement);
            _selectionStart = _selectionStart - _lastSentence.Length + replacement.Length;
            _ensureLastSpace();
            _message = _workingMessage;
            OnPropertyChanged("Message");
            OnPropertyChanged("CaretIndex");
        }

        private bool _isTerminationCharacter(Char aChar)
        {
            bool result = false;
            if (aChar.Equals('.') ||
                aChar.Equals('?') ||
                aChar.Equals('!'))
            {
                result = true;
            }
            return result;
        }

        private async Task _speakMessage()
        {
            await _voiceBox.Speak(_message);
        }

        public void TaskComplete(Task task, string identifier)
        {
            switch (identifier)
            {
                case "abbreviations":
                    {
                        _populateAbbreviations();
                        _enabled = true;
                        OnPropertyChanged("Enabled");
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
