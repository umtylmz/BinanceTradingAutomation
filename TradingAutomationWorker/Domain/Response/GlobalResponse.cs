// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Domain.Response
{
    public class GlobalResponse
    {
        public bool IsSucceed { get; set; } = true;
        public string? Message { get; set; }
    }

    public class GlobalResponse<T> : GlobalResponse
    {
        public T? Data { get; set; }
    }
}
