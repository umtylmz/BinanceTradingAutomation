// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace AzureTest.ExtensionLayer
{
    internal static class DateTimeExtension
    {
        internal static DateTime ToTurkeyStandardTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));
        }
    }
}
