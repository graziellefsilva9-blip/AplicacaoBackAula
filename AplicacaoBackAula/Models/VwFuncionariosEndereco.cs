using System;
using System.Collections.Generic;

namespace AplicacaoBackAula.Models;

public partial class VwFuncionariosEndereco
{
    public int Idfuncionario { get; set; }

    public string Funcionario { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public DateOnly DataAdmissao { get; set; }

    public string Cep { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string Complemento { get; set; } = null!;
}
