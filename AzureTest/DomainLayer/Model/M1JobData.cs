// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace Domain.Model
{
    public class M1JobData
    {
        public string Name { get; set; }
        public M1StatusEnum Status { get; set; } = M1StatusEnum.NotStarted;
    }
}
