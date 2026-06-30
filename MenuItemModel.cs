using System.ComponentModel;

namespace SobaKing
{
    public class MenuItemModel : INotifyPropertyChanged
    {
        public int Id { get; set; }

        // Non-nullable default values
        public string Name { get; set; } = "";
        public int Price { get; set; }
        public string ImageUrl { get; set; } = "";

        // Drink / SoftDrink / Food
        public string Category { get; set; } = "";

        private int quantity = 1;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        // Nullable-safe event
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
