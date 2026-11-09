using AgroConecta.Domain.Frete;
using AgroConecta.Domain.Validacao;

namespace AgroConecta.Tests.Dominio;

public class ValidacaoDocumentoTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void Cpf_valido_e_aceito(string documento)
    {
        Assert.True(new ValidadorCpf().Validar(documento));
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("52998224724")]
    [InlineData("123")]
    [InlineData("")]
    public void Cpf_invalido_e_rejeitado(string documento)
    {
        Assert.False(new ValidadorCpf().Validar(documento));
    }

    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    public void Cnpj_valido_e_aceito(string documento)
    {
        Assert.True(new ValidadorCnpj().Validar(documento));
    }

    [Theory]
    [InlineData("11222333000180")]
    [InlineData("00000000000000")]
    [InlineData("123")]
    public void Cnpj_invalido_e_rejeitado(string documento)
    {
        Assert.False(new ValidadorCnpj().Validar(documento));
    }

    [Fact]
    public void Validadores_sao_intercambiaveis_pelo_contrato()
    {
        IValidadorDocumento validador = new ValidadorCpf();

        Assert.True(validador.Validar("52998224725"));
        Assert.Equal("CPF", validador.Tipo);
    }
}

public class FreteTests
{
    [Fact]
    public void Frete_fixo_ignora_peso_e_distancia()
    {
        IEstrategiaFrete estrategia = new FreteFixo(25m);

        Assert.Equal(25m, estrategia.Calcular(10m, 50m));
    }

    [Fact]
    public void Frete_por_peso_soma_taxa_por_kg_e_por_km()
    {
        IEstrategiaFrete estrategia = new FretePorPeso(10m, 2.5m, 0.35m);

        Assert.Equal(42m, estrategia.Calcular(10m, 20m));
    }

    [Fact]
    public void Frete_rejeita_peso_invalido()
    {
        IEstrategiaFrete estrategia = new FretePorPeso();

        Assert.Throws<Domain.Exceptions.RegraNegocioException>(
            () => estrategia.Calcular(0m, 10m));
    }

    [Fact]
    public void Estrategias_de_frete_sao_polimorficas()
    {
        IEstrategiaFrete[] estrategias = [new FreteFixo(30m), new FretePorPeso(10m, 2.5m, 0.35m)];

        var valores = estrategias.Select(estrategia => estrategia.Calcular(10m, 20m)).ToArray();

        Assert.Equal(new[] { 30m, 42m }, valores);
    }
}
