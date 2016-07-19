using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkAbout.Model
{
    public class Category
    {
        private string _name;
        private List<Phrase> _phrases;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public List<Phrase> Phrases
        {
            get
            {
                return _phrases;
            }
            set
            {
                _phrases = value;
            }
        }


        /// <summary>
        /// 
        /// Constructor takes a name value and initialises
        /// an empty list of phrases.
        /// 
        /// Usually used by client to create new category.
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Category(string name)
        {
            _name = name;
            _phrases = new List<Phrase>();
        }


        /// <summary>
        /// 
        /// Constructor takes a name value and a list
        /// of phrases.
        /// 
        /// Usually used when deserialising.
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="phrases"></param>
        public Category(string name, List<Phrase> phrases)
        {
            _name = name;
            _phrases = phrases;
        }
    }
}
