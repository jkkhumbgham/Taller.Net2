namespace Presentacion.Modelos.VistaModelos;

public class CursoVistaModelo
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string Idioma { get; set; } = string.Empty;
    public bool EsPublicado { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime ActualizadoEn { get; set; }
}

public class CrearCursoModelo
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Nivel { get; set; } = "Principiante";
    public string Idioma { get; set; } = "Español";
    public bool EsPublicado { get; set; }
}
