using System;
using System.Linq.Expressions;
using Xlent.Lever.Library.Core.Exceptions;

namespace Xlent.Lever.Library.Core.Assert
{
    public static class BllContract
    {
        public static void Require<TParameter>(string parameterName, TParameter parameterValue,
            Expression<Func<TParameter, bool>> requirementExpression)
        {
            GenericContract<ContractException>.Require(parameterName, parameterValue, requirementExpression);
        }

        public static void RequireNotNull<TParameter>(string parameterName, TParameter parameterValue)
        {
            GenericContract<ContractException>.RequireNotNull(parameterName, parameterValue);
        }

        public static void RequireNotNullOrWhitespace(string parameterName, string parameterValue)
        {
            GenericContract<ContractException>.RequireNotNullOrWhitespace(parameterName, parameterValue);
        }

        public static void Require(Expression<Func<bool>> requirementExpression)
        {
            GenericContract<ContractException>.Require(requirementExpression);
        }

        public static void Require(bool mustBeTrue, string message)
        {
            GenericContract<ContractException>.Require(mustBeTrue, message);
        }
    }
}
