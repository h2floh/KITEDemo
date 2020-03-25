using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Microsoft.Identity.Client;

namespace AndroidApp
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        // Keystore 
        //https://coderwall.com/p/r09hoq/android-generate-release-debug-keystores
        //See
        // https://docs.microsoft.com/en-us/azure/active-directory/develop/tutorial-v2-android
        // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Xamarin-Android-specifics

        private AppConfiguration _config; 
        private string _accessToken = "Error - no access token request started";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Button update = FindViewById<Button>(Resource.Id.update);
            update.Click += OnButtonClicked;

            // Configuration
            AssetManager assets = this.Assets;
            _config = AppConfiguration.Config(assets.Open("settings.json"));

            // See https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Xamarin-Android-specifics
            // Moved this to login button
            //Task.Run(async () =>
            //{
            //    await LoginAsync().ConfigureAwait(false);
            //    update.Text = "Update";
            //});
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

            if ((sender as Button).Text == "Login")
            {
                Task.Run(async () =>
                {
                    await LoginAsync().ConfigureAwait(false);
                    (sender as Button).Text = "Update";
                });
            }
            else
            {
                RunOnUiThread(async () =>
                {
                    if (_accessToken.StartsWith(@"Error"))
                    {
                        status.Text = _accessToken;
                    }
                    else
                    {
                        //(sender as Button).Text = "Click me again!";
                        status.Text = await VehicleAPICallAsync(vehicleId.Text, backgroundId.Text).ConfigureAwait(true);
                    }
                });
            }
        }

        async Task<string> VehicleAPICallAsync(string vehicleId, string backgroundId)
        {
            
            var httpClient = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
            var request = new HttpRequestMessage(HttpMethod.Post, _config.apiUrl);
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

            var _app = PublicClientApplicationBuilder.Create(_config.clientId)
                .WithRedirectUri(_config.redirect_uri)
                .WithParentActivityOrWindow(() => this)
                .WithTenantId(_config.tenant)
                .Build();

            // always want to login
            AuthenticationResult result;
            var accounts = await _app.GetAccountsAsync();
            IAccount account = accounts.FirstOrDefault<IAccount>();

            string[] scopes = _config.scopes;

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
