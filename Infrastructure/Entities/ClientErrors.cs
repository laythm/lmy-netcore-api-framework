using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entities
{
    public partial class ClientErrors 
    {
        public ClientErrors()
        {
           
        }
        public string ID { get; set; }
        public string Browser { get; set; }
        public string UserID { get; set; }
        public string Error { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
