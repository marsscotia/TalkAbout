using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkAbout.Model
{
    /// <summary>
    /// 
    /// Class represents a phrase which has been saved by the user
    /// for future use.
    /// 
    /// </summary>
    public class Phrase
    {
        private string _name;
        private int _frequency;
        private DateTime _recent;

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

        public int Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
            }
        }

        public DateTime Recent
        {
            get
            {
                return _recent;
            }
            set
            {
                _recent = value;
            }
        }

        /// <summary>
        /// 
        /// Constructor takes a name value
        /// and sets other parameters to defaults.
        /// 
        /// Usually used by client to construct new phrase
        /// with values set to represent the fact it has not
        /// yet been used.
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Phrase(string name)
        {
            _name = name;
            _frequency = 0;
            _recent = DateTime.MinValue;
        }

        /// <summary>
        /// 
        /// Onstructor takes name, frequency, and a string
        /// representation of a datetime.
        /// 
        /// Usually used when deserialising.
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="frequency"></param>
        /// <param name="datetimestring"></param>
        public Phrase(string name, int frequency, string datetimestring)
        {
            _name = name;
            _frequency = frequency;
            DateTime recent = new DateTime();
            bool succeed = DateTime.TryParse(datetimestring, out recent);
            if (succeed)
            {
                _recent = recent;
            }
            else
            {
                _recent = DateTime.MinValue;
            }

        }

        
    }




}
