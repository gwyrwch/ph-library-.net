using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ph.RouteConstraints
{
    public class ActionConstraint : IRouteConstraint
    {
        public IEnumerable<string> ActionsPossible { get; set; }
        
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, 
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            return ActionsPossible.Contains(values[routeKey]?.ToString().ToLowerInvariant());
        }
    }
}