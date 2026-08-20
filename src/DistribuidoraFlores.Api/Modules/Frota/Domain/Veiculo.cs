namespace DistribuidoraFlores.Api.Modules.Frota.Domain;

public class Veiculo
{
    public Guid Id { get; private set; }
    public string Placa { get; private set; }
    public string Modelo { get; private set; }
    public int CapacidadeKg { get; private set; }
    public bool Ativo { get; private set; }

    protected Veiculo()
    {
        Placa = null!;
        Modelo = null!;
    }

    public Veiculo(string placa, string modelo, int capacidadeKg)
    {
        if (string.IsNullOrWhiteSpace(placa))
            throw new ArgumentException("Placa é obrigatória.");

        if (capacidadeKg <= 0)
            throw new ArgumentException("Capacidade deve ser maior que zero.");

        Id = Guid.NewGuid();
        Placa = placa.ToUpperInvariant();
        Modelo = modelo;
        CapacidadeKg = capacidadeKg;
        Ativo = true;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}