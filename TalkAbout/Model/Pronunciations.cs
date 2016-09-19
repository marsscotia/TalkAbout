using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Data;

namespace TalkAbout.Model
{
    public class Pronunciations
    {
        private static Pronunciations _instance;
        private ObservableCollection<Pronunciation> _pronunciationList;
        private JsonConverter _converter;
        private const string _filename = "pronunciations.txt";

        //error codes for CRUD methods
        private const int _success = 0;
        private const int _pronunciationExists = -1;
        private const int _containsSpace = -1;

        public static Pronunciations Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Pronunciations();
                }
                return _instance;
            }
        }

        public ObservableCollection<Pronunciation> PronunciationList
        {
            get
            {
                return _pronunciationList;
            }
        }

        private Pronunciations()
        {
            _converter = new JsonConverter();
            _pronunciationList = new ObservableCollection<Pronunciation>();
        }

        public async Task LoadPronunciationsFromFile()
        {
            List<Pronunciation> list = await _converter.GetPronunciations(_filename);
            _pronunciationList.Clear();
            foreach (Pronunciation item in list)
            {
                _pronunciationList.Add(item);
            }
        }

        //CRUD operations
        //Clients call these methods to create or delete abbreviations

        public int AddPronunciation(string word, string sound)
        {
            int result = 1;
            if (_wordExists(word))
            {
                result = _pronunciationExists;
            }
            else
            {
                if (word.Contains(" "))
                {
                    result = _containsSpace;
                }
                else
                {
                    _pronunciationList.Add(new Pronunciation(word, sound));
                    result = _success;
                }
            }
            _save(result);
            return result;
        }

        public void DeletePronunciations(List<Pronunciation> selected)
        {
            for (int i = 0; i < selected.Count(); i++)
            {
                if (_pronunciationList.Contains(selected[i]))
                {
                    _pronunciationList.Remove(selected[i]);
                }
            }
            _save(_success);
        }

        private bool _wordExists(string word)
        {
            bool result = false;
            foreach (Pronunciation pronunciation in _pronunciationList)
            {
                if (pronunciation.Word.ToLower().Equals(word.ToLower()))
                {
                    result = true;
                }
            }
            return result;
        }

        private void _save(int success)
        {
            if (success == _success)
            {
                _converter.SavePronunciations(_filename);
            }
        }
    }
}
