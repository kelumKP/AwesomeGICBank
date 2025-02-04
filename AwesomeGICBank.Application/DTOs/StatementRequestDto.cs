using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Application.DTOs
{
    public class StatementRequestDto
    {
        public string AccountNumber { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
