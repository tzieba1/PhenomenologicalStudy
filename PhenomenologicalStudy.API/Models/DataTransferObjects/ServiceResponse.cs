using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects
{
  /// <summary>
  /// Allows service response data to be transfered generically.
  /// Responses to clients passing requests to this API are wrapped with this object.
  /// Services are assumed to be OK (200) until possibley changing to informational (1xx),
  /// other success (2xx), redirection (3xx), client-side (4xx) or internal server error (5xx) 
  /// codes by some validation condition - where Status, Messages, and Success are modified 
  /// ServiceResponse is immediately returned when 4xx or 5xx status codes are issued.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ServiceResponse<T>
  {
    public T Data { get; set; }
    public bool Success { get; set; } = true;
    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
    public ICollection<string> Messages { get; set; } = new List<string>();
  }
}
