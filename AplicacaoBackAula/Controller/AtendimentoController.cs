using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore;
using AplicacaoBackAula.DTO;
using AplicacaoBackAula.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;




namespace AplicacaoBackAula.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtendimentoController : ControllerBase
    {

        // DEFININDO ATRIBUTO DO CONTEXT
        private readonly BdAulaBackendContext _context;

        // MÉTODO CONSTRUTOR

        public AtendimentoController(BdAulaBackendContext context)
        {
            _context = context;
        }

        // LISTAR ATENDIMENTOS

        [HttpGet]
        public async Task<ActionResult<AtendimentoListDTO>> GetAll()
        {
            var atendimentos = await _context.Atendimentos
                .Select(a => new AtendimentoListDTO
                {
                    IdAtendimento = a.Idatendimento,
                    IdPessoa = a.Idpessoa,
                    NomePessoa = a.IdpessoaNavigation.Nome,
                    DataAtendimento = a.DataAtendimento,
                    StatusAtendimento = a.StatusAtendimento,

                    Itens = a.Atendimentoitems
                    .Select(i => new AtendimentoItemListDTO
                    {
                        IdAtendimento = i.Idatendimento,
                        IdItem = i.Iditem,
                        Item = i.IditemNavigation.Item1,
                        IdFuncionario = i.Idfuncionario,
                        NomeFuncionario = i.IdfuncionarioNavigation.IdpessoaNavigation.Nome,
                        cargo = i.IdfuncionarioNavigation.Cargo,
                        Quantidade = i.QuantidadeItens,
                        ValorUnitario = i.ValorunitarioEntrada
                    }).ToList()

                }).ToListAsync();

            return Ok(atendimentos);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<AtendimentoListDTO>> GetAtendimento(int id)
        {
            var atendimento = await _context.Atendimentos
                .Where(a => a.Idatendimento == id)
                .Select(a => new AtendimentoListDTO
                {
                    IdAtendimento = a.Idatendimento,
                    IdPessoa = a.Idpessoa,
                    NomePessoa = a.IdpessoaNavigation.Nome,
                    DataAtendimento = a.DataAtendimento,
                    StatusAtendimento = a.StatusAtendimento,

                    Itens = a.Atendimentoitems
                    .Select(i => new AtendimentoItemListDTO
                    {
                        IdAtendimento = i.Idatendimento,
                        IdItem = i.Iditem,
                        Item = i.IditemNavigation.Item1,
                        IdFuncionario = i.Idfuncionario,
                        NomeFuncionario = i.IdfuncionarioNavigation.IdpessoaNavigation.Nome,
                        cargo = i.IdfuncionarioNavigation.Cargo,
                        Quantidade = i.QuantidadeItens,
                        ValorUnitario = i.ValorunitarioEntrada
                    }).ToList()

                }).FirstOrDefaultAsync();

            if (atendimento == null)
            {
                return NotFound(new
                {
                    mensagem = "ATENDIMENTO NÃO LOCALIZADO!!!"
                });
            }

            return Ok(atendimento);
        }



        // Adicionar um Atendimento
        [HttpPost]

        public async Task<ActionResult<AtendimentoListDTO>> PostAtendimento(AtendimentoPostDTO dto)
        {
            //VERIFICAR A EXISTÊNCIA DA PESSOA
            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(a => a.Idpessoa == dto.IdPessoa);

            if (pessoa == null)
            {
                return NotFound(new
                {
                    mensagem = "PESSOA NÃO LOCALIZADA!!!"
                });
            }

            //CRIA O OBJETO ATENDIMENTO E PROJETA OS DADOS DO DTO NESSE OBJETO
            var atendimento = new Atendimento
            {
                Idpessoa = dto.IdPessoa,
                DataAtendimento = dto.DataAtendimento,
                StatusAtendimento = dto.StatusAtendimento,
                Atendimentoitems = dto.Itens
                    .Select(i => new Atendimentoitem
                    {
                        Iditem = i.IdItem,
                        Idfuncionario = i.IdFuncionario,
                        QuantidadeItens = i.Quantidade,
                        ValorunitarioEntrada = i.ValorUnitario
                    }).ToList()
            };

            //PREPARA OS DADOS PARA SEREM ENVIADOS AO BANCO DE DADOS
            _context.Atendimentos.Add(atendimento);

            //SALVA OS DADOS NO BANCO DE DADOS
            await _context.SaveChangesAsync();

            //RETORNA OS DADOS QUE FORAM ENVIADOS AO BANCO DE DADOS UTILIZANDO GetAtendimento
            return CreatedAtAction(
                nameof(GetAtendimento),
                new { id = atendimento.Idatendimento },
                dto);
        }

        //Alterar Atendimento
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAtendimento(int id, AtendimentoPutDTO dto)
        {
            var atendimento = await _context.Atendimentos
                .Include(a => a.Atendimentoitems)
                .FirstOrDefaultAsync(a => a.Idatendimento == id);

            if (atendimento == null)
            {
                return NotFound(new
                {
                    mensagem = "ATENDIMENTO NÃO LOCALIZADO!!!"
                });
            }

            atendimento.StatusAtendimento = dto.StatusAtendimento;

            //PEGANDO OS IDS DE IdItem e IdFuncionario DO dto PASSO PELO CLIENTE
            var ItensRecebidos = dto.Itens
            .Select(i => (i.IdItem, i.IdFuncionario)).ToHashSet();

            //COMPARAR OS IDS DE IdItem e IdFuncionario COM O DO BANCO DE DADOS.
            var ItensSeleciodosRemover = atendimento.Atendimentoitems
                .Where(i => !ItensRecebidos.Contains((i.Iditem, i.Idfuncionario))).ToList();

            //EXCLUI OS IDS QUE NÃO FORAM PASSADOS PELO CLIENTE VIA dto
            _context.Atendimentoitems.AddRange(ItensSeleciodosRemover);

            //VERIFICA E ATUALIZA OS ITENS EXISTENTES
            foreach (var itemDto in dto.Itens)
            {
                //TESTA NOVAMENTE A EXISTENCIA DO ITEM
                var ItemExistente = atendimento.Atendimentoitems
                .FirstOrDefault(i =>
                i.Iditem == itemDto.IdItem &&
                i.Idfuncionario == itemDto.IdFuncionario);


                //CASO ENCONTRE O ITEM ALTERA APENAS A QUANTIDADE E VALOR UNITÁRIO
                if (ItemExistente != null)
                {
                    ItemExistente.QuantidadeItens = itemDto.Quantidade;
                    ItemExistente.ValorunitarioEntrada = itemDto.ValorUnitario;
                }
                else
                {
                    //CASO NÃO ENCONTRE O ITEM ADICIONA UM NOVO ITEM NO BANCO DE DADOS
                    atendimento.Atendimentoitems.Add(new Atendimentoitem
                    {
                        Idatendimento = atendimento.Idatendimento,
                        Iditem = itemDto.IdItem,
                        Idfuncionario = itemDto.IdFuncionario,
                        QuantidadeItens = itemDto.Quantidade,
                        ValorunitarioEntrada = itemDto.ValorUnitario
                    });
                }

            }
                      //SALVA NO BANCO DE DADOS
                      await _context.SaveChangesAsync();

                     //NÃO RETORNA NADA
                     return NoContent();
        }

        [HttpDelete("{id:int}")]
      
          public async Task<ActionResult> DeleteAtendimento(int id)
        {
            //VERIFICA A EXISTENCIA DO ATENDIMENTO
            var atendimento = await _context.Atendimentos
           .Include(a => a.Atendimentoitems)
           .FirstOrDefaultAsync(a => a.Idatendimento == id);

            //CASO NÃO ENCONTRE O ATENDIMENTO PASSADO PELO CLIENTE VIA ID NO PARAMETRO
            //EMITE A MENSAGEM ABAIXΟ

            if (atendimento == null)
            {
                return NotFound(new
                {
                    mensagem = "ATENDIMENTO NÃO LOCALIZADO!!!"
                });
            }

            //PREPARA A EXCLUSÃO OS ITENS NA TABELA Atendimentolten
            _context.Atendimentoitems.RemoveRange(atendimento.Atendimentoitems);
            
            //PREPARA A EXCLUSÃO OS ITENS NA TABELA Atendimento
            _context.Atendimentos.RemoveRange(atendimento);
            
            //CONFIRMA AS EXCLUSÕES
            await _context.SaveChangesAsync();
           
            //NÃO RETORNA NADA
            return NoContent();
        }
    }
}
