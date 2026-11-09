using System;
using System.Collections.Generic;

namespace AplicacaoBackAula.Models;

public partial class VwClientesEndereco
{
    public int Idpessoa { get; set; }

    public string Cliente { get; set; } = null!;

    public long Cpf { get; set; }

    public string Email { get; set; } = null!;

    public long Telefone { get; set; }

    public string Cep { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string Complemento { get; set; } = null!;
}
