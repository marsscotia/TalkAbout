using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;

namespace TalkAbout.ViewModel
{
    public class ViewModelChat: BindableBase, ITaskCompleteNotifier
    {
        private bool _showNewCategoryPanel;
        private bool _showDeleteButton;
        private bool _showPhrasesList;
        private bool _showSortedPhrasesList;
        private bool _showCategoryList;
        private bool _showError;
        private bool _selectionMode;
        private string _newCategory;
        private string _message;
        private string _error;
        private Categories _categories;
        private ObservableCollection<IGrouping<Category,Phrase>> _phrases;
        private ObservableCollection<Category> _categoryList;
        private Category _selectedCategory;
        private IList<Phrase> _selectedPhrases;
        private RelayCommand<IList<object>> _deletePhrasesCommand;

        private int _mode;
        
        private const int _chatMode = 1;
        private const int _saveMode = 2;

        private const string _phraseAlreadyExists = "That phrase already exists.";
        private const string _categoryNotFound = "The selected category couldn't be found.  Try selecting another.";
        private const string _messageEmpty = "The message is empty! Try typing something then saving it.";
        private const string _categoryEmpty = "The new category needs a name!  Try typing a name and then saving the phrase.";

        #region Properties
        /// <summary>
        /// 
        /// Property exposes application settings.
        /// 
        /// </summary>
        public Settings Settings
        {
            get
            {
                return Settings.Instance;
            }
        }

        /// <summary>
        /// Property defines whether the new category panel is shown.
        /// This panel is only shown in save mode, when Use Categories is on.
        /// </summary>
        public bool ShowNewCategoryPanel
        {
            get
            {
                return _showNewCategoryPanel;
            }
            set
            {
                SetProperty(ref _showNewCategoryPanel, value);
            }
        }

        /// <summary>
        /// Property defines whether the delete button is shown.
        /// This button is only shown in selection mode.
        /// </summary>
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

        /// <summary>
        /// Property determines whether the phrases list is shown.
        /// This list is phrases grouped by category with semantic zoom.
        /// It is not shown when Use Categories is off.
        /// </summary>
        public bool ShowPhrasesList
        {
            get
            {
                return _showPhrasesList;
            }
            set
            {
                SetProperty(ref _showPhrasesList, value);
            }
        }

        public bool ShowCategoryList
        {
            get
            {
                return _showCategoryList;
            }
            set
            {
                SetProperty(ref _showCategoryList, value);
                OnPropertyChanged("SelectionModeEnabled");
            }
        }


        public string NewCategory
        {
            get
            {
                return _newCategory;
            }
            set
            {
                SetProperty(ref _newCategory, value);
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                SetProperty(ref _message, value);
            }
        }

        public ObservableCollection<IGrouping<Category, Phrase>> Phrases
        {
            get
            {
                _phrases.Clear();
                var phraseslist = from Category c in _categories.CategoryList
                                  from Phrase p in c.Phrases
                                  orderby p.Name 
                                  group p by c into newgroup
                                  orderby newgroup.Key.Name
                                  select newgroup;
                foreach (var item in phraseslist)
                {
                    _phrases.Add(item);
                }
                Debug.WriteLine("ViewModelChat.cs: Answering call to Phrases property, collection has " + _phrases.Count() + " items.");
                return _phrases;

            }
        }

        public ObservableCollection<Category> CategoryList
        {
            get
            {
                _categoryList.Clear();
                var categories = from Category c in _categories.CategoryList
                                 orderby c.Name
                                 select c;
                foreach (var item in categories)
                {
                    _categoryList.Add(item);
                }
                return _categoryList;
            }
        }

        /// <summary>
        /// Property defines selected category in category list.
        /// Selected category is the one a new phrase will be
        /// added to. 
        ///
        /// </summary>
        public Category SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                SetProperty(ref _selectedCategory, value);
            }
        }

        /// <summary>
        /// Property determines whether the error panel is shown.
        /// </summary>
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

        /// <summary>
        /// Property defines font size for headers, as 2 more
        /// than font size set in Settings.
        /// </summary>
        public int HeaderFontSize
        {
            get
            {
                return Settings.FontSize + 2;
            }
        }

        /// <summary>
        /// Property determines whether selection mode is switched on;
        /// that is, whether multiple phrases can be selected to be deleted.
        /// </summary>
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

        public IList<Phrase> SelectedPhrases
        {
            set
            {
                SetProperty(ref _selectedPhrases, value);
            }
        }

        public bool SelectionModeEnabled
        {
            get
            {
                return !ShowCategoryList;
            }
        }

        public RelayCommand<IList<object>> DeletePhrasesCommand
        {
            get
            {
                return new RelayCommand<IList<object>>(DeletePhrases);
            }
        }
         

        #endregion Properties


        #region Constructor
        public ViewModelChat()
        {
            _categories = Categories.Instance;
            _phrases = new ObservableCollection<IGrouping<Category, Phrase>>();
            _categoryList = new ObservableCollection<Category>();
            ChatMode();
            _loadPhrases();
        }

        #endregion Constructor 

        /// <summary>
        /// Switches the app to save phrase mode.
        /// </summary>
        public void SavePhraseMode()
        {
            
                if (Settings.UseCategories)
                {
                    ShowNewCategoryPanel = true;
                    ShowCategoryList = true;
                    ShowPhrasesList = false;
                } 
            
        }

        /// <summary>
        /// Switches the app to chat mode.  This is the 
        /// default mode, and the one the app starts in.
        /// </summary>
        public void ChatMode()
        {
            ShowNewCategoryPanel = false;
            ShowCategoryList = false;
            ShowPhrasesList = true;
            SelectionMode = false;
            NewCategory = "";
        }

        private void _loadPhrases()
        {
            TaskNotifier notifier = new TaskNotifier(_categories.LoadCategoriesFromFile(), this, "categories");

            
        }


        public void AddNewPhraseAndNewCategory()
        {
            if (!string.IsNullOrWhiteSpace(_message) && !string.IsNullOrWhiteSpace(_newCategory))
            {
                int result = _categories.AddPhrase(_message, _newCategory);
                //TODO: If there was an error, inform the user
                Message = "";
                NewCategory = "";
                OnPropertyChanged("Phrases");
                OnPropertyChanged("CategoryList");
                ChatMode();
            }
        }

        public void AddNewPhraseToSelectedCategory()
        {
            if (!string.IsNullOrWhiteSpace(_message) && _selectedCategory != null)
            {
                int result = _categories.AddPhrase(_message, _selectedCategory);
                Debug.WriteLine("ViewModelChat.cs: Call to add phrase to selected category with result: " + result);
                switch (result)
                {
                    case -1:
                        {
                            _reportError(_phraseAlreadyExists);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                Message = "";
                SelectedCategory = null;
                OnPropertyChanged("Phrases");
                ChatMode();
            }
            else
            {
                //if the selected category is not null, the user has selected one
                if (_selectedCategory != null)
                {
                    //if the message is empty, we have to inform the user
                    if (string.IsNullOrWhiteSpace(_message))
                    {
                        _reportError(_messageEmpty);
                        SelectedCategory = null;
                        ChatMode();
                        Debug.WriteLine("ViewModelChat.cs: Error to report message empty when adding new phrase to selected category.");
                    }
                }
               

            }
        }
        

        public void TaskComplete(Task task, string identifier)
        {
            switch (identifier)
            {
                case "categories":
                    {
                        OnPropertyChanged("Phrases");
                        OnPropertyChanged("CategoryList");
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

        public void ToggleSelectionMode()
        {

            SelectionMode = !SelectionMode;
            Debug.WriteLine("ViewModelChat.cs: Selection Mode is " + SelectionMode.ToString());
        }

        public void DeletePhrases(IList<object> selectedPhrases)
        {
            if (SelectionMode)
            {
                if (selectedPhrases.Count() > 0)
                {
                    List<Phrase> selectedList = new List<Phrase>();
                    foreach (var item in selectedPhrases)
                    {
                        selectedList.Add((Phrase)item);
                    }
                    _categories.DeletePhrases(selectedList);
                    OnPropertyChanged("Phrases");
                }
            }
        }

        private void _reportError(string error)
        {
            Error = error;
            ShowError = true;
            new TaskNotifier(Task.Delay(5000), this, "error");
        }
    }
}
