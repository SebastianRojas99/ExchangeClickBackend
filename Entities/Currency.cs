using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExchangeClick.Entities
{
	public class Currency
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CurrencyId{ get; set; }

        [Required]
        public string? CurrencyName { get; set; }
        
        [MaxLength(4)]
        public string? CurrencySymbol { get; set; }
        [Required]
        public decimal CurrencyValue { get; set; }//valor en relacion al dolar
		public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set;}
        
        
	}
}

