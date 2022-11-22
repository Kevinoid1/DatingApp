using DatingApp.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public static class Extension
    {
        public static void AddApplicationErrors(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime dateTime)
        {
            int age = DateTime.Today.Year - dateTime.Year;
            if (dateTime.AddYears(age) > DateTime.Today)
                age--;

            return age;
        }

        public static void AddPagination(this HttpResponse response,int currentPage, int itemsPerPage,
                                            int totalItems, int totalPages)
        {
            var pagination = new PaginationHeader(currentPage, itemsPerPage, totalPages, totalItems);
            var camelcaseFormatter = new JsonSerializerSettings();
            camelcaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(pagination, camelcaseFormatter));
            //adding this line to prevent cors error
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }


        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        public static int GetUserId(this ClaimsPrincipal User)
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        public static string GetUsername(this ClaimsPrincipal User)
        {
            return User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
