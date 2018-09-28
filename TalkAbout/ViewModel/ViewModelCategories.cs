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
    /// <summary>
    /// Class represents access to ViewModels of
    /// Phrase and Category.  
    /// 
    /// Contains CRUD methods.
    /// 
    /// </summary>
    public class ViewModelCategories: BindableBase
    {
        private ObservableCollection<ViewModelCategory> _viewModelCategoryList;
        private Categories _categories;
        private string _defaultCategory;
        private ResourceLoader _loader;

        private const int _success = 0;
        private const int _phraseExists = -1;
        private const int _categoryNotFound = -2;

        public ObservableCollection<ViewModelCategory> ViewModelCategoryList
        {
            get
            {
                return _viewModelCategoryList;
            }
        }

        public ViewModelCategories()
        {
            _loader = new ResourceLoader();
            _defaultCategory = _loader.GetString("DefaultCategoryName");
            _viewModelCategoryList = new ObservableCollection<ViewModelCategory>();
            _categories = Categories.Instance;
            _loadPhrases();
        }

        /// <summary>
        /// 
        /// Method saves a new phrase.  If the category with the specified name
        /// exists, it saves the new phrase into that category; if not, it creates
        /// a new category and saves the phrase into that.
        /// 
        /// </summary>
        /// <param name="phrase">The text of the new phrase</param>
        /// <param name="category">The name of the category into which the new phrase will be saved</param>
        /// <returns>An integer value representing either success or an error status</returns>
        public int AddPhrase(string phrase, string category)
        {
            int result = 1;
            if (_categoryExists(category))
            {
                ViewModelCategory selected = _getNamedCategory(category);
                if (_phraseExistsInCategory(phrase, selected))
                {
                    result = _phraseExists;
                }
                else
                {
                    Phrase newPhrase = new Phrase(phrase);
                    selected.Category.Phrases.Add(newPhrase);
                    selected.Phrases.Add(new ViewModelPhrase(newPhrase));
                    result = _success;
                }
            }
            else
            {
                Category newCategory = new Category(category);
                newCategory.Phrases.Add(new Phrase(phrase));
                _categories.CategoryList.Add(newCategory);
                _viewModelCategoryList.Add(new ViewModelCategory(newCategory));
                result = _success;
            }
            _categories.Save(result);
            return result;
        }

        /// <summary>
        /// 
        /// Method saves a new phrase into the selected category.
        /// 
        /// </summary>
        /// <param name="phrase">The text of the new phrase to be saved</param>
        /// <param name="selected">The selected category into which the phrase is to be saved</param>
        /// <returns>An integer value representing success or an error state</returns>
        public int AddPhrase(string phrase, ViewModelCategory selected)
        {
            int result = 1;
            if (_categoryExists(selected))
            {
                if (_phraseExistsInCategory(phrase, selected))
                {
                    result = _phraseExists;
                }
                else
                {
                    Phrase newPhrase = new Phrase(phrase);
                    selected.Category.Phrases.Add(newPhrase);
                    selected.Phrases.Add(new ViewModelPhrase(newPhrase));
                    result = _success;
                }
            }
            else
            {
                result = _categoryNotFound;
            }
            _categories.Save(result);
            return result;
        }

        /// <summary>
        /// Method saves a phrase.  Most commonly used when categories are switched
        /// off, so saves into default category.  If default category does not exist, 
        /// it creates it first.
        /// </summary>
        /// <param name="phrase">The text of the phrase to be saved</param>
        /// <returns>An integer value representing success or an error state</returns>
        public int AddPhrase(string phrase)
        {
            int result = 1;

            if (_phraseExistsAtAll(phrase))
            {
                result = _phraseExists;
            }
            else
            {
                Phrase newPhrase = new Phrase(phrase);
                if (_categoryExists(_defaultCategory))
                {
                    ViewModelCategory defaultCategory = _getNamedCategory(_defaultCategory);
                    defaultCategory.Category.Phrases.Add(newPhrase);
                    defaultCategory.Phrases.Add(new ViewModelPhrase(newPhrase));
                    result = _success;
                }
                else
                {
                    Category defaultCategory = new Category(_defaultCategory);
                    defaultCategory.Phrases.Add(newPhrase);
                    _categories.CategoryList.Add(defaultCategory);
                    _viewModelCategoryList.Add(new ViewModelCategory(defaultCategory));
                    result = _success;
                }
            }
            _categories.Save(result);
            return result;
        }

        public void DeletePhrases(List<ViewModelPhrase> selected)
        {
            foreach (var phrase in selected)
            {
                ViewModelCategory category = _findCategoryForPhrase(phrase);
                if (category != null)
                {
                    category.Phrases.Remove(phrase);
                    category.Category.Phrases.Remove(phrase.Phrase);
                    if (category.Phrases.Count == 0)
                    {
                        _viewModelCategoryList.Remove(category);
                        _categories.CategoryList.Remove(category.Category);
                    }
                }

            }
            _categories.Save(_success);
        }

        public void DeleteCategories(List<ViewModelCategory> selected)
        {
            foreach (var category in selected)
            {
                if (_viewModelCategoryList.Contains(category))
                {
                    _viewModelCategoryList.Remove(category);
                    _categories.CategoryList.Remove(category.Category);
                }
            }
            _categories.Save(_success);
        }

        public void PhraseSelected(ViewModelPhrase selected)
        {
            selected.Frequency = selected.Frequency + 1;
            selected.Recent = DateTime.Now;
            _categories.Save(_success);
        }

        private void _loadPhrases()
        {
            foreach (var category in _categories.CategoryList)
            {
                _viewModelCategoryList.Add(new ViewModelCategory(category));
            }
        }

        private bool _categoryExists(ViewModelCategory aCategory)
        {
            return _viewModelCategoryList.Any(c => c == aCategory);
        }

        private bool _categoryExists(string categoryName)
        {
            return _viewModelCategoryList.Any(c => c.Name == categoryName);
        }

        private ViewModelCategory _getNamedCategory(string categoryName)
        {
            return _viewModelCategoryList.First(c => c.Name == categoryName);
        }

        private bool _phraseExistsInCategory(string phrase, ViewModelCategory category)
        {
            return category.Phrases.Any(p => p.Name == phrase);
        }

        private bool _phraseExistsAtAll(string phrase)
        {
            return _viewModelCategoryList.SelectMany(c => c.Phrases).Any(p => p.Name == phrase);
        }

        private ViewModelCategory _findCategoryForPhrase(ViewModelPhrase phrase)
        {
            return _viewModelCategoryList.First(c => c.Phrases.Any(p => p == phrase));
        }
    }
}
