using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Newtonsoft.Json;
using System.Net.Http;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using Plugin.Media.Abstractions;

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

            CameraButton.Clicked += CameraButton_Clicked;
        }

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
                

                // Console.WriteLine(photo.Path);

                // Xamarin.Forms.Image img = new Xamarin.Forms.Image();

            }

            
        }
        
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

        private async Task<string> sendPostImageRequest(HttpClient client, MediaFile photo) 
        {

            var multi = new MultipartFormDataContent();
            var stream = photo.GetStream();
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            var imageContent = new ByteArrayContent(bytes);

            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            multi.Add(imageContent, "test", "test.jpg");
            // multi.Add(scontent);

            var result = await client.PostAsync("https://animalguess.azurewebsites.net", multi);
            // var response = client.PostAsync("https://animalguess.azurewebsites.net", multi);
            // var result = response.Result;
            var jsonString = await result.Content.ReadAsStringAsync();
            
            return jsonString;

            //Console.WriteLine("Risposta Richiesta Post: " + result.ReasonPhrase + "---------" + result.ToString());
            //Console.WriteLine("Richiesta Post: " + result.RequestMessage);
            //Console.WriteLine("BODY RISPOSTA: " + jsonString);

        }

        private void sendGetRequest()
        {

            var client = new HttpClient();

            var result = client.GetAsync("https://animalguess.azurewebsites.net").Result;
            Console.WriteLine(result.ReasonPhrase + "---------" + result.ToString());
        }

    }
}
