using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corretora.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int AccountID { get; set; }
        public int CustomerID { get; set; }
        public decimal Balance { get; set; }
    }
}