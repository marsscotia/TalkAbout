using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;

namespace TalkAbout.ViewModel
{
    public class ViewModelPhrase: BindableBase
    {
        private Phrase _phrase;
        private Settings _settings;

        public string Name
        {
            get
            {
                return _phrase.Name;
            }
        }

        public int Frequency
        {
            get
            {
                return _phrase.Frequency;
            }
            
        }

        public DateTime Recent
        {
            get
            {
                return _phrase.Recent;
            }
        }

        public Phrase Phrase
        {
            get
            {
                return _phrase;
            }
        }

        public int FontSize
        {
            get
            {
                return _settings.FontSize;
            }
        }

        public ViewModelPhrase(Phrase aPhrase)
        {
            _phrase = aPhrase;
            _settings = Settings.Instance;
        }
    }
}
