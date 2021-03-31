using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Extensions
{
    public static class ModelStateDictionaryExtension
    {
        public static IDictionary<string, string> GetErrors(this ModelStateDictionary modelState)
        {
            var errors = modelState
               .Where(e => e.Value.Errors.Count > 0)
               .ToDictionary(e => e.Key, e => e.Value.Errors.FirstOrDefault().ErrorMessage);

            return errors;
        }
    }
}