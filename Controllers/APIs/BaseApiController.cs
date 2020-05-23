using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Models;
using CoreApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CoreApi.Controllers.APIs
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = SchemeConstants.Api)]
    public class BaseApiController : Controller
    {
        /// <summary>
        /// Return success status with empty data
        /// </summary>
        /// <returns></returns>
        protected DataResponse Success()
        {
            HttpContext.Response.StatusCode = 200;

            return new DataResponse()
            {
                StatusCode = 200
            };
        }

        /// <summary>
        /// Response success status with data.
        /// </summary>
        /// <param name="dataResponse"></param>
        /// <returns></returns>
        protected DataResponse Success(object dataResponse)
        {
            HttpContext.Response.StatusCode = 200;

            return new DataResponse()
            {
                StatusCode = 200,
                Data = dataResponse
            };
        }

        /// <summary>
        /// Response success status with data and pagination support.
        /// </summary>
        /// <param name="dataResponse"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        protected DataResponse SuccessWithPagination(object dataResponse, int totalPage)
        {
            HttpContext.Response.StatusCode = 200;

            return new DataResponse()
            {
                StatusCode = 200,
                TotalPage = totalPage,
                Data = dataResponse
            };
        }

        /// <summary>
        /// Response bad request
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected DataResponse BadRequest(string message = "invalid_data", double statusCode = 400)
        {
            HttpContext.Response.StatusCode = 400;

            return new DataResponse()
            {
                StatusCode = statusCode,
                Message = message
            };
        }


        /// <summary>
        /// Return bad request with details fields error.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected DataResponse BadRequest(ModelStateDictionary modelState, string message = "invalid_data at fields {0}")
        {
            HttpContext.Response.StatusCode = 400;

            var response = new DataResponse()
            {
                StatusCode = 400,
                Message = "invalid_data"
            };

            // Get fields error
            if (modelState != null)
            {
                var errorsKey = from modelstate in ModelState.AsQueryable().Where(f => f.Value.Errors.Count > 0)
                    select new { modelstate.Key };

                if (errorsKey.Any())
                    response.Message = string.Format(message, string.Join(",", errorsKey.Select(x => x.Key)));
            }

            return response;
        }

        /// <summary>
        /// Return 404 Not Found
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected DataResponse NotFound(string message = "Không tìm thấy dữ liệu.", double statusCode = 404)
        {
            HttpContext.Response.StatusCode = 404;

            return new DataResponse()
            {
                StatusCode = statusCode,
                Message = message
            };
        }

        /// <summary>
        /// Response internal server error
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected DataResponse Error(string message = "unkown_error")
        {
            // throw exception
            throw new Exception(message);
        }

        protected DataResponse Error(IEnumerable<IdentityError> errors)
        {
            return Error(errors.Aggregate("", (current, identityError) => current + $"{identityError.Code} ({identityError.Description})\r\n"));
        }

        /// <summary>
        /// Support get field from Post FormCollections
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected string GetField(string fieldName)
        {
            return (Request.Form != null && Request.Form.ContainsKey(fieldName)) ? ((string)Request.Form[fieldName]).Trim() : String.Empty;
        }
    }
}
