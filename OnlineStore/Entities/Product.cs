﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int? OrderId { get; set; }
        public Order? Order { get; set; }


    }
}
