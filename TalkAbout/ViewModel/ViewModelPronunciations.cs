using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;
using Windows.ApplicationModel.Resources;

namespace TalkAbout.ViewModel
{
    public class ViewModelPronunciations : BindableBase, ITaskCompleteNotifier
    {
        private bool _showDeleteButton;
        private bool _selectionMode;
        private bool _showError;
        private string _pronunciationWord;
        private string _pronunciationSound;
        private string _error;
        private TaskNotifier _notifier;
        private IList<Pronunciation> _selectedPronunciations;
        private Pronunciations _pronunciations;
        private ObservableCollection<Pronunciation> _pronunciationCollection;

        public bool SelectionMode
        {
            get
            {
                return _selectionMode;
            }
            set
            {
                SetProperty(ref _selectionMode, value);
            }
        }

        public string PronunciationWord
        {
            get
            {
                return _pronunciationWord;
            }
            set
            {
                SetProperty(ref _pronunciationWord, value);
            }
        }

        public string PronunciationSound
        {
            get
            {
                return _pronunciationSound;
            }
            set
            {
                SetProperty(ref _pronunciationSound, value);
            }
        }

        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                SetProperty(ref _error, value);
            }
        }

        public bool ShowError
        {
            get
            {
                return _showError;
            }
            set
            {
                SetProperty(ref _showError, value);
            }
        }

        public IList<Pronunciation> SelectedPronunciations
        {
            set
            {
                SetProperty(ref _selectedPronunciations, value);
            }
        }

        public ObservableCollection<Pronunciation> PronunciationCollection
        {
            get
            {
                _pronunciationCollection.Clear();
                var list = from Pronunciation pronunciation in _pronunciations.PronunciationList
                           orderby pronunciation.Word
                           select pronunciation;
                foreach (var item in list)
                {
                    _pronunciationCollection.Add(item);
                }

                return _pronunciationCollection;
            }
        }

        public Command AddPronunciationCommand
        {
            get
            {
                return new Command(AddPronunciation);
            }
        }

        public Command ToggleSelectionModeCommand
        {
            get
            {
                return new Command(ToggleSelectionMode);
            }
        }

        public RelayCommand<IList<Object>> DeletePronunciationsCommand
        {
            get
            {
                return new RelayCommand<IList<object>>(DeletePronunciations);
            }
        }

        public RelayCommand<string> SpeakCommand
        {
            get
            {
                return new RelayCommand<string>(Speak);
            }
        }

        public Settings Settings
        {
            get
            {
                return Settings.Instance;
            }
        }

        public ViewModelPronunciations()
        {
            _pronunciations = Pronunciations.Instance;
            _pronunciationCollection = new ObservableCollection<Pronunciation>();
            PronunciationWord = "";
            PronunciationSound = "";
            SelectionMode = false;
            _notifier = new TaskNotifier(this);
            _loadPronunciations();
        }

        
        public void TaskComplete(Task task, string identifier)
        {
            switch (identifier)
            {
                case "pronunciations":
                    {
                        OnPropertyChanged("PronunciationCollection");
                        break;
                    }
                case "error":
                    {
                        ShowError = false;
                        Error = "";
                        break;
                    }
                default:
                    {
                        break;
                    }
                    
            }
        }

        public void AddPronunciation()
        {
            ResourceLoader loader = new ResourceLoader();
            if (!string.IsNullOrWhiteSpace(_pronunciationWord) && !string.IsNullOrWhiteSpace(_pronunciationSound))
            {
                int result = _pronunciations.AddPronunciation(_pronunciationWord.Trim(), _pronunciationSound.Trim());
                switch (result)
                {
                    case -1:
                        {
                            _reportError(loader.GetString("ErrorPronunciationExists"));
                            break;
                        }
                    case -2:
                        {
                            _reportError(loader.GetString("ErrorPronunciationContainsSpaces"));
                            break;
                        }
                    case 0:
                        {
                            PronunciationWord = "";
                            PronunciationSound = "";
                            OnPropertyChanged("PronunciationCollection");
                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_pronunciationWord))
                {
                    _reportError(loader.GetString("ErrorPronunciationWordMissing"));
                }
                else if (string.IsNullOrWhiteSpace(_pronunciationSound))
                {
                    _reportError(loader.GetString("ErrorPronunciationSoundMissing"));
                }
            }
        }

        public void ToggleSelectionMode()
        {
            SelectionMode = !SelectionMode;
        }

        public void DeletePronunciations(IList<Object> selectedPronunciations)
        {
            if (SelectionMode)
            {
                if (selectedPronunciations.Count() > 0)
                {
                    List<Pronunciation> list = new List<Pronunciation>();
                    foreach (var item in selectedPronunciations)
                    {
                        list.Add((Pronunciation)item);
                    }
                    _pronunciations.DeletePronunciations(list);
                    OnPropertyChanged("PronunciationCollection");
                }
            }
        }

        public void Speak(string utterance)
        {
            throw new NotImplementedException();
        }

        private void _loadPronunciations()
        {
            _notifier.Execute(_pronunciations.LoadPronunciationsFromFile(), "pronunciations");
        }

        private void _reportError(string error)
        {
            Error = error;
            ShowError = true;
            _notifier.Execute(Task.Delay(5000), "error");
        }

        
    }
}
