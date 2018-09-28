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
            set
            {
                _phrase.Frequency = value;
                OnPropertyChanged("Frequency");
            }
            
        }

        public DateTime Recent
        {
            get
            {
                return _phrase.Recent;
            }
            set
            {
                _phrase.Recent = value;
                OnPropertyChanged("Recent");
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

        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                SetProperty(ref _position, value);
                OnPropertyChanged("Shortcut");
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
                else if (_position == 9)
                {
                    result = "alt + " + 0;
                }
                else
                {
                    result = "";
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
