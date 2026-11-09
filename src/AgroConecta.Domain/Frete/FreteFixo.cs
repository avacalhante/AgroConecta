using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Domain.Frete;

/// <summary>Frete fixo por entrega, independente de peso e distância.</summary>
public class FreteFixo(decimal valor = 25.00m) : IEstrategiaFrete
{
    public string Nome => "Fixo";

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

        return valor;
    }
}
