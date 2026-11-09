using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplicacaoBackAula.Models;
using AplicacaoBackAula.DTO;

namespace AplicacaoBackAula.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoItemController : ControllerBase
    {
        //DECLARAÇÃO DO ATRIBUTO _context
        private readonly BdAulaBackendContext _context;
        //DECLARAÇÃO DO MÉTODO CONSTRUTOR

        public ProdutoItemController(BdAulaBackendContext context)
        {
            //INSTANCIA DO ARTIBUTO context
            _context = context;
        }

        //LISTAR TODOS OS PRODUTOS

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {

            var produtos = await _context.Produtos
            .Include(p => p.IditemNavigation)
            .ToListAsync();

            return Ok(produtos);


        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProdutos(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.IditemNavigation)
                .FirstOrDefaultAsync(p => p.Iditem == id);

            if (produto == null)
            {
                return NotFound(new { mensagem = "Produto Não localizado" });
            }

            return Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult> PostProdutos(ProdutoDTO dados)

        {


            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ==========================================
                // 1 - CRIAR O ITEM
                // ==========================================

                var item = new Item
                {
                    Idcategoria = dados.Idcategoria,
                    Item1 = dados.Item,
                    DescricaoItem = dados.DescricaoItem,
                    Preco = dados.Preco,
                    TipoItem = dados.TipoItem,
                    AtivoProdutoServico = dados.AtivoProdutoServico
                };

                //PREPARA O OBJETO ITEM PARA ENVIAR PARA O BANCO DE DADOS
                _context.Items.Add(item);

                // Salva o ITEM no banco
                await _context.SaveChangesAsync();






                // ==========================================
                // 2 - PEGAR O ID GERADO
                // ==========================================

                int idItemGerado = item.Iditem;


                // ==========================================
                // 3 - CRIAR O PRODUTO
                // ==========================================

                var produto = new Produto
                {
                    Iditem = idItemGerado,

                    CodigoPreco = dados.CodigoPreco,

                    PrecoVenda = dados.PrecoVenda,

                    Unidade = dados.Unidade
                };

                //PREPARA O OBJETO PRODUTO PARA ENVIAR PARA O BANCO DE DADOS
                _context.Produtos.Add(produto);

                // Salva o PRODUTO
                await _context.SaveChangesAsync();


                // ==========================================
                // 4 - CONFIRMAR TRANSAÇÃO
                // ==========================================

                await transaction.CommitAsync();

                // ==========================================
                // 5 - RETORNA RESULTADO
                // ==========================================

                return CreatedAtAction(
             nameof(GetProdutos),
             new { id = produto.Iditem },
             produto
                );
            }
            catch (Exception ex)
            {
                // Se alguma operação falhar,
                // desfaz o INSERT do Item e do Produto.
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    mensagem = "Erro ao cadastrar produto.",
                    erro = ex.Message
                });
            }

        }
        //ALTERAR PRODUTO
        [HttpPut("{id}")]

        public async Task<IActionResult> PutProduto(int id, ProdutoDTO dados)
        {
            // Busca o Produto junto com o Item
            var produtoExistente = await _context.Produtos
            .Include(p => p.IditemNavigation)
            .FirstOrDefaultAsync(p => p.Iditem == id);

            if (produtoExistente == null)
            {
                {
                    return NotFound(new
                    {
                        mensagem = "Produto não encontrado."
                    });
                }
                //=================================
                // ATUALIZA DADOS DA TABELA PRODUTO
                //=================================
                produtoExistente.CodigoPreco = dados.CodigoPreco;
                produtoExistente.PrecoVenda = dados.PrecoVenda;
                produtoExistente.Unidade = dados.Unidade;


                // =============================
                // ATUALIZA DADOS DA TABELA ITEM
                //===============================
                produtoExistente.IditemNavigation.Idcategoria = dados.Idcategoria;
                produtoExistente.IditemNavigation.Item1 = dados.Item;
                produtoExistente.IditemNavigation.DescricaoItem = dados.DescricaoItem;
                produtoExistente.IditemNavigation.Preco = dados.Preco;

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound(new
                    {
                        mensagem = "Produto não encontrado."
                    });
                }
                throw;
            }
            return NoContent();

        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteProduto(int id)
        {
            using var transaction =
            await _context.Database.BeginTransactionAsync();
            try
            {
                var produto = await _context.Produtos
                 .FirstOrDefaultAsync(p => p.Iditem == id);
                if (produto == null)
                {
                    return NotFound(new
                    {
                        mensagen = "Produto não encontrado."

                    });

                    
                }

                // Remove o Produto
                _context.Produtos.Remove(produto);

                await _context.SaveChangesAsync();

                // Busca o Item correspondente
                var item = await _context.Items
                 .FirstOrDefaultAsync(i => i.Iditem == id);

                if (item != null)
                {
                    // Remove o Item
                    _context.Items.Remove(item);

                    await _context.SaveChangesAsync();
                }
                    await transaction.CommitAsync();

                 return NoContent();
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    mensagem = "Erro ao excluir produto.",
                   
                });
            }


        }


        private bool ProdutoExists(int id)
        {
            return _context.Produtos
               .Any(p => p.Iditem == id);
        }
    }
}





