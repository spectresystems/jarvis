// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Jarvis.Core
{
    public interface IQueryResult : IEquatable<IQueryResult>
    {
        string Title { get; }
        string Description { get; }
        float Distance { get; }
        float Score { get; }
        QueryResultType Type { get; }
    }
}