using System;
using System.Linq.Expressions;
using Xlent.Lever.Library.Core;
using Xlent.Lever.Library.WebApi.Exceptions;

namespace Xlent.Lever.Library.WebApi.Assert
{
    public static class ServerContract
    {
        public static void Require<TParameter>(string parameterName, TParameter parameterValue,
            Expression<Func<TParameter, bool>> requirementExpression)
        {
            var message = ContractSupport.GetErrorMessageIfFalse(parameterName, parameterValue, requirementExpression);
            MaybeThrowException(message);
        }

        public static void RequireNotNull<TParameter>(string parameterName, TParameter parameterValue)
        {
            var message = ContractSupport.GetErrorMessageIfNull(parameterName, parameterValue);
            MaybeThrowException(message);
        }

        public static void RequireNotNullOrWhitespace(string parameterName, string parameterValue)
        {
            var message = ContractSupport.GetErrorMessageIfNullOrWhitespace(parameterName, parameterValue);
            MaybeThrowException(message);
        }

        public static void Require(Expression<Func<bool>> requirementExpression)
        {
            var message = ContractSupport.GetErrorMessageIfFalse(requirementExpression);
            MaybeThrowException(message);
        }

        private static void MaybeThrowException(string message)
        {
            if (message == null) return;
            throw new ServerContractException(message);
        }
    }
}
