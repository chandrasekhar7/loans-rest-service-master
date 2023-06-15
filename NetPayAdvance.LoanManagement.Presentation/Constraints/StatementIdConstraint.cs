using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace NetPayAdvance.LoanManagement.Presentation.Constraints
{
    public class StatementIdConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out var value) && value != null)
            {
                var stmtValues = value.ToString().Split('-');
                return stmtValues.Length == 2 && int.TryParse(stmtValues[0], out var l) &&
                       DateOnly.TryParseExact(stmtValues[1], "yyyyMMdd", out var d);
            }
            return false;
        }
    }
}

