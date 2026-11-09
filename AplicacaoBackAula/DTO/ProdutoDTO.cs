namespace AplicacaoBackAula.DTO
{
    public class ProdutoDTO
    {
        // DADOS DA TABELA ITEM
        public int Idcategoria { get; set; }
        
        public string? Item { get; set; }
      
        public string? DescricaoItem { get; set; }
        
        public decimal Preco { get; set; }

        public string? TipoItem { get; set; }
        public string AtivoProdutoServico { get; set; }
    
     //DADOS TABELA PRODUTO
        public int CodigoPreco { get; set; }
        public decimal PrecoVenda { get; set; }
    
        public string? Unidade { get; set; }

}
}
