﻿using AspNetCore.ApiGateway.Application.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiGateway.API.Application.Authorization
{
    public class GetAuthorizationService : IGetGatewayAuthorization
    {
        public void Authorize(AuthorizationFilterContext context)
        {
            //Put your authorization here
        }
    }

    public class GetWithParamsAuthorizationService : IGetWithParamsGatewayAuthorization
    {
        public void Authorize(AuthorizationFilterContext context)
        {
            //Put your authorization here
        }
    }
}
