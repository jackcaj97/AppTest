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
using Xamarin.Forms.Xaml;

namespace AppTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecognitionActivity : ContentPage
    {
        private User user;  // Utente che effettua l'accesso.

        public RecognitionActivity()
        {
            InitializeComponent();

            CameraButton.Clicked += CameraButton_Clicked;
            GetCollectionButton.Clicked += GetCollection;

            user = UserState.user;

            UserEmailLabel.Text = user.email;

            // Si imposta la Source del CachedImage
            CachedImageView.Source = user.picture;
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
                    resultText.Text = "Result";
                    var jsonString = await sendPostImageRequest(client, photo);

                    Console.WriteLine("JSONSTRING: " + jsonString);

                    if(jsonString.Equals("Pretty nice this picture"))
                    {
                        resultText.Text = "" + jsonString;
                    }
                    else
                    {
                        Result result = JsonConvert.DeserializeObject<Result>(jsonString);
                        List<Prediction> predictionList = result.predictions;
                        Prediction firstValue = predictionList.ElementAt(0);
                        Prediction secondValue = predictionList.ElementAt(1);

                        if (secondValue.probability > 0.50)
                            resultText.Text = "Animal: " + firstValue.tagName + " - " + secondValue.tagName;
                        else
                            resultText.Text = "Animal: " + firstValue.tagName;
                    }

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

        private async void GetCollection(object sender, EventArgs e)
        {
            //var app = App.Current;
            //app.MainPage = new CollectionPage();

            // Si aggiorna la ContentPage in esecuzione.
            await Navigation.PushAsync(new CollectionPage());
        }
    }
}