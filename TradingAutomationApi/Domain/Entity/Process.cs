// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Domain.Entity
{
    public class Process : BaseEntity
    {
        public string Name { get; set; }
        public int Status { get; set; }
    }
}
