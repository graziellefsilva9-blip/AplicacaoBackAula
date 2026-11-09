using System;
using System.Collections.Generic;

namespace AplicacaoBackAula.Models;

public partial class VwProdutosCategoriasEstoque
{
    public int Iditem { get; set; }

    public string Produto { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public decimal PrecoVenda { get; set; }

    public string Unidade { get; set; } = null!;

    public int QuantidadeEstoque { get; set; }
}
