using System;
using System.Collections.Generic;

namespace Azurecito.Data.Entidades;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool EsAdmin { get; set; }

    public virtual ICollection<Foto> Fotos { get; set; } = new List<Foto>();
}
