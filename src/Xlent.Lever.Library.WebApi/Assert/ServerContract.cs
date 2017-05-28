using System;
using System.Linq.Expressions;
using Xlent.Lever.Library.Core.Assert;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.WebApi.Exceptions;

namespace Xlent.Lever.Library.WebApi.Assert
{
    public static class ServerContract
    {
        public static void Require<TParameter>(string parameterName, TParameter parameterValue,
            Expression<Func<TParameter, bool>> requirementExpression)
        {
            GenericContract<ServerContractException>.Require(parameterName, parameterValue, requirementExpression);
        }

        public static void RequireNotNull<TParameter>(string parameterName, TParameter parameterValue)
        {
            GenericContract<ServerContractException>.RequireNotNull(parameterName, parameterValue);
        }

        public static void RequireNotNullOrWhitespace(string parameterName, string parameterValue)
        {
            GenericContract<ServerContractException>.RequireNotNullOrWhitespace(parameterName, parameterValue);
        }

        public static void Require(Expression<Func<bool>> requirementExpression)
        {
            GenericContract<ServerContractException>.Require(requirementExpression);
        }
        public static void Require(bool mustBeTrue, string message)
        {
            GenericContract<ContractException>.Require(mustBeTrue, message);
        }
    }
}
