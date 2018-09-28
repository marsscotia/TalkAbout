using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;

namespace TalkAbout.ViewModel
{
    public class ViewModelCategory: BindableBase
    {
        private Category _category;
        private Settings _settings;
        private List<ViewModelPhrase> _phrases;

        public string Name
        {
            get
            {
                return _category.Name;
            }
        }

        public Category Category
        {
            get
            {
                return _category;
            }
        }

        public List<ViewModelPhrase> Phrases
        {
            get
            {
                return _phrases;
            }
        }

        public int FontSize
        {
            get
            {
                return _settings.FontSize;
            }
        }

        public int HeaderFontSize
        {
            get
            {
                return _settings.FontSize + 2;
            }
        }

        public ViewModelCategory(Category aCategory)
        {
            _category = aCategory;
            _settings = Settings.Instance;
            _phrases = new List<ViewModelPhrase>();
            foreach (var phrase in _category.Phrases)
            {
                _phrases.Add(new ViewModelPhrase(phrase));
            }
        }

    }
}
