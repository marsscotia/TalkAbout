﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;
using Windows.Data.Json;

namespace TalkAbout.Data
{
    /// <summary>
    /// 
    /// Class serialises and deserialises data
    /// to and from json.
    /// 
    /// </summary>
    public class JsonConverter
    {
        private FileAccessor _accessor;

        //strings for category and phrase json values
        private const string _categoriesString = "categories";
        private const string _nameString = "name";
        private const string _frequencyString = "frequency";
        private const string _recentString = "recent";
        private const string _phrasesString = "phrases";

        //strings for abbreviation json values
        private const string _abbreviationsString = "abbreviations";
        private const string _shortcutString = "shortcut";
        private const string _expansionString = "expansion";

        //strings for pronunciation json values
        private const string _pronunciationsString = "pronunciations";
        private const string _wordString = "word";
        private const string _soundString = "sound";

        public JsonConverter()
        {
            _accessor = new FileAccessor();
        }

        /// <summary>
        /// 
        /// Method retrieves a list of category objects from
        /// specified filename within local storage.
        /// 
        /// Returns an empty list if anything goes wrong.
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<List<Category>> GetCategories(string filename)
        {
            

            List<Category> results = new List<Category>();
            JsonObject categoriesObject = new JsonObject();

            string jsonstring = await _accessor.getJsonString(filename);
            bool success = JsonObject.TryParse(jsonstring, out categoriesObject);
            if (success)
            {
                JsonArray categoriesArray = categoriesObject.GetNamedArray(_categoriesString);
                for (int i = 0; i < categoriesArray.Count(); i++)
                {
                    JsonObject categoryObject = categoriesArray[i].GetObject();
                    string categoryName = categoryObject.GetNamedString(_nameString);
                    JsonArray phrasesArray = categoryObject.GetNamedArray(_phrasesString);
                    List<Phrase> phrasesList = new List<Phrase>();
                    foreach (JsonValue phraseValue in phrasesArray)
                    {
                        JsonObject phraseObject = phraseValue.GetObject();
                        string phraseName = phraseObject.GetNamedString(_nameString);
                        int frequency = (int)phraseObject.GetNamedNumber(_frequencyString);
                        string dateTimeString = phraseObject.GetNamedString(_recentString);
                        phrasesList.Add(new Phrase(phraseName, frequency, dateTimeString));
                    }

                    results.Add(new Category(categoryName, phrasesList));
                }

                
                
            }


            return results;

        }

        public void SaveCategories(string filename)
        {
            JsonObject categoriesObject = new JsonObject();
            JsonArray categoriesArray = new JsonArray();
            foreach (Category category in Categories.Instance.CategoryList)
            {
                JsonObject categoryObject = new JsonObject();
                categoryObject[_nameString] = JsonValue.CreateStringValue(category.Name);
                JsonArray phrasesArray = new JsonArray();
                foreach (Phrase phrase in category.Phrases)
                {
                    JsonObject phraseObject = new JsonObject();
                    phraseObject[_nameString] = JsonValue.CreateStringValue(phrase.Name);
                    phraseObject[_frequencyString] = JsonValue.CreateNumberValue(phrase.Frequency);
                    phraseObject[_recentString] = JsonValue.CreateStringValue(phrase.Recent.ToString());
                    phrasesArray.Add(phraseObject);

                }
                categoryObject[_phrasesString] = phrasesArray;

                categoriesArray.Add(categoryObject);
            }

            categoriesObject[_categoriesString] = categoriesArray;

            string jsonString = categoriesObject.Stringify();

            _accessor.writeJsonFile(filename, jsonString);
        }

        public async Task<List<Abbreviation>> GetAbbreviations(string filename)
        {
            List<Abbreviation> result = new List<Abbreviation>();
            JsonObject abbreviationsObject = new JsonObject();

            string jsonString = await _accessor.getJsonString(filename);
            bool success = JsonObject.TryParse(jsonString, out abbreviationsObject);
            if (success)
            {
                Debug.WriteLine("JsonConverter.cs: abbreviations jsonString successfully parsed");
                JsonArray abbreviationsArray = abbreviationsObject.GetNamedArray(_abbreviationsString);
                for (int i = 0; i < abbreviationsArray.Count(); i++)
                {
                    JsonObject abbreviationObject = abbreviationsArray[i].GetObject();
                    string shortcut = abbreviationObject.GetNamedString(_shortcutString);
                    string expansion = abbreviationObject.GetNamedString(_expansionString);
                    result.Add(new Abbreviation(shortcut, expansion));
                }
                Debug.WriteLine("JsonConverter.cs: List has " + result.Count + " abbreviations.");
            }
            return result;
        }

        public void SaveAbbreviations(string filename)
        {
            JsonObject abbreviationsObject = new JsonObject();
            JsonArray abbreviationsArray = new JsonArray();

            foreach (Abbreviation abbreviation in Abbreviations.Instance.AbbreviationList)
            {
                JsonObject abbreviationObject = new JsonObject();
                abbreviationObject[_shortcutString] = JsonValue.CreateStringValue(abbreviation.Shortcut);
                abbreviationObject[_expansionString] = JsonValue.CreateStringValue(abbreviation.Expansion);
                abbreviationsArray.Add(abbreviationObject);
            }

            abbreviationsObject[_abbreviationsString] = abbreviationsArray;
            string jsonString = abbreviationsObject.Stringify();

            _accessor.writeJsonFile(filename, jsonString);
        }

        public async Task<List<Pronunciation>> GetPronunciations(string filename)
        {
            List<Pronunciation> result = new List<Pronunciation>();
            JsonObject pronunicationsObject = new JsonObject();

            string jsonString = await _accessor.getJsonString(filename);
            bool success = JsonObject.TryParse(jsonString, out pronunicationsObject);
            if (success)
            {
                Debug.WriteLine("JsonConverter.cs: pronunciations jsonString successfully parsed");
                JsonArray pronunciationsArray = pronunicationsObject.GetNamedArray(_pronunciationsString);
                for (int i = 0; i < pronunciationsArray.Count(); i++)
                {
                    JsonObject pronunciationObject = pronunciationsArray[i].GetObject();
                    string word = pronunciationObject.GetNamedString(_wordString);
                    string sound = pronunciationObject.GetNamedString(_soundString);
                    result.Add(new Pronunciation(word, sound));
                }
            }

            return result;
        }

        public void SavePronunciations(string filename)
        {
            JsonObject pronunicationsObject = new JsonObject();
            JsonArray pronunciationsArray = new JsonArray();

            foreach (Pronunciation pronunciation in Pronunciations.Instance.PronunciationList)
            {
                JsonObject pronunciationObject = new JsonObject();
                pronunciationObject[_wordString] = JsonValue.CreateStringValue(pronunciation.Word);
                pronunciationObject[_soundString] = JsonValue.CreateStringValue(pronunciation.Sound);
                pronunciationsArray.Add(pronunciationObject);
            }

            pronunicationsObject[_pronunciationsString] = pronunciationsArray;
            string jsonString = pronunicationsObject.Stringify();

            _accessor.writeJsonFile(filename, jsonString);
        }


    }
}
