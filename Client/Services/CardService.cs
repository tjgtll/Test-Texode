using Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Client.Services
{
    public class CardService
    {
        private static string URL;
        private readonly string imageStringPath = $"/get/image?pathImage=";

        private static byte[] Read(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public CardService(string url)
        {
            URL = url;
        }

        public List<ViewCard> LoadAPI()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL + "/get");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(string.Empty).Result;
            var dataObjects = response.Content.ReadAsByteArrayAsync();
            string jsonString = System.Text.Encoding.Default.GetString(dataObjects.Result);
            List<ViewCard> result = JsonSerializer.Deserialize<List<ViewCard>>(jsonString);
            foreach (var item in result)
            {
                item.Image = URL + imageStringPath + item.Image;
            }

            client.Dispose();
            return result;
        }

        public ViewCard GetCard(string name)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(URL + "/get/c");
            HttpResponseMessage response = client.GetAsync($"?name={name}").Result;
            var dataObjects = response.Content.ReadAsByteArrayAsync();
            string jsonCard = System.Text.Encoding.Default.GetString(dataObjects.Result);
            ViewCard result = JsonSerializer.Deserialize<ViewCard>(jsonCard);

            result.Image = URL + imageStringPath + result.Image;

            return result;
        }

        public void DeleteCard(System.Collections.IList items)
        {
            foreach (var item in items)
            {
                if (item is ViewCard card)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(URL + "/delete");
                    HttpResponseMessage response = client.DeleteAsync($"?name={card.Name}").Result;
                }
            }
        }

        public void DeleteCard(ViewCard item)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL + "/delete");
            HttpResponseMessage response = client.DeleteAsync($"?name={item.Name}").Result;
        }

        public void AddCard(string addTextBox, string imagePath)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent($"{addTextBox}"), "Name");
            form.Add(new StringContent($"{Path.GetFileName(imagePath)}"), "Image");
            using (FileStream fs = new FileStream(imagePath, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite))
            {
                var file_content = new ByteArrayContent(Read(fs));
                form.Add(file_content, "ProfileImage", $"{Path.GetFileName(imagePath)}");
            }
            HttpResponseMessage response = httpClient.PostAsync($"{URL}/post", form).Result;
            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
            string sd = response.Content.ReadAsStringAsync().Result;
        }

        public void UpdateCard(string updateTextBox, string imagePath, string oldName)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL + "/put");

            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent($"{updateTextBox}"), "Name");
            form.Add(new StringContent("image"), "Image");

            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var file_content = new ByteArrayContent(Read(fs));
                form.Add(file_content, "ProfileImage", $"{Path.GetFileName(imagePath)}");
            }
            HttpResponseMessage response = client.PutAsync($"?oldName={oldName}", form).Result;
            response.EnsureSuccessStatusCode();
            client.Dispose();
            string sd = response.Content.ReadAsStringAsync().Result;
        }

        public void UpdateCard(string updateTextBox, string oldName)
        {
            HttpClient client = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent($"{oldName}"), "oldName");
            form.Add(new StringContent($"{updateTextBox}"), "newName");
            client.BaseAddress = new Uri(URL + "/put/n");
            HttpResponseMessage response = client.PutAsync($"?oldName={oldName}&newName={updateTextBox}", form).Result;

        }
    }
}
