﻿namespace ADSCrossPlatform.Code.Models
{
    public class ProductData
    {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string[]? Storages { get; set; }
        public double[]? QtyInStorages { get; set; }
    }
}
