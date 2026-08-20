namespace DistribuidoraFlores.Api.Modules.Frota.Domain;

public class Entrega
{
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid VeiculoId { get; private set; }
    public Guid MotoristaId { get; private set; }
    public StatusEntrega Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataConclusao { get; private set; }

    protected Entrega() { }

    public Entrega(Guid pedidoId, Guid veiculoId, Guid motoristaId)
    {
        Id = Guid.NewGuid();
        PedidoId = pedidoId;
        VeiculoId = veiculoId;
        MotoristaId = motoristaId;
        Status = StatusEntrega.AguardandoSaida;
        DataCriacao = DateTime.UtcNow;
    }

    public void IniciarRota()
    {
        if (Status != StatusEntrega.AguardandoSaida)
            throw new InvalidOperationException("Só é possível iniciar rota de uma entrega aguardando saída.");

        Status = StatusEntrega.EmRota;
    }

    public void Concluir()
    {
        if (Status != StatusEntrega.EmRota)
            throw new InvalidOperationException("Só é possível concluir uma entrega em rota.");

        Status = StatusEntrega.Concluida;
        DataConclusao = DateTime.UtcNow;
    }
}