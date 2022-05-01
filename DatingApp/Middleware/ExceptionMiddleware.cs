using DatingApp.CustomException;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IWebHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate _next, ILogger<ExceptionMiddleware> _logger, IWebHostEnvironment _env)
        {
            next = _next;
            logger = _logger;
            env = _env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex.Message, ex.StackTrace);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = env.EnvironmentName.ToLower() == "development" ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));

                
            }
        }


    }
}
