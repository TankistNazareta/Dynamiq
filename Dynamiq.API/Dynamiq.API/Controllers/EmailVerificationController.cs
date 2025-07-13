using Dynamiq.API.Commands.EmailVerification;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dynamiq.API.Controllers
{
    [Route("emailVerification")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        private readonly IEmailVerificationRepo _emailRepo;
        private readonly ILogger<EmailVerificationController> _logger;
        private readonly IMediator _mediator;

        public EmailVerificationController(IEmailVerificationRepo emailRepo, ILogger<EmailVerificationController> logger, IMediator mediator)
        {
            _emailRepo = emailRepo;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EmailVerificationDto emailVerification)
        {
            var response = await _emailRepo.Update(emailVerification);

            _logger.LogInformation("Email Verification with id: {Id} was updated", emailVerification.Id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _emailRepo.GetAll();

            _logger.LogInformation("Retrieved all Email Verification records, count: {Count}", all?.Count() ?? -1);

            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _emailRepo.GetById(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmailVerificationDto emailVerification)
        {
            var response = await _emailRepo.Insert(emailVerification);

            _logger.LogInformation("Email Verification with id: {Id} was created", response.Id);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _emailRepo.Delete(id);

            _logger.LogInformation("Email Verification with id: {Id} was removed", id);

            return Ok("EmailVerificationData was removed");
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid id)
        {
            await _mediator.Send(new ConfirmEmailCommand(id));

            _logger.LogInformation("Email Verification with id: {Id} was confirmed", id);

            return Ok("You confirmed email successfully");
        }

        [HttpGet("token/{token}")]
        public async Task<IActionResult> GetByToken([FromRoute] string token)
        {
            var result = await _emailRepo.GetByToken(token);

            return Ok(result);
        }
    }
}
