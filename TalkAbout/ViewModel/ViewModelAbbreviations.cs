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
    public class ViewModelAbbreviations : BindableBase, ITaskCompleteNotifier
    {
        private bool _showDeleteButton;
        private bool _selectionMode;
        private bool _showError;
        private string _abbreviationCode;
        private string _abbreviationPhrase;
        private string _error;
        private IList<Abbreviation> _selectedAbbreviations;
        private Abbreviations _abbreviations;
        private ObservableCollection<Abbreviation> _abbreviationCollection;

        public bool ShowDeleteButton
        {
            get
            {
                return _showDeleteButton;
            }
            set
            {
                SetProperty(ref _showDeleteButton, value);
            }
        }

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

        public string AbbreviationCode
        {
            get
            {
                return _abbreviationCode;
            }
            set
            {
                SetProperty(ref _abbreviationCode, value);
            }
        }

        public string AbbreviationPhrase
        {
            get
            {
                return _abbreviationPhrase;
            }
            set
            {
                SetProperty(ref _abbreviationPhrase, value);
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

        public IList<Abbreviation> SelectedAbbreviations
        {
            set
            {
                SetProperty(ref _selectedAbbreviations, value);
            }
        }

        public ObservableCollection<Abbreviation> AbbreviationCollection
        {
            get
            {
                _abbreviationCollection.Clear();
                var list = from Abbreviation abbr in _abbreviations.AbbreviationList
                           orderby abbr.Shortcut
                           select abbr;
                foreach (var item in list)
                {
                    _abbreviationCollection.Add(item);
                }

                return _abbreviationCollection;
            }
        }

        public Command AddAbbreviationCommand
        {
            get
            {
                return new Command(AddAbbreviation);
            }
        }

        public Command ToggleSelectionModeCommand
        {
            get
            {
                return new Command(ToggleSelectionMode);
            }
        }

        public RelayCommand<IList<Object>> DeleteAbbreviationsCommand
        {
            get
            {
                return new RelayCommand<IList<Object>>(DeleteAbbreviations);
            }

            
        }

        public Settings Settings
        {
            get
            {
                return Settings.Instance;
            }
        }


        public ViewModelAbbreviations()
        {
            _abbreviations = Abbreviations.Instance;
            _abbreviationCollection = new ObservableCollection<Abbreviation>();
            AbbreviationCode = "";
            AbbreviationPhrase = "";
            SelectionMode = false;
            _loadAbbreviations();
        }
            
        public void TaskComplete(Task task, string identifier)
        {
            switch (identifier)
            {
                case "abbreviations":
                    {
                        OnPropertyChanged("AbbreviationCollection");
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

        public void AddAbbreviation()
        {
            ResourceLoader loader = new ResourceLoader();
            if (!string.IsNullOrWhiteSpace(_abbreviationCode) && !string.IsNullOrWhiteSpace(_abbreviationPhrase))
            {
                
                int result = _abbreviations.AddAbbreviation(_abbreviationCode.Trim(), _abbreviationPhrase.Trim());
                switch (result)
                {
                    case -1:
                        {
                            _reportError(loader.GetString("ErrorAbbreviationExists"));
                            break;
                        }
                    case -2:
                        {
                            _reportError(loader.GetString("ErrorAbbreviationContainsSpaces"));
                            break;
                        }
                    case 0:
                        {
                            AbbreviationCode = "";
                            AbbreviationPhrase = "";
                            OnPropertyChanged("AbbreviationCollection");
                            break;
                        }
                    default:
                        break;
                }
                
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_abbreviationCode))
                {
                    _reportError(loader.GetString("ErrorAbbreviationShortcutMissing"));
                }
                else if (string.IsNullOrWhiteSpace(_abbreviationPhrase))
                {
                    _reportError(loader.GetString("ErrorAbbreviationExpansionMissing"));
                }
            }
        }

        public void ToggleSelectionMode()
        {
            SelectionMode = !SelectionMode;
        }

        public void DeleteAbbreviations(IList<Object> selectedAbbreviations)
        {
            if (SelectionMode)
            {
                if (selectedAbbreviations.Count() > 0)
                {
                    List<Abbreviation> list = new List<Abbreviation>();
                    foreach (var item in selectedAbbreviations)
                    {
                        list.Add((Abbreviation)item);
                    }
                    _abbreviations.DeleteAbbreviations(list);
                    OnPropertyChanged("AbbreviationCollection");
                }
            }
        }

        private void _loadAbbreviations()
        {
            TaskNotifier notifier = new TaskNotifier(_abbreviations.LoadAbbreviationsFromFile(), this, "abbreviations");
        }

        private void _reportError(string error)
        {
            Error = error;
            ShowError = true;
            new TaskNotifier(Task.Delay(5000), this, "error");
        }
    }
}
