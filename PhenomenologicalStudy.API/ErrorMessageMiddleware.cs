using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhenomenologicalStudy.API
{
    /// <summary>
    /// Class responsible for reading accept request header and content type of an HTTP request 
    /// </summary>
    public class ErrorMessageMiddleware
    {
        // The request delegate
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor for middleware that caches the request delegate
        /// </summary>
        /// <param name="next"></param>
        public ErrorMessageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Method called when middleware is invoked in the pipeline. Responsible for checking if the accept header and content type is application/xml or application/json and serializing the response accordingly.
        /// </summary>
        /// <param name="context">HTTP request information</param>
        /// <returns>Void Async Task</returns>
        public async Task InvokeAsync(HttpContext context, PhenomenologicalStudyContext dbContext)
        {
            HttpRequest request = context.Request;  // Use request status for deciding to change the response based on a detected error.
            bool noException = false;

            // ERROR; 406: Accept header required.
            if (!request.Headers.ContainsKey("Accept"))
            {
                context.Response.StatusCode = 406;
            }
            // ERROR; 415: Content-Type must be either XML or JSON format.
            else if (request.ContentType != "application/xml" && request.ContentType != "application/json")
            {
                context.Response.StatusCode = 415;
                CheckAndAssignAcceptHeader(context, request);
            }
            else   // "Accept" key included in header.
            {
                // Check for a value supplied to Accept header and output them to StringValues to be further processed.
                CheckAndAssignAcceptHeader(context, request);

                // Try passing on request before checking for other status errors after reaching endpoints in pipeline and returning a response.
                try 
                { 
                      await _next(context); 
                      noException = true;   // Needed for specific SQL error messages whenever DbUpdateException is not caught in a controller action via SaveChangesAsync().
                } 
                catch (Exception ex)  // Including specific SQL error message to save to database with ErrorMessage object and return as serialized object to client.
                {
                    ErrorMessage errorMessage = new ErrorMessage() { Message = ex.Message, StatusCode = 500 };
                    context.Response.StatusCode = 500;
                    dbContext.ErrorMessages.Add(errorMessage);
                    await dbContext.SaveChangesAsync();
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage), Encoding.UTF8);
                }
            }

            // Check status after response comes back from endpoint with possible 400-500 status codes.
            ushort status = Convert.ToUInt16(context.Response.StatusCode);
            if(status > 400 && status < 501 && noException)
            { 
                // Switch on status to assign an appropriate error message.
                ErrorMessage errorMessage = status switch
                {
                    404 => new ErrorMessage
                    {
                        StatusCode = Convert.ToUInt16(status),
                        Message = $"Resource not found in the API."
                    },
                    405 => new ErrorMessage
                    {
                        StatusCode = Convert.ToUInt16(status),
                        Message = $"Method specified in attempted request to endpoint is incorrect."
                    },
                    406 => new ErrorMessage
                    {
                        StatusCode = Convert.ToUInt16(status),
                        Message = $"Invalid request. An 'Accept' header must be included in the request."
                    },
                    415 => new ErrorMessage
                    {
                        StatusCode = Convert.ToUInt16(status),
                        Message = $"Content-Type must be either XML or JSON format."
                    },
                    500 => new ErrorMessage
                    {
                        StatusCode = Convert.ToUInt16(status),
                        Message = $"Unexpected error occured."
                    },
                    _ => new ErrorMessage
                    {
                        StatusCode = Convert.ToUInt16(status),
                        Message = $"Unexpected error."
                    }
                };

                // Add error meesage to database, save changes, and write error message string to request body.
                dbContext.ErrorMessages.Add(errorMessage);
                await dbContext.SaveChangesAsync();

                // Needed to check for Accept-Header value
                if (request.Headers.ContainsKey("Accept"))  // Check for Accept-Header key in header
                {
                    bool check = request.Headers.TryGetValue("Accept", out StringValues acceptHeader);
                    if (acceptHeader == "application/xml")       // Check for XML serialize. 
                    {
                        using MemoryStream stream = new MemoryStream();
                        XmlSerializer serializer = new XmlSerializer(typeof(ErrorMessage));
                        serializer.Serialize(stream, errorMessage);
                        await context.Response.WriteAsync(Encoding.UTF8.GetString(stream.ToArray()));
                    }
                    else  // Default to JSON serialize.
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage), Encoding.UTF8);
                }
                else  // Accept-Header does not exist; Only for 406 error
                    await context.Response.WriteAsync(errorMessage.Message);
            }
        }

        /// <summary>
        /// Check Accept Header from request header. 
        /// If it exists, then checks if it does not contains application/xml or application/json.
        /// If it does not, then default the accept header to application/json and set content-type of Response to application/json.
        /// Otherwise, assign content-type to response based on accept header values
        /// </summary>
        /// <param name="context">HTTP context information</param>
        /// <param name="request">HTTP request information</param>
        private static void CheckAndAssignAcceptHeader(HttpContext context, HttpRequest request)
        {
            if (request.Headers.TryGetValue("Accept", out StringValues acceptValue))
            {
                if (!acceptValue.ToArray().Contains("application/xml") && !acceptValue.ToArray().Contains("application/json"))
                {
                    context.Request.Headers.Remove("Accept");
                    context.Request.Headers.Add("Accept", new StringValues("application/json"));
                    context.Response.ContentType = "application/json";
                }
                else if (context.Request.Headers["Accept"] == "application/xml")
                {
                    context.Response.ContentType = "application/xml";
                }
                else if (context.Request.Headers["Accept"] == "application/json")
                {
                    context.Response.ContentType = "application/json";
                }
            }
        }
    }
}