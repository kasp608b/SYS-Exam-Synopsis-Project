﻿namespace OrderApiC
{
    public class OrderLine
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int NoOfItems { get; set; }
    }
}
