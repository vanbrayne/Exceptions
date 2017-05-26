using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xlent.Lever.Library.Core.Exceptions.Client;

namespace Xlent.Lever.Library.Core.Exceptions
{
    public static class BllContract
    {
        public static void Require<TParameter>(string parameterName, TParameter parameterValue,
            Expression<Func<TParameter, bool>> requirementExpression)
        {
            var message = ContractSupport.GetErrorMessageIfFalse(parameterName, parameterValue, requirementExpression);
            if (message == null) return;
            throw new ContractException(message);
        }
        public static void Require(Expression<Func<bool>> requirementExpression)
        {
            var message = ContractSupport.GetErrorMessageIfFalse(requirementExpression);
            if (message == null) return;
            throw new ContractException(message);
        }
    }
}
