using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkAbout.Model
{
    public class Abbreviation
    {
        private string _shortcut;
        private string _expansion;

        public string Shortcut
        {
            get
            {
                return _shortcut;
            }
            set
            {
                _shortcut = value;
            }
        }

        public string Expansion
        {
            get
            {
                return _expansion;
            }
            set
            {
                _expansion = value;
            }
        }

        public Abbreviation(string shortcut, string expansion)
        {
            _shortcut = shortcut;
            _expansion = expansion;
        }
    }
}
