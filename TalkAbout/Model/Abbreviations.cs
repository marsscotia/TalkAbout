using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Data;

namespace TalkAbout.Model
{
    /// <summary>
    /// 
    /// Class is single point of access 
    /// for abbreviations.  Also delegates
    /// serialisation.
    /// 
    /// Class is singleton to ensure single point of access.
    /// 
    /// </summary>
    public class Abbreviations
    {
        private static Abbreviations _instance;
        private List<Abbreviation> _abbreviationList;
        private JsonConverter _converter;
        private const string _filename = "abbreviations.txt";

        //error codes for CRUD methods
        private const int _success = 0;
        private const int _abbreviationExists = -1;
        private const int _containsSpace = 2;

        public static Abbreviations Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Abbreviations();
                }
                return _instance;
            }
        }

        public List<Abbreviation> AbbreviationList
        {
            get
            {
                return _abbreviationList;
            }
        }


        private Abbreviations()
        {
            _converter = new JsonConverter();
            _loadAbbreviationsFromFile();
        }

        private async void _loadAbbreviationsFromFile()
        {
            _abbreviationList = await _converter.GetAbbreviations(_filename);
        }

        //CRUD operations
        //Clients call these methods to create or delete abbreviations

        public int AddAbbreviation(string shortcut, string expansion)
        {
            int result = 1;
            if (_shortcutExists(shortcut))
            {
                result = _abbreviationExists;
            }
            else
            {
                if (shortcut.Contains(" "))
                {
                    result = _containsSpace;
                }
                else
                {
                    _abbreviationList.Add(new Abbreviation(shortcut, expansion));
                    result = _success;
                }
            }
            _save(result);
            return result;
        }

        public void DeleteAbbreviations(List<Abbreviation> selected)
        {
            for (int i = 0; i < selected.Count(); i++)
            {
                if (_abbreviationList.Contains(selected[i]))
                {
                    _abbreviationList.Remove(selected[i]);
                }
            }
            _save(_success);
        }

        public void DeleteAbbreviation(Abbreviation selected)
        {
            List<Abbreviation> list = new List<Abbreviation>();
            list.Add(selected);
            DeleteAbbreviations(list);
        }

        private bool _shortcutExists(string shortcut)
        {
            bool result = false;
            foreach (Abbreviation abbreviation in _abbreviationList)
            {
                if (abbreviation.Shortcut.ToLower().Equals(shortcut.ToLower()))
                {
                    result = true;
                }
            }
            return result;
        }

        

        private void _save(int result)
        {
            if (result == _success)
            {
                _converter.SaveAbbreviations(_filename);
            }
        }
    }
}
