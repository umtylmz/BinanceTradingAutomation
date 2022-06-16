// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Interface;

namespace Domain.Test.Model
{
    public class TestTaskDetail
    {
        public string TaskName { get; set; }
        public ITaskDetail TaskDetail { get; set; }
    }
}
