﻿using Microsoft.Extensions.Configuration;
using System;

namespace Luger.Api.Features.Configuration
{
    public interface ILugerConfigurationProvider
    {
        BucketOptions? GetBucketConfiguration(string bucket);
        TimeSpan GetBucketRotationFrequency(string bucket);
        byte[] GetIssuesSigningKey();
        IConfigurationSection GetJwtBearerOptions();
    }
}