using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Commands;
using IntregationTestsWithTestContainerAndAwsLocalStack.Api.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var request = new GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(request);

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(AddProductCommand command)
        {
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
    }
}
