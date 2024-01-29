using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ExchangeClick.Entities

{
    
    public class Subscription
	{
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionId { get; set; }
		[Required]
		public string? SubscriptionName { get; set; }
		public decimal SubPrice { get;set;}
		public int SubCount { get; set; }
    }
}

