using Asp.Versioning;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Application.Users.LogInUser;
using Bookify.Application.Users.RegisterUser;
using Bookify.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Users;

[ApiController]
// Deprecated يعني مهمل واللي بيشتغل فيرجن 2 بس 
[ApiVersion(ApiVersions.V1)] // its supported to have multiple versions for the same controller
//[ApiVersion(ApiVersions.V2)] // , so we can have both v1 and v2 for the same controller PRRRRRRRRRRRRRRRR!!!
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("me")]
    //[MapToApiVersion(ApiVersions.V1)]
    //[Authorize(Roles = Roles.Registered)] شلناها لانو السطر اللي بعدها "البيرمشنز" اصلا مرتبط بالرولز فخلص هيك ضمنيا في اكثر من شرط
    [HasPermission(Permissions.UsersRead)] // = [Authorize(Permissions.UsersRead)]
    public async Task<IActionResult> GetLoggedInUser/*V1*/(CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }

    // هون صار عندي نفس الايند بوينت فالسواغر صار يضرب حردان يا مان
    // فلازم نظظظظظظظظظيف نضيف بكج Asp.Versioning.Mvc.ApiExplorer on the Infrastructure layer and then ونضبط الوضع بالDI>> AddApiExplorer attributes for the versioning
    //[HttpGet("me")]
    //[MapToApiVersion(ApiVersions.V2)]
    //[HasPermission(Permissions.UsersRead)] 
    //public async Task<IActionResult> GetLoggedInUserV2(CancellationToken cancellationToken)
    //{
    //    var query = new GetLoggedInUserQuery();

    //    var result = await _sender.Send(query, cancellationToken);

    //    return Ok(result.Value);
    //}

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn(
        LogInUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request.Email, request.Password);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }
}
