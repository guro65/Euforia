using UnityEngine;
using System.Collections;
using UnityEngine.U2D;

public class Inimigo : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public int vida = 100;
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private bool estaSeguindo;
    [SerializeField] private bool estaAtirando;
    [SerializeField] private float distanciaDeteccao = 10.0f;
    [SerializeField] private float distanciaAtaque = 5.0f;
    [SerializeField] private float velocidadeEspecial = 7.0f;
    [SerializeField] private float tempoAntesDoEspecial = 2.0f;
    [SerializeField] private float recargaAtaqueEspecial = 5.0f;
    private Animator animator;
    private Rigidbody rb;
    private bool estaVivo = true;
    private Vector3 ultimaPosicaoConhecida;
    private bool playerNaAreaDeAtaque;
    private bool podeUsarAtaqueEspecial = true;
    private bool ataqueEspecialAtivo = false;
    private bool ataqueNormalAtivo = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        estaSeguindo = false;
        estaAtirando = false;
        animator.SetBool("EstaParado", true);
    }

    // Update is called once per frame
    void Update()
    {
        float distanciaAtePlayer = Vector3.Distance(transform.position, player.position);

        if (estaVivo)
        {
            // Faz o boss olhar sempre na direção do jogador
            OlhaParaOPlayer();

            if (distanciaAtePlayer <= distanciaAtaque)
            {
                //Ataque normal se estiver próximo
                playerNaAreaDeAtaque = true;
                Ataque();
            }
            else if (distanciaAtePlayer <= distanciaDeteccao)
            {
                //Player fora do alcance
                playerNaAreaDeAtaque = false;
                ultimaPosicaoConhecida = player.position;
                MoverAteOPlayer();
            }
            else if (!playerNaAreaDeAtaque && podeUsarAtaqueEspecial)
            {
                //Ataque especial se o player estiver fora do alcance
                AtaqueEspecial();
            }
        }
    }

    private void OlhaParaOPlayer()
    {
        // Faz o boss olhar sempre na direção do jogador
        Vector3 direcao = (player.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));

        // Slerp faz a rotação de forma suave
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void MoverAteOPlayer()
    {
        animator.SetBool("Andar", true);
        animator.SetBool("Atirar", false);

        // Move o boss até a última posição conhecida do jogador
        Vector3 direcao = (ultimaPosicaoConhecida - transform.position).normalized;

        transform.position += direcao * velocidade * Time.deltaTime;
    }

    private void Ataque()
    {
        animator.SetBool("Andar", false);
        StartCoroutine(AtaqueNormalAtivado());
    }

    IEnumerator AtaqueNormalAtivado()
    {
        yield return new WaitForSeconds(3.0f);

        animator.SetTrigger("Ataque");

        StopAllCoroutines();
    }

    private void AtaqueEspecial()
    {
        podeUsarAtaqueEspecial = false;
        ataqueEspecialAtivo = true;

        animator.SetBool("Andar", false);
        animator.SetBool("Atirar", true);


        // Ataque especial
        StartCoroutine(MovimentoAtaqueEspecial());
    }

    IEnumerator MovimentoAtaqueEspecial()
    {
        yield return new WaitForSeconds(tempoAntesDoEspecial);

        while (Vector3.Distance(transform.position, ultimaPosicaoConhecida) > 0.1f)
        {
            Vector3 direcao = (ultimaPosicaoConhecida - transform.position).normalized;
            transform.position += direcao * velocidadeEspecial * Time.deltaTime;
            yield return null; // Espera um frame
        }

        ataqueEspecialAtivo = false;
        animator.SetBool("Atirar", false);

        StartCoroutine(RecarregarAtaqueEspecial());
    }

   IEnumerator RecarregarAtaqueEspecial()
    {
        yield return new WaitForSeconds(recargaAtaqueEspecial);

        podeUsarAtaqueEspecial = true;
    }
}
