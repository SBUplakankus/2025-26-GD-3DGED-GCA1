using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDGame.Scripts.Events.Channels;
using Microsoft.VisualBasic.FileIO;

namespace GDGame.Scripts.Systems
{
    public enum LanguageOption { English, Czech, Ukranian }
    public class LocalisationController
    {
        #region Fields
        private static LocalisationController _instance;
        private LanguageOption _currentLanguage;
        private readonly Dictionary<string, string> _englishDict;
        private readonly Dictionary<string, string> _ukranianDict;
        private readonly Dictionary<string, string> _czechDict;
        #endregion

        #region Constrcutors
        public LocalisationController()
        {
            _englishDict = LoadCSV(AppData.ENGLISH_CSV_PATH);
            _czechDict = LoadCSV(AppData.CZECH_CSV_PATH);
            _ukranianDict = LoadCSV(AppData.UKRANIAN_CSV_PATH);
            _currentLanguage = LanguageOption.English;
        }
        #endregion

        #region Accessors
        public static LocalisationController Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Ensure you call Initialise() first");

                return _instance;
            }
        }
        #endregion

        #region Methods
        public static void Initialise()
        {
            if (_instance != null) return;

            _instance = new LocalisationController();
        }

        private static Dictionary<string, string> LoadCSV(string filePath)
        {
            var dict = new Dictionary<string, string>();
            var parser = new TextFieldParser(filePath); 

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                dict.Add(fields[0], fields[1]);
            }

            return dict;
        }

        public string Get(string key)
        {
            var activeDict = _currentLanguage switch
            {
                LanguageOption.English => _englishDict,
                LanguageOption.Czech => _czechDict,
                LanguageOption.Ukranian => _ukranianDict,
                _ => _englishDict
            };

            if(activeDict.TryGetValue(key, out var translation))
                return translation;

            if(_englishDict.TryGetValue(key, out var fallback))
                return fallback;

            return $"ERROR: {key} NOT FOUND";
        }

        public void SetLanguage(LanguageOption language) => _currentLanguage = language;
        #endregion
    }
}
