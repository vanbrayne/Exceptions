using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xlent.Lever.Library.Core.Exceptions.Client;

namespace Xlent.Lever.Library.Core.Exceptions
{
    public static class ContractSupport
    {
        public static string GetErrorMessageIfFalse<T>(string parameterName, T parameterValue, Expression<Func<T, bool>> requirementExpression)
        {
            if (requirementExpression.Compile()(parameterValue)) return null;

            var condition = requirementExpression.Body.ToString();
            condition = condition.Replace(requirementExpression.Parameters.First().Name, parameterName);
            return $"{parameterName} ({parameterValue}) is required to fulfil {condition}.";
        }

        public static string GetErrorMessageIfFalse(Expression<Func<bool>> requirementExpression)
        {
            if (requirementExpression.Compile()()) return null;

            var condition = requirementExpression.Body.ToString();
            return $"Call must fulfil {condition}.";
        }
    }
}
