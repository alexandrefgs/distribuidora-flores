namespace DistribuidoraFlores.Api.Modules.Clientes.Domain;

public enum TipoDocumento
{
    CPF,
    CNPJ
}

public class Documento
{
    public string Numero { get; private set; }
    public TipoDocumento Tipo { get; private set; }

    private Documento(string numero, TipoDocumento tipo)
    {
        Numero = numero;
        Tipo = tipo;
    }

    public static Documento Criar(string numeroInformado)
    {
        var numero = new string(numeroInformado.Where(char.IsDigit).ToArray());

        var tipo = numero.Length switch
        {
            11 => TipoDocumento.CPF,
            14 => TipoDocumento.CNPJ,
            _ => throw new ArgumentException("Documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ).")
        };

        var valido = tipo == TipoDocumento.CPF
            ? ValidarCpf(numero)
            : ValidarCnpj(numero);

        if (!valido)
            throw new ArgumentException($"{tipo} inválido.");

        return new Documento(numero, tipo);
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
            return false; // rejeita "111.111.111-11" etc.

        int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var digito1 = CalcularDigitoVerificador(cpf[..9], multiplicadores1);
        var digito2 = CalcularDigitoVerificador(cpf[..9] + digito1, multiplicadores2);

        return cpf.EndsWith($"{digito1}{digito2}");
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1)
            return false;

        int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var digito1 = CalcularDigitoVerificador(cnpj[..12], multiplicadores1);
        var digito2 = CalcularDigitoVerificador(cnpj[..12] + digito1, multiplicadores2);

        return cnpj.EndsWith($"{digito1}{digito2}");
    }

    private static int CalcularDigitoVerificador(string baseNumero, int[] multiplicadores)
    {
        var soma = baseNumero
            .Select((digito, indice) => (digito - '0') * multiplicadores[indice])
            .Sum();

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    public override string ToString() => Numero;
}