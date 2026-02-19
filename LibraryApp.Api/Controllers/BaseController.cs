
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase;
