using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkAbout.Model
{
    public class Pronunciation
    {
        private string _word;
        private string _sound;

        public string Word
        {
            get
            {
                return _word;
            }
            set
            {
                _word = value;
            }
        }

        public string Sound
        {
            get
            {
                return _sound;
            }
            set
            {
                _sound = value;
            }
        }

        public Pronunciation(string aWord, string aSound)
        {
            _word = aWord;
            _sound = aSound;
        }
    }
}
