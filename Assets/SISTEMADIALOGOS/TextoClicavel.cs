using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; // Necessário para detectar o mouse
using System;

// Este script transforma qualquer Texto em um Botão
public class TextoClicavel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI meuTexto;
    
    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corMouseEmCima = Color.yellow;

    private Action onClickAction; // Guarda a função que deve rodar ao clicar

    void Awake()
    {
        // Pega o texto automaticamente se não tiver sido arrastado
        if (meuTexto == null) meuTexto = GetComponent<TextMeshProUGUI>();
        meuTexto.color = corNormal;
    }

    // Chamado pelo Manager para configurar este botão
    public void Configurar(string texto, Action acaoAoClicar)
    {
        meuTexto.text = "> " + texto;
        onClickAction = acaoAoClicar;
    }

    // --- Eventos do Mouse (Substitui o componente Button) ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        meuTexto.color = corMouseEmCima; // Muda cor
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        meuTexto.color = corNormal; // Volta cor
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClickAction != null) onClickAction.Invoke(); // Executa a escolha
    }
}