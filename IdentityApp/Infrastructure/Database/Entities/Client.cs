using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Infrastructure.Database.Entities
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
