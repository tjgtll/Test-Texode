using Server.Models;
using System.Collections.Generic;

namespace Server.Repos
{
    public interface ICardRepo
    {
        Card Get(string name);
        List<Card> All();
        void Create(Card card);
        void Update(string oldName, Card card);
        void Update(string oldName, string newName);
        void Delete(string card);
    }
}
