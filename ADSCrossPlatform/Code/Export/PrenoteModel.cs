namespace ADS.Code.Export
{
    public class PrenoteModel
    {
        public Guid Id { get; set; }
        public string ProductID { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public string Row { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Qty { get; set; } = string.Empty;
        public bool IsPrimaryPlace { get; set; }
        public bool IsSalesLocation { get; set; }
        public byte[]? RowVersion { get; set; } // Оптимистическая блокировка
        public float OrderedQty { get; set; }
    }
}
