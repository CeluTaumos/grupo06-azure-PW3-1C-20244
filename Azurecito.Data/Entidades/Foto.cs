using System;
using System.Collections.Generic;

namespace Azurecito.Data.Entidades;

public partial class Foto
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string FotoUrl { get; set; } = null!;

    public bool EstaAprobada { get; set; }

    public virtual Usuario? User { get; set; }
}
