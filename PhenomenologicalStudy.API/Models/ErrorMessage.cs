using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhenomenologicalStudy.API.Models
{
    public class ErrorMessage
    {
      
        public Guid Id { get; set; }

       
        public String Message { get; set; }

      
        public UInt16 StatusCode { get; set; }


        public ErrorMessage(UInt16 statusCode, String message)
        {
            Message = message;
            StatusCode = statusCode;
        }
        public ErrorMessage(String message)
        {
            Id = Guid.NewGuid();
            Message = message;
        }

        public ErrorMessage() { }
    }
}
