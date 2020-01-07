using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Extensions;
using DahlizApp.Data;
using Microsoft.AspNetCore.Http;

namespace DahlizApp.Core.Infrastructure
{
    public static class Checker
    {
        public static async Task CheckLangId(HttpContext context,DahlizDb db,string sessionName)
        {
            int id = context.GetLanguage(sessionName);
            if(id == 0)
            {
                await context.SetLanguage("en", db, sessionName);
            }
        }
        public static bool CheckList<T>(List<T> array)
        {
            bool result = true;
            if (array == null)
            {
                result = false;
            }

            foreach (var item in array)
            {
                if (item == null)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}
