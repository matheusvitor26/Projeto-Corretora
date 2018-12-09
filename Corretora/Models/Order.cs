using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corretora.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        [Display(Name = "Tipo")]
        public Type Type { get; set; }
        [Display(Name = "Ativo")]
        public int AssetID { get; set; }
        [Display(Name = "Cliente")]
        public int CustomerID { get; set; }
        [Display(Name = "Data")]
        public DateTime Date { get; set; }
        [Display(Name = "Quantidade")]
        public int Quantity { get; set; }
        [Display(Name = "Valor")]
        public decimal OrderValue { get; set; }
        public Status Status { get; set; }
    }
}