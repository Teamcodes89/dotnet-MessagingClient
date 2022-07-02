using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Objects.External
{
    public class MessageRequest
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public byte[] Text { get; set; }
    }
}
