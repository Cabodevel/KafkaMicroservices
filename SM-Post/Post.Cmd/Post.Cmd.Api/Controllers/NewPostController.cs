using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NewPostController : ControllerBase
    {
        private readonly ILogger<NewPostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }


        [HttpPost]
        public async Task<IActionResult> NewPostAsync(NewPostCommand command)
        {
            try
            {
                var id = Guid.NewGuid();
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return StatusCode(StatusCodes.Status201Created, new NewPostResponse
                {
                    Message = "New post creation request completed succesfully"
                });
            }
            catch (Exception e)
            {

                _logger.LogWarning("Bad request");

                return BadRequest(new BaseResponse
                {
                    Message = e.Message,
                });
            }
        }
    }
}
