using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExchangeClick.Models.Enum;

namespace ExchangeClick.Entities
{
	public class User
	{
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		
        public int UserId{get;set;}
		[Required]
		public string? Name{get;set;}
		[Required]
        public string? LastName { get; set; }
		[Required]
		public string? Username { get; set; }
		[Required]
		public string? Password { get; set; }
		[Required]
        public string? Email { get; set; }
        public int? SubscriptionId { get; set; }
        public Role Role { get; set; } 
        [ForeignKey("SubscriptionId")]
        public virtual Subscription? Subscription { get; set; }  // Propiedad de navegación para el usuario relacionado
        
	}
}

