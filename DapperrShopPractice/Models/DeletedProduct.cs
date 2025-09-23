using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperrShopPractice.Models
{
    public class DeletedProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; //что б пустым не был
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
