﻿using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Store_API.Data;
using System.Text;

namespace Store_API.Helpers
{
    public class CF
    {
        public static int GetInt(object val)
        {
            if (val is int) return (int)val;
            else
            {
                int d;
                Int32.TryParse(val + "", out d);
                return d;
            }
        }

        public static double GetDouble(object val)
        {
            if (val is double) return (double)val;
            else
            {
                double d;
                double.TryParse(val + "", out d);
                return d;
            }
        }

        public static long GetLong(object val)
        {
            if (val is long) return (long)val;
            else
            {
                long d;
                long.TryParse(val + "", out d);
                return d;
            }
        }

        public static decimal GetDecimal(object val)
        {
            if (val is decimal) return (decimal)val;
            else
            {
                decimal d;
                decimal.TryParse(val + "", out d);
                return d;
            }
        }

        public static string Base64ForUrlEncode(string str)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(str);
            return WebEncoders.Base64UrlEncode(encbuff);
        }

        public static string Base64ForUrlDecode(string str)
        {
            byte[] decbuff = WebEncoders.Base64UrlDecode(str);
            return Encoding.UTF8.GetString(decbuff);
        }

        public static double ConvertVndToUsd(double vnd)
        {
            double usd = 0;
            if (vnd <= 0) usd = 0;
            usd = vnd / 25000;
            return usd;
        }

        public static JsonSerializerSettings JsonFormat()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            return jsonSettings;
        }
    }
}
