using GreenMarket.API.Options;
using GreenMarket.Application.UseCases.Commandes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace GreenMarket.API.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly string _webhookSecret;

    public StripeWebhookController(IMediator mediator, IOptions<StripeOptions> options)
    {
        _mediator      = mediator;
        _webhookSecret = options.Value.WebhookSecret;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _webhookSecret
            );

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;
                if (intent is null) return BadRequest();

                if (intent.Metadata.TryGetValue("commandeId", out var commandeIdStr)
                    && int.TryParse(commandeIdStr, out var commandeId))
                {
                    await _mediator.Send(new ValiderPaiementCommand(commandeId, intent.Id));
                }
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}