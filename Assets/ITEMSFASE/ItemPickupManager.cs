using UnityEngine;
using System.Collections.Generic;

public class ItemPickupManager : MonoBehaviour
{
    public static ItemPickupManager Instance;
    public AudioClip unlockSound; 
    private AudioSource audioSource;

    [System.Serializable]
    public struct SkillPanelLink
    {
        public string nomeIdentificador; // Apenas para organização no Inspector
        public SkillType skillType;      // O tipo da habilidade (do seu Enum)
        public GameObject panelObject;   // O painel Canvas correspondente a essa skill
    }

    [Header("Lista de Telas de Habilidade")]
    // Aqui você vai arrastar cada painel para cada tipo de skill
    public List<SkillPanelLink> skillPanels; 

    // Variável para lembrar qual painel está aberto agora (para poder fechar)
    private GameObject currentActivePanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);


        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Cria um AudioSource se ainda não tiver
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Configura para tocar sons de UI, ignorando a escala de tempo
        audioSource.ignoreListenerPause = true; 
        audioSource.ignoreListenerVolume = false;

        // Garante que todos os painéis comecem fechados
        foreach (var link in skillPanels)
        {
            if(link.panelObject != null)
                link.panelObject.SetActive(false);
        }
    }

    public void ShowSkillPanel(SkillType skillToUnlock)
    {
        // 1. Procura na lista qual painel corresponde a essa skill
        SkillPanelLink foundLink = skillPanels.Find(x => x.skillType == skillToUnlock);

        if (foundLink.panelObject != null)
        {
            // 2. Guarda a referência e ativa o painel
            currentActivePanel = foundLink.panelObject;
            currentActivePanel.SetActive(true);

            // TOCA O EFEITO SONORO AQUI!
            if (unlockSound != null && audioSource != null)
            {
                // Usa PlayOneShot para não interromper outros sons e tocar o efeito
                audioSource.PlayOneShot(unlockSound);
            }

            // 3. Pausa o jogo
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError($"Nenhum painel configurado para a skill: {skillToUnlock}");
        }
    }

    // Função para o Botão "Continuar/Fechar"
    public void CloseCurrentPanel()
    {
        // 1. Desativa o painel que estiver aberto
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
        }

        // 2. Despausa o jogo (ISSO CONSERTA O SEU PROBLEMA DE TRAVAR)
        Time.timeScale = 1f;
    }
}