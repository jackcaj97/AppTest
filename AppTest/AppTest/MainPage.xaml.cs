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
                var posts = JsonConvert.DeserializeObject<List<Post>>(result);

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
                Name = "test.jpg"
            });

            if (photo != null)
            {

                var photoImage = ImageSource.FromStream(() => { return photo.GetStream(); });

                try
                {

                    var client = new HttpClient();

                    sendPostImageRequest(client, photo);

                    // sendPostTextRequest();

                    // sendGetRequest(client);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                

                // Console.WriteLine(photo.Path);

                // PhotoImage.Source = photoImage;

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

        private async void sendPostImageRequest(HttpClient client, MediaFile photo) 
        {

            // StreamContent scontent = new StreamContent(photo.GetStream());
            
            //scontent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            //{
            //    FileName = "test.jpg",
            //    Name = "test"
            //};
            //scontent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            var multi = new MultipartFormDataContent();
            var stream = photo.GetStream();
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            var imageContent = new ByteArrayContent(bytes);

            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            multi.Add(imageContent, "test", "test.jpg");
            // multi.Add(scontent);

            var result = client.PostAsync("https://animalguess.azurewebsites.net", multi).Result;
            // var response = client.PostAsync("https://animalguess.azurewebsites.net", multi);
            // var result = response.Result;

            Console.WriteLine("Risposta Richiesta Post: " + result.ReasonPhrase + "---------" + result.ToString());
            Console.WriteLine("Richiesta Post: " + result.RequestMessage);
            Console.WriteLine("BODY RISPOSTA: " + result.Content.ReadAsStringAsync().Result);

            //// var client = new HttpClient();
            //var form = new MultipartFormDataContent();

            //// form.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            //form.Add(new StreamContent(photo.GetStream()), "test", "test.jpg");

            //var httpClient = new System.Net.Http.HttpClient();
            //var url = "https://animalguess.azurewebsites.net";
            //// var responseMSG = await httpClient.GetAsync(url);
            //var responseMSG = await httpClient.PostAsync(url, form);


            //if (responseMSG.IsSuccessStatusCode) {
            //    var responsePath = await responseMSG.Content.ReadAsStringAsync();
            //    Console.WriteLine(responsePath);
            //}
            //else
            //{
            //    Console.WriteLine("Richiesta: " + responseMSG.RequestMessage);
            //}
        }

        private void sendGetRequest()
        {

            var client = new HttpClient();

            var result = client.GetAsync("https://animalguess.azurewebsites.net").Result;
            Console.WriteLine(result.ReasonPhrase + "---------" + result.ToString());
        }

    }
}
