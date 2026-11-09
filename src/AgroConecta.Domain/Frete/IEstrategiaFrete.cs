namespace AgroConecta.Domain.Frete;

/// <summary>
/// Estratégia de cálculo de frete. Novas regras entram como novas implementações,
/// sem alterar o serviço de pedidos (OCP).
/// </summary>
public interface IEstrategiaFrete
{
    string Nome { get; }

    decimal Calcular(decimal pesoKg, decimal distanciaKm);
}
