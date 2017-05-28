using System;
using System.Linq;
using System.Linq.Expressions;

namespace Xlent.Lever.Library.Core.Assert
{
    public static class GenericContract<TException>
        where TException : Exception 
    {
        public static void Require<TParameter>(string parameterName, TParameter parameterValue,
            Expression<Func<TParameter, bool>> requirementExpression)
        {
            var message = GetErrorMessageIfFalse(parameterName, parameterValue, requirementExpression);
            MaybeThrowException(message);
        }

        public static void RequireNotNull<TParameter>(string parameterName, TParameter parameterValue)
        {
            var message = GetErrorMessageIfNull(parameterName, parameterValue);
            MaybeThrowException(message);
        }

        public static void RequireNotNullOrWhitespace(string parameterName, string parameterValue)
        {
            var message = GetErrorMessageIfNullOrWhitespace(parameterName, parameterValue);
            MaybeThrowException(message);
        }

        public static void Require(Expression<Func<bool>> requirementExpression)
        {
            var message = GetErrorMessageIfFalse(requirementExpression);
            MaybeThrowException(message);
        }

        public static void Require(bool mustBeTrue, string message)
        {
            var m = GetErrorMessageIfFalse(mustBeTrue, message);
            MaybeThrowException(m);
        }

        public static string GetErrorMessageIfFalse<T>(string parameterName, T parameterValue, Expression<Func<T, bool>> requirementExpression)
        {
            if (requirementExpression.Compile()(parameterValue)) return null;

            var condition = requirementExpression.Body.ToString();
            condition = condition.Replace(requirementExpression.Parameters.First().Name, parameterName);
            return $"Contract violation: {parameterName} ({parameterValue}) is required to fulfil {condition}.";
        }

        public static string GetErrorMessageIfNull<T>(string parameterName, T parameterValue)
        {
            return parameterValue != null ? null : $"Contract violation: {parameterName} must not be null.";
        }

        public static string GetErrorMessageIfNullOrWhitespace(string parameterName, string parameterValue)
        {
            if (!string.IsNullOrWhiteSpace(parameterValue)) return null;
            var value = parameterValue == null ? "null" : $"\"{parameterValue}\"";
            return $"Contract violation: {parameterName} ({value}) must not be null, empty or whitespace.";
        }

        public static string GetErrorMessageIfFalse(Expression<Func<bool>> requirementExpression)
        {
            if (requirementExpression.Compile()()) return null;

            var condition = requirementExpression.Body.ToString();
            return $"Contract violation: The call must fulfil {condition}.";
        }

        public static string GetErrorMessageIfFalse(bool mustBeTrue, string message)
        {
            if (mustBeTrue) return null;
            
            return $"Contract violation: {message}.";
        }

        private static void MaybeThrowException(string message)
        {
            if (message == null) return;
            var exception = (TException) Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }
    }
}
