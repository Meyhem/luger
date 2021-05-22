﻿using Luger.Api.Features.Logging;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Luger.Api.Api.Models
{
    public class AddLogRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel? Level { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string>? Labels { get; set; }
    }
}
