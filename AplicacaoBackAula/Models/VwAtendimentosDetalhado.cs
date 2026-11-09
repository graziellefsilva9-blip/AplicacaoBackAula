using System;
using System.Collections.Generic;

namespace AplicacaoBackAula.Models;

public partial class VwAtendimentosDetalhado
{
    public int Idatendimento { get; set; }

    public int Idcliente { get; set; }

    public string Cliente { get; set; } = null!;

    public DateOnly DataAtendimento { get; set; }

    public string StatusAtendimento { get; set; } = null!;

    public int Iditem { get; set; }

    public string Item { get; set; } = null!;

    public int QuantidadeItens { get; set; }

    public decimal ValorunitarioEntrada { get; set; }

    public int Idfuncionario { get; set; }

    public string Funcionario { get; set; } = null!;
}
