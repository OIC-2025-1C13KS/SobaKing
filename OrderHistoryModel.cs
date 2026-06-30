using System;

namespace SobaKing
{
    public class OrderHistoryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        public string OrderDate { get; set; }
        public string ImageUrl { get; internal set; }
    }
}