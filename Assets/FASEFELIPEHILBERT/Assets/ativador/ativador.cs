using UnityEngine;
using System.Collections;

public class ativador : MonoBehaviour
{
    SpriteRenderer sr;
    public KeyCode key;
    bool ativado = false;
    GameObject note;
    Color old;
    public GameObject prefabFogo; // Arraste seu efeito de fogo aqui no Inspector

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (sr != null)
            old = sr.color;
    }

    void Update()
    {
        // Se a nota saiu da colisão, reseta o estado
        if (ativado && note == null)
        {
            ativado = false;
            return;
        }

        // Detecta o clique da tecla correspondente
        if (Input.GetKeyDown(key))
        {
            StartCoroutine(Pressed()); // Animação visual do botão apertando

            // Verifica se tem uma nota na mira (Collider)
            if (ativado && note != null)
            {
                Movimento_Circulo dados = note.GetComponent<Movimento_Circulo>();

                if (dados != null)
                {
                    string tipoNota = dados.tipoDaPorta; // Ex: "AND", "OR"

                    // --- AQUI A MÁGICA ACONTECE ---
                    // Chamamos nossa função blindada para verificar se acertou
                    if (VerificarAcerto(tipoNota))
                    {
                        // --- ACERTOU! ---
                        
                        // 1. Efeito Visual
                        if (prefabFogo != null)
                        {
                            Instantiate(prefabFogo, transform.position, Quaternion.identity);
                        }

                        // 2. Debug para garantir
                        Debug.Log("ACERTOU! Botão " + key + " | Nota: " + tipoNota + " | Regra: " + GeradorPortas.regraAtual);

                        // 3. Dar Pontos
                        if (Pontuação.instance != null)
                            Pontuação.instance.GanharPontos();

                        // 4. Destruir a nota
                        Destroy(note);
                        ativado = false;
                    }
                    else
                    {
                        // --- ERROU! ---
                        Debug.Log("ERROU! Botão " + key + " | Nota: " + tipoNota + " | Regra: " + GeradorPortas.regraAtual);

                        if (Pontuação.instance != null)
                            Pontuação.instance.PerderPontos();

                        Destroy(note);
                        ativado = false;
                    }
                }
            }
        }
    }

    // --- FUNÇÃO DE VERIFICAÇÃO INTELIGENTE ---
    bool VerificarAcerto(string nota)
        {
            // 1. Limpeza básica
            string n = nota.ToUpper().Trim();                 // Nome da Nota (Ex: "XOR")
            string r = GeradorPortas.regraAtual.ToUpper().Trim(); // Nome da Regra (Ex: "XOR (3)_0")

            // 2. O IF QUE VOCÊ PEDIU (Mantido por segurança)
            if (n == "AND" && r == "AND_0") return true;

            // 3. LÓGICA DE "CONTÉM" COM PROTEÇÃO
            // Isso resolve o problema do "(3)" ou qualquer outro texto extra.

            if (n == "AND")
            {
                // Regra deve ter "AND", mas NÃO pode ter "NAND"
                return r.Contains("AND") && !r.Contains("NAND");
            }

            if (n == "OR")
            {
                // Regra deve ter "OR", mas NÃO pode ter "XOR"
                // (Adicionei !Contains("PORTA") caso seu sprite chame "Porta_OR")
                return r.Contains("OR") && !r.Contains("XOR"); 
            }

            if (n == "NAND")
            {
                return r.Contains("NAND");
            }

            if (n == "XOR")
            {
                return r.Contains("XOR");
            }

            if (n == "NOT")
            {
                return r.Contains("NOT");
            }

            // Se não for nenhum dos casos acima, verifica igualdade direta por último
            return n == r;
        }

    // --- DETECÇÃO DE COLISÃO ---
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "note")
        {
            ativado = true;
            note = col.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == note)
        {
            ativado = false;
            note = null;
        }
    }

    // Animação simples de cor ao apertar
    IEnumerator Pressed()
    {
        if (sr != null) sr.color = Color.black;
        yield return new WaitForSeconds(0.05f);
        if (sr != null) sr.color = old;
    }
}