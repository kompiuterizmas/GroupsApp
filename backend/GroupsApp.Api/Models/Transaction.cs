using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupsApp.Api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int GroupId { get; set; }

        // <-- padarome nullable, kad nebūtų privaloma inicializuoti
        public Group? Group { get; set; }

        public int PayerUserId { get; set; }
        public User? Payer { get; set; }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        [NotMapped]
        public Dictionary<int, decimal>? SplitDetails { get; set; }
    }
}