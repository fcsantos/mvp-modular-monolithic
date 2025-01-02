using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyMonolithicApp.Products.Application.Commands;
using MyMonolithicApp.Products.Application.DTOs;
using MyMonolithicApp.Products.Application.Queries;
using System.Net;

namespace MyMonolithicApp.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações de CRUD de Produtos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lista todos os produtos cadastrados.
        /// </summary>
        /// <returns>Uma lista de <see cref="ProductDto"/>.</returns>
        /// <response code="200">Retorna a lista de produtos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);
        }

        /// <summary>
        /// Retorna um produto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Retorna o produto encontrado.</returns>
        /// <response code="200">Retorna o produto.</response>
        /// <response code="404">Se o produto não for encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductDto>> Get(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            return Ok(product);
        }

        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="command">Dados do produto a criar.</param>
        /// <returns>Retorna o produto criado.</returns>
        /// <response code="201">Retorna o produto criado.</response>
        /// <response code="400">Se houver falha de validação (capturada via ValidationBehavior + Middleware).</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductCommand command)
        {
            // Se "Name" estiver vazio, o ValidationBehavior lança ValidationException,
            // e o GlobalExceptionMiddleware retorna 400 com detalhes.

            var product = await _mediator.Send(command);

            // Retorna 201 Created com a rota de GET e o objeto criado
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">ID do produto a atualizar.</param>
        /// <param name="command">Dados de atualização do produto.</param>
        /// <returns>Retorna o produto atualizado.</returns>
        /// <response code="200">Retorna o produto atualizado.</response>
        /// <response code="400">Se houver falha de validação.</response>
        /// <response code="404">Se o produto não for encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductCommand command)
        {
            // Ao invés de "if (command.Id != id) return BadRequest(...)",
            // passamos o ID da rota para o Handler decidir se é coerente.
            command.RouteId = id;

            var updatedProduct = await _mediator.Send(command);

            return Ok(updatedProduct);
        }

        /// <summary>
        /// Deleta um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto a deletar.</param>
        /// <response code="204">Se o produto foi deletado com sucesso.</response>
        /// <response code="404">Se não existir produto com este ID.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Se o produto não existir, o Handler lança NotFoundException
            await _mediator.Send(new DeleteProductCommand(id));

            return NoContent();
        }
    }
}
