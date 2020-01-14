using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Auth;

using Newtonsoft.Json;
using System.Net.Http;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using Plugin.Media.Abstractions;
using FFImageLoading.Forms;

namespace AppTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();

            LoginButton.Clicked += TryGoogleLogin;

            string clientId = null;
            string redirectURI = null;

            switch(Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = "829469651959-5giacqvqkr011ghnod42jgnvo4lm0o3i.apps.googleusercontent.com";
                    redirectURI = "com.googleusercontent.apps.829469651959-5giacqvqkr011ghnod42jgnvo4lm0o3i:/oauth2redirect";
                    break;
                case Device.Android:
                    clientId = "829469651959-uj01l00gc3u3g7d495h1sl9vdms7587f.apps.googleusercontent.com";
                    redirectURI = "com.googleusercontent.apps.829469651959-uj01l00gc3u3g7d495h1sl9vdms7587f:/oauth2redirect";
                    break;
            }


            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                "https://www.googleapis.com/auth/userinfo.email",
                new Uri("https://accounts.google.com/o/oauth2/auth"),
                new Uri(redirectURI),
                new Uri("https://www.googleapis.com/oauth2/v4/token"),
                null,
                true);

            // Successful authentication event
            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            AuthenticationState.authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        // Listener per l'autenticazione.
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            // Se l'utente è stato autenticato con successo si tenta di accedere alle informazioni dell'account Google tramite una richiesta GET.
            if (e.IsAuthenticated)
            {
                Console.WriteLine("------------ YATTASO: User successfully authenticated!");
                string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
                var request = new OAuth2Request("GET", new Uri(UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    string userJson = response.GetResponseText();
                    // Console.WriteLine("JSON User: " + userJson);

                    User userObject = JsonConvert.DeserializeObject<User>(userJson);
                    // Console.WriteLine("User: " + user.ToString());

                    UserState.user = userObject;

                    // Si aggiorna la ContentPage in esecuzione.
                    var app = App.Current;
                    app.MainPage = new NavigationPage(new RecognitionActivity());
                }
                else
                {
                    // Aggiungere notifica di errore nell'ottenimento di informazioni dall'account Google.
                }
            }
            else
            {
                Console.WriteLine("--------- WAIT A SECOND: User not authenticated! --- ");
            }
        }

        async void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            // Gestire eventuali errori nel login.
            Application.Current.Quit();
        }

        private void TryGoogleLogin(object sender, EventArgs e)
        {
            var app = App.Current;
            app.MainPage = new MainPage();
        }
    }
}
