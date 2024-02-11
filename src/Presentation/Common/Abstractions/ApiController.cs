using Application.Abstractions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Common.Abstractions;

[ApiController]
[Produces("application/json")]
[Route("/api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected T GetService<T>() where T : notnull => HttpContext.RequestServices.GetRequiredService<T>();

    protected IAppDbContext DbContext => GetService<IAppDbContext>();

    protected IMediator Mediator => GetService<IMediator>();

    protected IDateTimeProvider DateTimeProvider => GetService<IDateTimeProvider>();

    protected IMapper Mapper => GetService<IMapper>();
}