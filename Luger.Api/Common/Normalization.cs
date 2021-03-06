﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Luger.Api.Common
{
    public static class Normalization
    {
        public static string NormalizeBucketName(string bucket)
        {
            return Regex.Replace(bucket.Trim().ToLower(), "[^a-z0-9]", "");
        }

        public static string NormalizeLabelName(string label)
        {
            return Regex.Replace(label.Trim().ToLower(), "[^a-z0-9]", "");
        }

        public static Dictionary<string, string> NormalizeLogLabels(Dictionary<string, string> labels)
        {
            var normalizedLabels = labels
                .Select(kvp => KeyValuePair.Create(NormalizeLabelName(kvp.Key), kvp.Value))
                .GroupBy(g => g.Key)
                .Select(g => g.First());

            return new Dictionary<string, string>(normalizedLabels);
        }
    }
}