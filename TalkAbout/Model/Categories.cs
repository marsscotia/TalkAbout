using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Data;
using Windows.Storage;

namespace TalkAbout.Model
{
    /// <summary>
    /// 
    /// Class is central point of access for saved phrases and
    /// categories.  Also delegates serialisation.
    /// 
    /// Class is singleton to ensure single point of access.
    /// 
    /// </summary>
    public class Categories
    {
        private static Categories _instance;
        private List<Category> _categoryList;
        private JsonConverter _converter;
        private const string _filename = "categories.txt";

        //error codes for CRUD methods
        private const int _success = 0; //success
        private const int _phraseExists = -1; //phrase already exists
        private const int _categoryNotFound = -2; //the specified category object couldn't be found

        //name for default category
        private const string _defaultCategory = "TalkAbout";


        public static Categories Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Categories();
                    Debug.WriteLine("Categories.cs: New Categories instance created.");
                }
                return _instance;
            }
        }

        public List<Category> CategoryList
        {
            get
            {
                return _categoryList;
            }
        }
            

        private Categories()
        {
            _converter = new JsonConverter();
            _categoryList = new List<Category>();

        }
        
        public async Task LoadCategoriesFromFile()
        {
            List<Category> list = await _converter.GetCategories(_filename);
            _categoryList.Clear();
            foreach (Category category in list)
            {
                _categoryList.Add(category);
            }
            
        }

        public void Save(int successCode)
        {
            _save(successCode);
        }

        //CRUD operations
        //Clients call these methods to create or delete phrases or categories

        public int AddPhrase(string phrase, string category)
        {
            int result = 1;
            if (_categoryExists(category))
            {
                Category selected = _getNamedCategory(category);
                if (_phraseExistsInCategory(phrase, selected))
                {
                    result = _phraseExists;
                }
                else
                {
                    selected.Phrases.Add(new Phrase(phrase));
                    result = _success;
                }
            }
            else
            {
                Category newCategory = new Category(category);
                newCategory.Phrases.Add(new Phrase(phrase));
                _categoryList.Add(newCategory);
                result = _success;
            }
            _save(result);
            return result;
        }

        public int AddPhrase(string phraseName, Category selected)
        {
            int result = 1;
            if (_categoryExists(selected))
            {
                if (_phraseExistsInCategory(phraseName, selected))
                {
                    result = _phraseExists;
                }
                else
                {
                    selected.Phrases.Add(new Phrase(phraseName));
                    result = _success;
                }
            }
            else
            {
                result = _categoryNotFound;
            }
            _save(result);
            return result;
        }

        public int AddPhrase(string phraseName)
        {
            int result = 1;
            if (_phraseExistsAtAll(phraseName))
            {
                result = _phraseExists;
            }
            else
            {
                if (_categoryExists(_defaultCategory))
                {
                    Category defaultCategory = _getNamedCategory(_defaultCategory);
                    defaultCategory.Phrases.Add(new Phrase(phraseName));
                    result = _success;
                }
                else
                {
                    Category defaultCategory = new Category(_defaultCategory);
                    defaultCategory.Phrases.Add(new Phrase(phraseName));
                    _categoryList.Add(defaultCategory);
                    result = _success;
                }
            }
            _save(result);
            return result;
        }

        public void DeletePhrase(Phrase selected)
        {
            List<Phrase> list = new List<Phrase>();
            list.Add(selected);
            DeletePhrases(list);
        }

        public void DeleteCategory(Category selected)
        {
            List<Category> list = new List<Category>();
            list.Add(selected);
            DeleteCategories(list);
        }


        /// <summary>
        /// 
        /// Method deletes a list of phrases.  
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void DeletePhrases(List<Phrase> selected)
        {
            for (int i = 0; i < selected.Count(); i++)
            {
                Category category = _findCategoryForPhrase(selected[i]);
                if (category != null)
                {
                    category.Phrases.Remove(selected[i]);
                    if (category.Phrases.Count() == 0)
                    {
                        _categoryList.Remove(category);
                    }
                }
            }
            _save(_success);
        }

        public void DeleteCategories(List<Category> selected)
        {
            for (int i = 0; i < selected.Count(); i++)
            {
                if (_categoryList.Contains(selected[i]))
                {
                    _categoryList.Remove(selected[i]);
                }
            }
            _save(_success);
        }

        /// <summary>
        /// Method updates the recent and frequency fields on 
        /// a phrase when it has been selected.
        /// </summary>
        /// <param name="selected"></param>
        public void PhraseSelected(Phrase selected)
        {
            selected.Frequency = selected.Frequency + 1;
            selected.Recent = DateTime.Now;
            _save(_success);
        }

        //Helper methods
        //Mostly convenience methods for CRUD operations

        /// <summary>
        /// 
        /// Method checks whether a named category exists.
        /// Case insensitive.
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns>true if category exists, false otherwise</returns>
        private bool _categoryExists(string categoryName)
        {
            bool result = false;
            foreach(Category category in _categoryList)
            {
                if (category.Name.ToLower().Equals(categoryName.ToLower()))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// Method checks whether a specified category object
        /// exists.
        /// 
        /// </summary>
        /// <param name="selected"></param>
        /// <returns>true if specified category object exists, false otherwise</returns>
        private bool _categoryExists(Category selected)
        {
            bool result = false;
            foreach (Category category in _categoryList)
            {
                if (category == selected)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// Method retrieves the category object with
        /// the specified name.
        /// Case insensitive.
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The category with the specified name, null if the category is not found.</returns>
        private Category _getNamedCategory(string name)
        {
            Category result = null;
            foreach (Category category in _categoryList)
            {
                if (category.Name.ToLower().Equals(name.ToLower()))
                {
                    result = category;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// Method checks whether a phrase already exists in 
        /// a specified category.
        /// Case insensitive. Punctuation sensitive.
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if phrase already exists, false otherwise</returns>
        private bool _phraseExistsInCategory(string name, Category category)
        {
            bool result = false;
            foreach (Phrase phrase in category.Phrases)
            {
                if (phrase.Name.ToLower().Equals(name.ToLower()))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// Method checks whether a phrase already exists in 
        /// a category with a specified name.
        /// Case insensitive.  Punctuation sensitive.
        /// 
        /// </summary>
        /// <param name="phraseName"></param>
        /// <param name="categoryName"></param>
        /// <returns>true if phrase exists in category with specified name, false otherwise</returns>
        private bool _phraseExistsInCategory(string phraseName, string categoryName)
        {
            bool result = false;
            if (_categoryExists(categoryName))
            {
                result = _phraseExistsInCategory(phraseName, _getNamedCategory(categoryName));
            }
            return result;
        }

        /// <summary>
        /// 
        /// Method checks whether a specified phrase exists at all, 
        /// in any category.
        /// Case insensitive. Punctuation sensitive.
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if specified phrase exists in any category, false otherwise</returns>
        private bool _phraseExistsAtAll(string name)
        {
            bool result = false;
            foreach (Category category in _categoryList)
            {
                foreach (Phrase phrase in category.Phrases)
                {
                    if (phrase.Name.ToLower().Equals(name.ToLower()))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// Method finds the category which contains
        /// the specified phrase object.
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="selected"></param>
        /// <returns>the category object which contains the specified phrase object, or null if the phrase is not found</returns>
        private Category _findCategoryForPhrase(Phrase selected)
        {
            Category result = null;
            foreach (Category category in _categoryList)
            {
                if (category.Phrases.Contains(selected))
                {
                    result = category;
                }
            }
            return result;
        }

        private void _save(int successCode)
        {
            if (successCode == _success)
            {
                _converter.SaveCategories(_filename); 
            }
        }
    }
}
