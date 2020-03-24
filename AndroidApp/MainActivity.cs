using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Microsoft.Identity.Client;

namespace AndroidApp
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
      DataHost = "com.companyname.androidapp",
      DataScheme = "msauth",
      DataPath = "/DWxBsg/Q8zSqNAzwnqv6YIZbJr4=")]
    public class MsalActivity : BrowserTabActivity
    {
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        // Keystore 
        //https://coderwall.com/p/r09hoq/android-generate-release-debug-keystores
        //See
        // https://docs.microsoft.com/en-us/azure/active-directory/develop/tutorial-v2-android
        // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Xamarin-Android-specifics

        private string _accessToken;

        // These values have all to be updated if the app is created again
        string APIUrl = $"https://kite.cse.msft.flow-soft.com/Vehicle";
        private string tenant = "001ceeb6-e0d3-42e0-88fb-0e388f8c6675";
        private string clientid = "dfd5c23a-b4a9-46fb-ae2f-86d9dec12eff";
        //

        private string redirect_uri = "msauth://com.companyname.androidapp/DWxBsg%2FQ8zSqNAzwnqv6YIZbJr4%3D";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //Task.Run(async () =>
            //{
            //    await RefreshAccessTokenAsync().ConfigureAwait(false);
            //});

            // See https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Xamarin-Android-specifics
            Task.Run(async () =>
            {
                await LoginAsync().ConfigureAwait(false);
            });

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Button update = FindViewById<Button>(Resource.Id.update);
            update.Click += OnButtonClicked;


        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
            EditText backgroundId = FindViewById<EditText>(Resource.Id.BackgroundImage);
            EditText vehicleId = FindViewById<EditText>(Resource.Id.vehicleId);
            TextView status = FindViewById<TextView>(Resource.Id.status);

            RunOnUiThread(async () =>
            {
                if (_accessToken.StartsWith(@"Error"))
                {
                    status.Text = "No access Token";
                }
                else
                {
                    //(sender as Button).Text = "Click me again!";
                    status.Text = await VehicleAPICallAsync(vehicleId.Text, backgroundId.Text).ConfigureAwait(true);
                }
            });
        }

        async Task<string> VehicleAPICallAsync(string vehicleId, string backgroundId)
        {
            
            var httpClient = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
            var request = new HttpRequestMessage(HttpMethod.Post, APIUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

            var body = @"{ ""vehicleId"" : " + vehicleId +
                        @", ""number"" : " + backgroundId +
                        @"}";

            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    return $"Error: {statusCode} {response.ReasonPhrase}";
                }

                return "Success";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        async Task LoginAsync()
        {

            var _app = PublicClientApplicationBuilder.Create(clientid)
                .WithRedirectUri(redirect_uri)
                .WithParentActivityOrWindow(() => this)
                .WithTenantId(tenant)
                .Build();

            // always want to login
            AuthenticationResult result;
            var accounts = await _app.GetAccountsAsync();
            IAccount account = accounts.FirstOrDefault<IAccount>();

            string[] scopes = new string[] { "openid", "offline_access", "api://KiteDemoPlatformAPI/user_impersonation" };

            try
            {
                // We don't want to login silently
                //result = await _app.AcquireTokenSilent(scopes, account)
                //                  .ExecuteAsync();
                result = await _app.AcquireTokenInteractive(scopes)
                                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                result = await _app.AcquireTokenInteractive(scopes)
                                    .ExecuteAsync();
            }

            _accessToken = result.AccessToken;
        }

        // Keeping code for reuse in other apps/demos
        async Task RefreshAccessTokenAsync()
        {
            var scopes = System.Net.WebUtility.UrlEncode("openid offline_access api://KiteDemoPlatformAPI/user_impersonation");
            var redirect_uri = System.Net.WebUtility.UrlEncode("https://kite.cse.msft.flow-soft.com");
            var client_secret = System.Net.WebUtility.UrlEncode(@""); // Not good security practice to store secret in app
            var refresh_token = @""; // Refresh Token of one user, should be stored externally, this was for demo purposes 

            string tokenUrl = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";
            var httpClient = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            var body = $"client_id={clientid}" +
             $"&scope={scopes}" +
             $"&redirect_uri={redirect_uri}" +
             @"&grant_type=refresh_token" +
             $"&client_secret={client_secret}" +
             $"&refresh_token={refresh_token}";

            request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    throw new Exception(statusCode.ToString());
                }
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = Newtonsoft.Json.Linq.JObject.Parse(content);
                _accessToken = json["access_token"].ToString();
            }
            catch (Exception e)
            {
                _accessToken = $"Error: {e.Message}";
            }
        }

        protected override void OnActivityResult(int requestCode,
                                         Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode,
                                                                                    resultCode,
                                                                                    data);
        }
    }


}
