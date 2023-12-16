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
		public string SubscriptionName { get; set; }
		public decimal SubPrice { get;set;}
		public int SubCount { get; set; }
		
		public Subscription()
		{
			if (SubscriptionName == "Subscription Free")
			{
				SubPrice = 0;
				SubCount = 10;
			}
            else if (SubscriptionName == "Subscription Trial")
            {
                SubPrice = 5;
                SubCount = 100;
            }
            else if (SubscriptionName == "Subscription Pro")
            {
                SubPrice = 20;
                SubCount = int.MaxValue;
            }


        }
    }
}

