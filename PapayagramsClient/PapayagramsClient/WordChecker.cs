﻿using NHunspell;
using PapayagramsClient.PapayagramsService;
using System;

namespace PapayagramsClient
{
    public static class WordChecker
    {
        private static string _spellCheckAff;
        private static string _spellCheckDict;

        public static bool ValidWord(string word, LanguageDC language)
        {
            // throws argument exception when language is not english or spanish
            SetLanguageDictionary(language);
            bool result = false;

            result = WordExists(word);

            if (!result)
            {
                string suggestion = GetSimilarWord(word);
                suggestion = RemoveAccents(suggestion);
                result = word == suggestion;
            }

            return result;
        }

        private static void SetLanguageDictionary(LanguageDC language)
        {
            if (language.Equals(LanguageDC.Spanish))
            {
                _spellCheckAff = "../Resources/Dictionaries/es_MX.aff";
                _spellCheckDict = "../Resources/Dictionaries/es_MX.dic";
            }
            else if (language.Equals(LanguageDC.English))
            {
                _spellCheckAff = "../Resources/Dictionaries/en_US.aff";
                _spellCheckDict = "../Resources/Dictionaries/en_US.dic";
            }
            else
            {
                throw new ArgumentException("The provided language does not exist or isn't implemented");
            }
        }

        private static string RemoveAccents(string word)
        {
            word = word.Replace("á", "a");
            word = word.Replace("é", "e");
            word = word.Replace("í", "i");
            word = word.Replace("ú", "u");
            word = word.Replace("ó", "o");

            return word;
        }

        private static string GetSimilarWord(string word)
        {
            using (Hunspell hunspell = new Hunspell(_spellCheckAff, _spellCheckDict))
            {
                return hunspell.Suggest(word)[0];
            }
        }

        private static bool WordExists(string word)
        {
            using (Hunspell hunspell = new Hunspell(_spellCheckAff, _spellCheckDict))
            {
                return hunspell.Spell(word);
            }
        }
    }
}
