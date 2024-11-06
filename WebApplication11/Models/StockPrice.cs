namespace WebApplication11.Models
{
    public class StockPrice
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }   // Променяме от string на DateTime
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public DateTime RetrievedAt { get; set; }   // Добавяме ново свойство
    }
}
