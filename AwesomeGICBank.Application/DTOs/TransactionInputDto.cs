using AwesomeGICBank.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Application.DTOs
{
    public class TransactionInputDto
    {
        public DateTime Date { get; set; }
        public string AccountNumber { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
