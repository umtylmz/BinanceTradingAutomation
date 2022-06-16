// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Domain.Entity
{
    public class BaseEntity
    {
        [JsonProperty(PropertyName = "id")] //cosmos db için
        public string Id { get; set; }
    }
}
