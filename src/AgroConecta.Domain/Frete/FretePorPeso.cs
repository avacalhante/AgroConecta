using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Domain.Frete;

/// <summary>Frete por peso e distância: taxa base + valor por kg + valor por km.</summary>
public class FretePorPeso(decimal taxaBase = 10.00m, decimal porKg = 2.50m, decimal porKm = 0.35m)
    : IEstrategiaFrete
{
    public string Nome => "Por peso";

    public decimal Calcular(decimal pesoKg, decimal distanciaKm)
    {
        if (pesoKg <= 0)
        {
            throw new RegraNegocioException("O peso do pedido deve ser maior que zero.");
        }

        if (distanciaKm < 0)
        {
            throw new RegraNegocioException("A distância não pode ser negativa.");
        }

        return decimal.Round(taxaBase + (pesoKg * porKg) + (distanciaKm * porKm), 2);
    }
}
