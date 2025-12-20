using UnityEngine;

public class TransicaoGenerica : TransicaoBase
{
    [Header("Configuração de Prova (Opcional)")]
    [Tooltip("Marque isso se essa transição deve levar para uma prova em vez de outra fase comum.")]
    public bool irParaProva = false;

    [Tooltip("O ID da prova que será carregada (Ex: 1 para Fase 1, 3 para Fase 3).")]
    public int idDaProva = 1;

    [Tooltip("Nome exato da cena da prova (Ex: CenaProva).")]
    public string nomeCenaDaProva;

    protected override void Trocar()
    {
        // Verifica se é para ir para a prova e se o nome da cena foi configurado
        if (irParaProva && !string.IsNullOrEmpty(nomeCenaDaProva))
        {
            Debug.Log($"Transição de Prova ativada! Configurando ID: {idDaProva} e carregando {nomeCenaDaProva}");

            // 1. Configura o ID da fase atual no GameSession (Igual ao GeradorPortas)
            GameSession.FaseAtualIndex = idDaProva;

            // 2. Chama o método da base, mas passando o nome da cena da prova
            TrocarCena(nomeCenaDaProva);
        }
        else
        {
            // Comportamento original: Carrega a cena definida no campo 'Nome Cena' do script base
            TrocarCena(nomeCena); 
        }
    }
}