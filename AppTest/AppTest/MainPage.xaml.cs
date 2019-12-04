﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Newtonsoft.Json;
using System.Net.Http;
using System.Drawing;

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
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {

                var photoImage = ImageSource.FromStream(() => { return photo.GetStream(); });

                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(photo.GetStream()), "\"animalImage\"", $"\"{photo.Path}\"");

                var httpClient = new System.Net.Http.HttpClient();
                var url = "https://animalguess.azurewebsites.net";
                // var responseMSG = await httpClient.GetAsync(url);
                var responseMSG = await httpClient.PostAsync(url, content);

                if (responseMSG.IsSuccessStatusCode) {
                    var responsePath = await responseMSG.Content.ReadAsStringAsync();
                    Console.WriteLine(responsePath);
                }
                else
                {
                    Console.WriteLine("Stica");
                }

                // Console.WriteLine(photo.Path);

                PhotoImage.Source = photoImage;

                Xamarin.Forms.Image img = new Xamarin.Forms.Image();

            }

            
        }

    }
}