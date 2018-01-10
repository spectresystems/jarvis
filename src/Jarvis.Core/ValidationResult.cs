// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Jarvis.Core
{
    public sealed class ValidationResult
    {
        public bool HasError { get; }
        public string Message { get; }

        private ValidationResult(string message)
        {
            HasError = message != null;
            Message = message;
        }

        public static ValidationResult Ok()
        {
            return new ValidationResult(null);
        }

        public static ValidationResult Error(string message)
        {
            return new ValidationResult(message);
        }
    }
}