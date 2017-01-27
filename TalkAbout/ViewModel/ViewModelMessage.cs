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
        private int _selectionStart;
        private int _oldSelectionStart;
        private int _selectionLength;
        private bool _enabled;
        private TaskNotifier _notifier;
        private Abbreviations _abbreviations;
        private Dictionary<String, String> _abbreviationDictionary;

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
        

        public ViewModelMessage()
        {
            _notifier = new TaskNotifier(this);
            _selectionStart = 0;
            _selectionLength = 0;
            _message = "";
            _oldMessage = "";
            _oldSelectionStart = 0;
            _abbreviations = Abbreviations.Instance;
            _abbreviationDictionary = new Dictionary<string, string>();
            _loadAbbreviations();
        }

        public void Update()
        {
            _update();
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
                _workingMessage.Insert(_selectionStart - 1, " ");
                _selectionStart = _selectionStart + 1;
            }
        }

        private void _update()
        {
            //we only update if there is no selection in the message window
            if (_selectionLength == 0)
            {
               if ((_selectionStart == _oldSelectionStart + 1) && (_workingMessage.Length == _oldMessage.Length + 1))
                {
                    //the message has been incremented by one
                    if (_workingMessage[_selectionStart - 1].Equals(' '))
                    {
                        _findLastWord();
                        if (_abbreviationExists(_lastWord.Trim()))
                        {
                            Debug.WriteLine("ViewModelMessage.cs: Abbreviation found");
                            string expansion = _abbreviationDictionary[_lastWord.Trim()];
                            _workingMessage = _workingMessage.Replace(_lastWord, expansion);
                            _message = _workingMessage;
                            _selectionStart = _selectionStart + (expansion.Length - _lastWord.Length);
                            Debug.WriteLine("ViewModelMessage.cs: New message is " + _workingMessage);
                            _ensureLastSpace();
                            OnPropertyChanged("Message");
                            OnPropertyChanged("CaretIndex");
                        }
                    }
                }
            }
            
        }


        /// <summary>
        /// Method finds the last word before the caret in the passed string
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private void _findLastWord()
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
                if (Char.IsLetterOrDigit(_workingMessage[index]))
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
                _lastWord = _workingMessage.Substring(wordStart, (_selectionStart - 1) - wordStart);
                Debug.WriteLine("ViewModel.cs: Last word is " + _lastWord);
            }
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
