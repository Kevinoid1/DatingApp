using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.trial
{
    public class TrialAttribute : IAsyncActionFilter
    {
        public DateOnly DateOfBirth { get; set; }

        public void checker()
        {
            var date = new DateOnly();
            var time = new TimeOnly();
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
