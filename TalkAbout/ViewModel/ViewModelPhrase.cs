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
        private int _position;

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

        public string Shortcut
        {
            get
            {
                string result = "";
                if (_position < 9)
                {
                    result = "alt + " + (_position + 1);
                }
                return result;
            }
        }

        public ViewModelPhrase(Phrase aPhrase)
        {
            _phrase = aPhrase;
            _settings = Settings.Instance;
        }

        public ViewModelPhrase(Phrase aPhrase, int aPosition)
        {
            _phrase = aPhrase;
            _settings = Settings.Instance;
            _position = aPosition;
        }
    }
}
