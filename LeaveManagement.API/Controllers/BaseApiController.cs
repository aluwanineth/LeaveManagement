using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public abstract class BaseApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));
}
