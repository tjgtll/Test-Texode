using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Client.Models
{
    class ViewModel
    {
        public ObservableCollection<ViewCard> CardList { get; set; }

        public ViewModel()
        {
            CardList = new ObservableCollection<ViewCard>();
        }

        public ViewModel(List<ViewCard> cards)
        {
            CardList = new ObservableCollection<ViewCard>(cards);
        }
    }
}
