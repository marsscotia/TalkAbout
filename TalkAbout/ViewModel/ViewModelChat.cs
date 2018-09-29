using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;
using Windows.ApplicationModel.Resources;

namespace TalkAbout.ViewModel
{
    public class ViewModelChat: BindableBase, ITaskCompleteNotifier
    {
        #region Fields

        private bool _showNewCategoryPanel;
        private bool _showDeleteButton;
        private bool _showPhrasesList;
        private bool _showSortedPhrasesList;
        private bool _showCategoryList;
        private bool _showError;
        private bool _selectionMode;
        private bool _selectionModeChanged;
        private bool _phraseSelected;
        private string _newCategory;
        private string _error;
        private string _listHeaderText;
        private TaskNotifier _notifier;
        private ViewModelCategories _categories;
        private ObservableCollection<IGrouping<ViewModelCategory,ViewModelPhrase>> _phrases;
        private ObservableCollection<ViewModelCategory> _categoryList;
        //private ObservableCollection<Phrase> _sortedPhrases;
        private ObservableCollection<ViewModelPhrase> _sortedPhrases;
        private IOrderedEnumerable<ViewModelPhrase> _sortedPhrasesBacking;
        private IEnumerable<IGrouping<ViewModelCategory, ViewModelPhrase>> _phrasesBacking;
        private ViewModelCategory _selectedCategory;
        private IList<ViewModelPhrase> _selectedPhrases;
        private ViewModelPhrase _selectedPhrase;
        private ViewModelPhrase _selectedSortedPhrase;
        private Sorts _sort;
        private ViewModelMessage _viewModelMessage;
        
        private const int _chatMode = 1;
        private const int _saveMode = 2;

        private const int _alphabetSort = 1;
        private const int _frequencySort = 2;
        private const int _recentSort = 3;
        private const int _categorySort = 4;

        private const string _phraseAlreadyExists = "That phrase already exists.";
        private const string _categoryNotFound = "The selected category couldn't be found.  Try selecting another.";
        private const string _messageEmpty = "The message is empty! Try typing something then saving it.";
        private const string _categoryEmpty = "The new category needs a name!  Try typing a name and then saving the phrase.";

        private enum Sorts { Recent, Frequency, Alphabet, Category };
        private enum Lists { Phrases, Categories, SortedPhrases };

        #endregion Fields

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
                OnPropertyChanged("ShowPhrasesDeleteButton");
            }
        }

        /// <summary>
        /// Property determines whether the category list is shown.
        /// This list is used to offer the user the choice of category
        /// to save a phrase into.
        /// </summary>
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

        /// <summary>
        /// This property determines whether the category sorting button
        /// appears.  It should only appear when categories are being used
        /// and when the sorting panel has been switched on
        /// </summary>
        public bool ShowCategorySortButton
        {
            get
            {
                return Settings.UseCategories && Settings.ShowSorting;
            }
        }

        /// <summary>
        /// This property determines whether the sorted phrase list is 
        /// shown.  This is visible when any of the sorts except for category
        /// sort have been chosen
        /// </summary>
        public bool ShowSortedPhrasesList
        {
            get
            {
                return _showSortedPhrasesList;
            }
            set
            {
                SetProperty(ref _showSortedPhrasesList, value);
                OnPropertyChanged("ShowSortedPhrasesDeleteButton");
            }
        }


        /// <summary>
        /// This property represents the name of a new category,
        /// which the user types before the new category is created
        ///
        /// </summary>
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

        /// <summary>
        /// This property represents the header of the list, which
        /// changes according to what is displayed in the list
        /// </summary>
        public string ListHeaderText
        {
            get
            {
                return _listHeaderText;
            }
            set
            {
                SetProperty(ref _listHeaderText, value);
            }
        }

        /// <summary>
        /// This property represents the list of phrases grouped by
        /// category.
        /// </summary>
        public ObservableCollection<IGrouping<ViewModelCategory, ViewModelPhrase>> Phrases
        {
            get
            {
                _populatePhrases();
                Debug.WriteLine("ViewModelChat.cs: Answering call to Phrases property, collection has " + _phrases.Count() + " items.");
                return _phrases;

            }
        }

        /// <summary>
        /// This property represents the list of categories.
        /// </summary>
        public ObservableCollection<ViewModelCategory> CategoryList
        {
            get
            {
                _categoryList.Clear();
                var categories = from ViewModelCategory c in _categories.ViewModelCategoryList
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
        /// This property represents the ungrouped list of phrases,
        /// sorted by alphabet, frequency or recent.
        /// </summary>
        public ObservableCollection<ViewModelPhrase> SortedPhrases
        {
            get
            {
                _populateSortedPhrases();
                _sortedPhrases.Clear();
                for (int i = 0; i < _sortedPhrasesBacking.Count(); i++)
                {
                    _sortedPhrases.Add(_sortedPhrasesBacking.ElementAt(i));
                }
                return _sortedPhrases;
            }
        }


        /// <summary>
        /// Property defines selected category in category list.
        /// Selected category is the one a new phrase will be
        /// added to. 
        ///
        /// </summary>
        public ViewModelCategory SelectedCategory
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

        public ViewModelPhrase SelectedPhrase
        {
            get
            {
                return _selectedPhrase;
            }
            set
            {
                SetProperty(ref _selectedPhrase, value);
            }
        }

        public ViewModelPhrase SelectedSortedPhrase
        {
            get
            {
                return _selectedSortedPhrase;
            }
            set
            {
                SetProperty(ref _selectedSortedPhrase, value);
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

        /// <summary>
        /// This property represents the content of the error message
        /// </summary>
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
                _selectionModeChanged = true;
                SetProperty(ref _selectionMode, value);
                OnPropertyChanged("ShowPhrasesDeleteButton");
                OnPropertyChanged("ShowSortedPhrasesDeleteButton");
            }
        }

        public IList<ViewModelPhrase> SelectedPhrases
        {
            set
            {
                SetProperty(ref _selectedPhrases, value);
            }
        }

        /// <summary>
        /// Property represents whether Selection Mode is able to be activated
        /// </summary>
        public bool SelectionModeEnabled
        {
            get
            {
                return !ShowCategoryList;
            }
        }

        public int AlphabetSort
        {
            get
            {
                return _alphabetSort;
            }
        }

        public int FrequencySort
        {
            get
            {
                return _frequencySort;
            }
        }

        public int RecentSort
        {
            get
            {
                return _recentSort;
            }
        }

        public int CategorySort
        {
            get
            {
                return _categorySort;
            }
        }

        public bool ShowPhrasesDeleteButton
        {
            get
            {
                return SelectionMode && ShowPhrasesList;
            }
        }

        public bool ShowSortedPhrasesDeleteButton
        {
            get
            {
                return SelectionMode && ShowSortedPhrasesList;
            }
        }


        public ViewModelMessage VMMessage
        {
            get
            {
                return _viewModelMessage;
            }
           
        }

        public RelayCommand<IList<object>> DeletePhrasesCommand
        {
            get
            {
                return new RelayCommand<IList<object>>(DeletePhrases);
            }
        }

        public RelayCommand<int> SetSortCommand
        {
            get
            {
                return new RelayCommand<int>(SetSort);
            }
        }
        
        public Command CancelSaveCommand
        {
            get
            {
                return new Command(CancelSave);
            }
        }

        public Command SaveCommand
        {
            get
            {
                return new Command(SavePhraseMode);
            }
        }

        public Command ToggleSelectionModeCommand
        {
            get
            {
                return new Command(ToggleSelectionMode);
            }
        }

        public Command DeleteWordCommand
        {
            get
            {
                return new Command(DeleteWord);
            }
        }

        public Command ClearCommand
        {
            get
            {
                return new Command(Clear);
            }
        }

        public Command PhraseSelectedCommand
        {
            get
            {
                return new Command(PhraseSelected);
            }
        }

        public Command SortedPhraseSelectedCommand
        {
            get
            {
                return new Command(SortedPhraseSelected);
            }
        }

        public RelayCommand<string> SelectPhraseByPositionCommand
        {
            get
            {
                return new RelayCommand<string>(_selectPhraseByPosition);
            }
        }

        public Command SpeakCommand
        {
            get
            {
                return new Command(_speak);
            }
        }
        
        public Command AddNewPhraseAndNewCategoryCommand
        {
            get
            {
                return new Command(AddNewPhraseAndNewCategory);
            }
        }



        #endregion Properties


        #region Constructor
        public ViewModelChat()
        {
            _selectionModeChanged = false;
            _phraseSelected = false;
            _categories = new ViewModelCategories();
            _phrases = new ObservableCollection<IGrouping<ViewModelCategory, ViewModelPhrase>>();
            _sort = Sorts.Category;
            _sortedPhrases = new ObservableCollection<ViewModelPhrase>();
            _categoryList = new ObservableCollection<ViewModelCategory>();
            _viewModelMessage = new ViewModelMessage();
            _notifier = new TaskNotifier(this);
            ChatMode();
        }

        #endregion Constructor 

        /// <summary>
        /// Switches the app to save phrase mode.
        /// </summary>
        public void SavePhraseMode()
        {

            //if we're using categories, let the user select one or add a new one
            if (Settings.UseCategories)
            {
                ShowNewCategoryPanel = true;
                _showList(Lists.Categories);
            }
            //if we're not using categories, add the phrase to the default category
            else
            {
                if (!string.IsNullOrWhiteSpace(_viewModelMessage.Message))
                {
                    int result = _categories.AddPhrase(_viewModelMessage.Message);
                    switch (result)
                    {
                        case -1:
                            {
                                _reportError(_phraseAlreadyExists);
                                break;
                            }
                        default:
                            break;
                    }
                    OnPropertyChanged("SortedPhrases");
                }
                else
                {
                    _reportError(_messageEmpty);
                }
            }
            
        }

        /// <summary>
        /// Switches the app to chat mode.  This is the 
        /// default mode, and the one the app starts in.
        /// </summary>
        public void ChatMode()
        {
            ShowNewCategoryPanel = false;
            if (Settings.UseCategories)
            {
                if (_sort == Sorts.Category)
                {
                    _showList(Lists.Phrases);
                }
                else
                {
                    _showList(Lists.SortedPhrases);
                }
            }
            else
            {
                _showList(Lists.SortedPhrases);
            }
            SelectionMode = false;
            NewCategory = "";
        }

        /// <summary>
        /// This method is called when the text in the message window
        /// is changed.  It's a forwarding call to the ViewModelMessage object.
        /// </summary>
        public void UpdateMessage()
        {
            Debug.WriteLine("ViewModelChat.cs: UpdateMessage() called.");
            _viewModelMessage.Update();
            if (Settings.FilterPhrases)
            {
                OnPropertyChanged("SortedPhrases");
                OnPropertyChanged("Phrases");
            }
            
        }

        public void DeleteWord()
        {
            _viewModelMessage.DeleteWord();
        }

        public void Clear()
        {
            _viewModelMessage.Clear();
        }
        
        public void AddNewPhraseAndNewCategory()
        {
            if (!string.IsNullOrWhiteSpace(_viewModelMessage.Message) && !string.IsNullOrWhiteSpace(_newCategory))
            {
                int result = _categories.AddPhrase(_viewModelMessage.Message, _newCategory);
                //TODO: If there was an error, inform the user
                _viewModelMessage.Clear();
                NewCategory = "";
                OnPropertyChanged("Phrases");
                OnPropertyChanged("CategoryList");
                OnPropertyChanged("SortedPhrases");
                ChatMode();
            }
        }

        public void AddNewPhraseToSelectedCategory()
        {
            if (!string.IsNullOrWhiteSpace(_viewModelMessage.Message) && _selectedCategory != null)
            {
                int result = _categories.AddPhrase(_viewModelMessage.Message, _selectedCategory);
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
                _viewModelMessage.Clear();
                SelectedCategory = null;
                OnPropertyChanged("Phrases");
                OnPropertyChanged("SortedPhrases");
                ChatMode();
            }
            else
            {
                //if the selected category is not null, the user has selected one
                if (_selectedCategory != null)
                {
                    //if the message is empty, we have to inform the user
                    if (string.IsNullOrWhiteSpace(_viewModelMessage.Message))
                    {
                        _reportError(_messageEmpty);
                        SelectedCategory = null;
                        ChatMode();
                        Debug.WriteLine("ViewModelChat.cs: Error to report message empty when adding new phrase to selected category.");
                    }
                }
               

            }
        }

        public void PhraseSelected()
        {
            if (!SelectionMode)
            {
                
                if (_selectedPhrase != null)
                {
                    _viewModelMessage.PhraseSelected(_selectedPhrase);
                    _categories.PhraseSelected(_selectedPhrase);
                    _phraseSelected = true;
                } 
                
            }
        }

        public void PhraseSelected(ViewModelPhrase selectedPhrase)
        {
            if (!SelectionMode)
            {
                if (selectedPhrase != null)
                {
                    _viewModelMessage.PhraseSelected(selectedPhrase);
                    _categories.PhraseSelected(selectedPhrase);
                    _phraseSelected = true;
                }
            }
        }
        
        public void SortedPhraseSelected()
        {
            if (!SelectionMode)
            {
                
                if (_selectedSortedPhrase != null)
                {
                    _viewModelMessage.PhraseSelected(_selectedSortedPhrase);
                    _categories.PhraseSelected(_selectedSortedPhrase);
                    _phraseSelected = true;
                } 
            }
        }

        public void TaskComplete(Task task, string identifier)
        {
            switch (identifier)
            {
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
        }

        public void DeletePhrases(IList<object> selectedPhrases)
        {
            if (SelectionMode)
            {
                if (selectedPhrases.Count() > 0)
                {
                    List<ViewModelPhrase> selectedList = new List<ViewModelPhrase>();
                    foreach (var item in selectedPhrases)
                    {
                        
                        selectedList.Add((ViewModelPhrase)item);
                        
                    }
                    _categories.DeletePhrases(selectedList);
                    OnPropertyChanged("Phrases");
                    OnPropertyChanged("SortedPhrases");
                }
            }
        }

        public void SetSort(int aSort)
        {
            switch (aSort)
            {
                case _alphabetSort:
                    {
                        _setSort(Sorts.Alphabet);
                        break;
                    }
                case _frequencySort:
                    {
                        _setSort(Sorts.Frequency);
                        break;
                    }
                case _recentSort:
                    {
                        _setSort(Sorts.Recent);
                        break;
                    }
                case _categorySort:
                    {
                        _setSort(Sorts.Category);
                        break;
                    }
                default:
                    {
                        _setSort(Sorts.Category);
                        break;
                    }
                    
            }
            
        }

        public void CancelSave()
        {
            if (ShowNewCategoryPanel)
            {
                ChatMode();
            }
        }

        private async void _speak()
        {
            await Speak();
        }

        public async Task Speak()
        {
            await _viewModelMessage.Speak();
        }

        private void _setSort(Sorts aSort)
        {
            if (_sort != aSort)
            {
                _sort = aSort;
                switch (_sort)
                {
                    case Sorts.Recent:
                        {
                            _showList(Lists.SortedPhrases);

                            break;
                        }
                    case Sorts.Frequency:
                        {
                            _showList(Lists.SortedPhrases);

                            break;
                        }
                    case Sorts.Alphabet:
                        {
                            _showList(Lists.SortedPhrases);

                            break;
                        }
                    case Sorts.Category:
                        {
                            if (Settings.UseCategories)
                            {
                                _showList(Lists.Phrases);
                                OnPropertyChanged("Phrases");
                            }
                            break;
                        }
                    default:
                        break;
                }
                OnPropertyChanged("SortedPhrases"); 
            }
        }

        private void _populateSortedPhrases()
        {

            bool lastWordExists = !string.IsNullOrWhiteSpace(_viewModelMessage.LastWord);
            bool lastSentenceExists = !string.IsNullOrWhiteSpace(_viewModelMessage.LastSentence);

            IEnumerable<ViewModelPhrase> unsorted;

            if (Settings.FilterPhrases 
                && (lastWordExists || lastSentenceExists) && !_phraseSelected)
            {
                string lastWord = _viewModelMessage.LastWord.Trim();
                string lastSentence = _viewModelMessage.LastSentence.Trim();

                if (lastSentence == lastWord || !lastSentenceExists)
                {
                    unsorted = from ViewModelCategory c in _categories.ViewModelCategoryList
                                            from ViewModelPhrase p in c.Phrases
                                            where p.Name.Contains(lastWord)
                                            select p;

                    _viewModelMessage.Filtered = true;
                }
                else
                {
                    unsorted = (from ViewModelCategory c in _categories.ViewModelCategoryList
                                             from ViewModelPhrase p in c.Phrases
                                             where p.Name.Contains(lastWord)
                                             select p).Union(
                                            from ViewModelCategory c in _categories.ViewModelCategoryList
                                            from ViewModelPhrase p in c.Phrases
                                            where p.Name.Contains(lastSentence)
                                            select p).Distinct();

                    _viewModelMessage.Filtered = true;
                                            
                }

                //if neither of the above filters return results, then don't apply a filter
                if (unsorted.Count() == 0)
                {
                    unsorted = from ViewModelCategory c in _categories.ViewModelCategoryList
                                            from ViewModelPhrase p in c.Phrases
                                            select p;

                    _viewModelMessage.Filtered = false;
                }

            }
            else
            {
                unsorted = from ViewModelCategory c in _categories.ViewModelCategoryList
                                        from ViewModelPhrase p in c.Phrases
                                        select p;

                _viewModelMessage.Filtered = false;
            }

            _phraseSelected = false;

            Debug.WriteLine("ViewModelChat.cs: _sort is " + _sort.ToString());

            switch (_sort)
            {
                case Sorts.Recent:
                    { 
                        _sortedPhrasesBacking = unsorted.OrderByDescending(phrase => phrase.Recent);
                        break;
                    }
                case Sorts.Frequency:
                    {
                        _sortedPhrasesBacking = unsorted.OrderByDescending(phrase => phrase.Frequency);
                        break;
                    }
                case Sorts.Alphabet:
                    {
                        _sortedPhrasesBacking = unsorted.OrderBy(phrase => phrase.Name, StringComparer.CurrentCultureIgnoreCase);
                        break;
                    }
                case Sorts.Category:
                    {
                        _sortedPhrasesBacking = unsorted.OrderBy(phrase => phrase.Name, StringComparer.CurrentCultureIgnoreCase);
                        break;
                    }
                default:
                    {
                        
                        break;
                    }
            }

            Debug.WriteLine("ViewModelChat.cs: _sortedPhrasesBacking has " + _sortedPhrasesBacking.Count() + " items.");

            if (ShowSortedPhrasesList)
            {
                for (int i = 0; i < _sortedPhrasesBacking.Count(); i++)
                {
                    _sortedPhrasesBacking.ElementAt(i).Position = i;
                } 
            }
            
        }

        private void _populatePhrases()
        {
            _phrases.Clear();

            bool lastWordExists = !string.IsNullOrWhiteSpace(_viewModelMessage.LastWord);
            bool lastSentenceExists = !string.IsNullOrWhiteSpace(_viewModelMessage.LastSentence);

            if (Settings.FilterPhrases
                && (lastWordExists || lastSentenceExists) && !_phraseSelected)
            {
                string lastWord = _viewModelMessage.LastWord.Trim();
                string lastSentence = _viewModelMessage.LastSentence.Trim();

                if (lastSentence == lastWord || !lastSentenceExists)
                {
                    _phrasesBacking = from ViewModelCategory c in _categories.ViewModelCategoryList
                                      from ViewModelPhrase p in c.Phrases
                                      where p.Name.Contains(lastWord)
                                      orderby c.Name, p.Name
                                      group p by c into newgroup
                                      select newgroup;

                    _viewModelMessage.Filtered = true;
                }
                else
                {
                    _phrasesBacking = from ViewModelCategory c in _categories.ViewModelCategoryList
                                      from ViewModelPhrase p in c.Phrases
                                      where p.Name.Contains(lastWord) || p.Name.Contains(lastSentence)
                                      orderby c.Name, p.Name
                                      group p by c into newgroup
                                      select newgroup;

                    _viewModelMessage.Filtered = true;
                }

                if (_phrasesBacking.Count() == 0)
                {
                    _phrasesBacking = from ViewModelCategory c in _categories.ViewModelCategoryList
                                      from ViewModelPhrase p in c.Phrases
                                      orderby c.Name, p.Name
                                      group p by c into newgroup
                                      select newgroup;

                    _viewModelMessage.Filtered = false;
                }


            }
            else
            {
                _phrasesBacking = from ViewModelCategory c in _categories.ViewModelCategoryList
                                  from ViewModelPhrase p in c.Phrases
                                  orderby c.Name, p.Name
                                  group p by c into newgroup
                                  select newgroup;

                _viewModelMessage.Filtered = false;
            }

            _phraseSelected = false;

            foreach (var item in _phrasesBacking)
            {
                _phrases.Add(item);

            }

            if (ShowPhrasesList)
            {
                int count = 0;
                int outerIndex = 0;

                while (outerIndex < _phrases.Count())
                {
                    int innerIndex = 0;
                    var innerPhrases = _phrases[outerIndex];
                    while (innerIndex < innerPhrases.Count())
                    {
                        innerPhrases.ElementAt(innerIndex).Position = count;
                        count++;
                        innerIndex++;
                    }
                    outerIndex++;
                }
            }
        }

        private void _reportError(string error)
        {
            Error = error;
            ShowError = true;
            _notifier.Execute(Task.Delay(5000), "error");
        }

        private void _selectPhraseByPosition(string position)
        {
            bool isNumber = int.TryParse(position, out int i);
            if (isNumber)
            {
                if (i >= 0 && i <= 9)
                {
                    if (ShowPhrasesList)
                    {
                        ViewModelPhrase selected = (from c in Phrases
                                                    from p in c
                                                    where p.Position == i
                                                    select p).FirstOrDefault();
                        if (selected != null)
                        {
                            PhraseSelected(selected);
                        }
                    }
                    else if (ShowSortedPhrasesList)
                    {
                        ViewModelPhrase selected = SortedPhrases.FirstOrDefault(p => p.Position == i);
                        if (selected != null)
                        {
                            PhraseSelected(selected);
                        }
                    }
                }
            }
        }

        private void _showList(Lists aList)
        {
            ResourceLoader loader = new ResourceLoader();
            switch (aList)
            {
                case Lists.Phrases:
                    {
                        ShowPhrasesList = true;
                        ShowCategoryList = false;
                        ShowSortedPhrasesList = false;
                        ListHeaderText = loader.GetString("PhrasesListHeaderText");
                        break;
                    }
                case Lists.Categories:
                    {
                        ShowPhrasesList = false;
                        ShowCategoryList = true;
                        ShowSortedPhrasesList = false;
                        ListHeaderText = loader.GetString("CategoriesListHeaderText");
                        break;
                    }
                case Lists.SortedPhrases:
                    {
                        ShowPhrasesList = false;
                        ShowCategoryList = false;
                        ShowSortedPhrasesList = true;
                        ListHeaderText = loader.GetString("PhrasesListHeaderText");
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
