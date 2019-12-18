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
        private User user;  // Utente che effettua l'accesso.

        public MainPage()
        {
            InitializeComponent();

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



            CameraButton.Clicked += CameraButton_Clicked;

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

            AuthenticationState.authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        // Listener per il pulsante che permette di scattare una foto.
        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() 
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Medium
            });

            if (photo != null)
            {
                var photoImage = ImageSource.FromStream(() => { return photo.GetStream(); });

                PhotoImage.Source = photoImage;

                try
                {

                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("action", "recognition");

                    var jsonString = await sendPostImageRequest(client, photo);

                    Console.WriteLine("JSONSTRING: " + jsonString);

                    Result result = JsonConvert.DeserializeObject<Result>(jsonString);
                    List<Prediction> predictionList = result.predictions;
                    Prediction firstValue = predictionList.ElementAt(0);
                    Prediction secondValue = predictionList.ElementAt(1);


                    resultText.Text = "Animal: " + firstValue.tagName + " - " + secondValue.tagName;



                    // sendPostTextRequest();
                    // sendGetRequest(client);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            }

        }

        // Metodo per l'invio di una richiesta Post contenente un'immagine.
        private async Task<string> sendPostImageRequest(HttpClient client, MediaFile photo) 
        {

            var multi = new MultipartFormDataContent();
            var stream = photo.GetStream();
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            var imageContent = new ByteArrayContent(bytes);

            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            multi.Add(imageContent, "image", "image.jpg");
            // multi.Add(scontent);
            multi.Add(new StringContent(user.email), "utente");

            var result = await client.PostAsync("https://animalguess.azurewebsites.net", multi);
            // var response = client.PostAsync("https://animalguess.azurewebsites.net", multi);
            // var result = response.Result;
            var jsonString = await result.Content.ReadAsStringAsync();
            
            return jsonString;

            //Console.WriteLine("Risposta Richiesta Post: " + result.ReasonPhrase + "---------" + result.ToString());
            //Console.WriteLine("Richiesta Post: " + result.RequestMessage);
            //Console.WriteLine("BODY RISPOSTA: " + jsonString);

        }

        // Listener per l'autenticazione.
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {

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

                    user = JsonConvert.DeserializeObject<User>(userJson);
                    // Console.WriteLine("User: " + user.ToString());

                    UserEmailLabel.Text = "Email: " + user.email;
                    
                    // Si imposta la Source del CachedImage
                    CachedImageView.Source = user.picture;

                }
            }
            else
            {
                Console.WriteLine("--------- WAIT A SECOND: User not authenticated!");
            }
        }



        // Metodo di invio di una richiesta Post testuale
        private void sendPostTextRequest(HttpClient client)
        {

            var robe = new Dictionary<String, String>
                    {
                        {
                            "chiavecosa", "valorecosa"
                        },
                        {
                            "chiavecosa2", "valorecosa2"
                        }
                    };

            var content = new FormUrlEncodedContent(robe);

            var result = client.PostAsync("https://animalguess.azurewebsites.net", content).Result;
            Console.WriteLine("Risposta Richiesta Post: " + result.ReasonPhrase + "---------" + result.ToString());
            Console.WriteLine("Richiesta Post: " + result.RequestMessage);
        }

        // Metodo di invio di una richiesta Get.
        private void sendGetRequest()
        {

            var client = new HttpClient();

            var result = client.GetAsync("https://animalguess.azurewebsites.net").Result;
            Console.WriteLine(result.ReasonPhrase + "---------" + result.ToString());
        }

        /**
         * Metodo tester per il prelievo di un oggetto JSON.
         */
        public async void readDataJson(object sender, EventArgs e)
        {
            // await readDataJsonAsync(sender, e);
            using (var client = new HttpClient())
            {
                // send a GET request  
                var uri = "http://jsonplaceholder.typicode.com/posts";
                var result = await client.GetStringAsync(uri);

                //handling the answer  
                var posts = JsonConvert.DeserializeObject<List<Result>>(result);

                Console.WriteLine("posts: " + posts.First());

                // generate the output  
                var post = posts.First();
                resultText.Text = "First post:\n\n" + post;
            }
        }
    }
}
