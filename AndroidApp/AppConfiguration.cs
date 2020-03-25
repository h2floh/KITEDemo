using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Newtonsoft.Json;

namespace AndroidApp
{
    class AppConfiguration
    {
        private static AppConfiguration _singleton = null;
        // User Unchangeable Settings
        public string redirect_uri = "msauth://com.companyname.androidapp/DWxBsg%2FQ8zSqNAzwnqv6YIZbJr4%3D";

        // Loadable / User Changable Settings
        public string apiUrl { get; set; }
        public string tenant { get; set; }
        public string clientId { get; set; }

        public string[] scopes { get; set; }

        public static AppConfiguration Config(Stream configFileStream)
        {
            if (_singleton == null)
            {
                using (StreamReader streamReader = new StreamReader(configFileStream))
                {
                    // Initialize
                    var settingsFileContent = streamReader.ReadToEnd();
                    _singleton = JsonConvert.DeserializeObject<AppConfiguration>(settingsFileContent);
                }
                    
            }

            return _singleton;
        }

        private AppConfiguration()
        {

        }

    }
}