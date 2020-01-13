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
using System.Collections.ObjectModel;

namespace AppTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CollectionPage : ContentPage
    {
        private User user;  // Utente che effettua l'accesso.
        private ListView lvImageCollection;

        private ObservableCollection<CollectionImage> _collectionImages;
        public ObservableCollection<CollectionImage> CollectionImages
        {
            get
            {
                return _collectionImages ?? (_collectionImages = new ObservableCollection<CollectionImage>());
            }
        }

        public CollectionPage()
        {
            InitializeComponent();

            user = UserState.user;
            lvImageCollection = lvCollection;

            requestCollection();
        }

        private async void requestCollection()
        {
            try
            {

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("action", "collection");

                var jsonString = await sendPostRequest(client);
                Console.WriteLine("JSONSTRING Collection: " + jsonString);

                List<CollectionImage> collection = JsonConvert.DeserializeObject<List<CollectionImage>>(jsonString);

                foreach(CollectionImage image in collection)
                {
                    CollectionImages.Add(image);
                    Console.WriteLine(""+image.ToString());
                }

                lvCollection.ItemsSource = CollectionImages;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // Metodo per l'invio di una richiesta Post contenente un'immagine.
        private async Task<string> sendPostRequest(HttpClient client)
        {

            var multi = new MultipartFormDataContent();
            
            multi.Add(new StringContent(user.email), "utente");

            var result = await client.PostAsync("https://animalguess.azurewebsites.net", multi);
            // var response = client.PostAsync("https://animalguess.azurewebsites.net", multi);
            // var result = response.Result;
            var jsonString = await result.Content.ReadAsStringAsync();

            return jsonString;

        }

    }
}