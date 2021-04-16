using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(AddProductCommand command)
        {
            var result = await _mediator.Send(command);

            return CreatedAtAction("Get", new { id = result }, result);
        }
    }
}
