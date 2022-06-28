using Microsoft.AspNetCore.Hosting;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Server.Repos
{
    public class CardRepo : ICardRepo
    {
        private readonly string fileName = "\\cardStorage.json";
        private readonly string imageFolderPath = "\\image";
        private IHostingEnvironment _hostingEnvironment;

        public CardRepo(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public List<Card> All()
        {
            var obj = System.IO.File.ReadAllText(_hostingEnvironment.WebRootPath + fileName);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            var result = JsonSerializer.Deserialize<List<Card>>(obj, options).OrderBy(p => p.Name).ToList();

            return result;
        }

        public void Create(Card card)
        {
            if (card is null)
            {
                return;
            }

            var list = this.All();
            list.Add(card);
            var jsonString = JsonSerializer.Serialize(list);

            File.WriteAllText(_hostingEnvironment.WebRootPath + fileName, jsonString);
        }

        public void Delete(string name)
        {
            var list = this.All();
            foreach (var item in list.Where(p => p.Name == name))
            {
                var filePath = $"{_hostingEnvironment.WebRootPath}{imageFolderPath}\\{item.Image}";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            list.RemoveAll(p => p.Name == name);
            var jsonString = JsonSerializer.Serialize(list);

            System.IO.File.WriteAllText(_hostingEnvironment.WebRootPath + fileName, jsonString);
        }

        public Card Get(string name)
        {
            var result = All().FirstOrDefault(c => c.Name == name);
            return result;
        }

        public void Update(string oldName, Card card)
        {
            var item = Get(oldName);
            if (item is null)
            {
                return;
            }
            Delete(oldName);
            item.Name = card.Name;
            var filePath = $"{_hostingEnvironment.WebRootPath}{imageFolderPath}\\{item.Image}";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            item.Image = card.Image;
            Create(item);
        }

        public void Update(string oldName, string newName)
        {
            var item = Get(oldName);
            if (item is null)
            {
                return;
            }
            Delete(oldName);
            item.Name = newName;
            Create(item);
        }
    }
}
