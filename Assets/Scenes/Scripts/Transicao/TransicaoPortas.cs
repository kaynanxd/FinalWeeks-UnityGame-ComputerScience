using UnityEngine;

public class TransicaoPortas : TransicaoBase
{
    public Portas portas;

    protected override void Trocar()
    {
        if (portas != null && portas.PortaAberta())
        {
            TrocarCena(nomeCena);
        }
    }
}
