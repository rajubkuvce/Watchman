﻿namespace HealthService.API.Extensions
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return $"Status code:{StatusCode}\n\rMessage: {Message}";
        }
    }
}
