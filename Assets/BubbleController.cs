using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX; // N'oubliez pas d'importer Visual Effect Graph

public class BubbleController : MonoBehaviour
{
    // ====== Param�tres de mouvement et d'animation ======
    public float initialSpeed = 3f; // Vitesse de d�placement initiale de la bulle (quand elle n'a pas captur�)
    public float lifetime = 5f; // Dur�e de vie de la bulle avant qu'elle n'�clate (si rien n'est captur�)

    // Param�tres pour l'animation post-capture
    public float verticalAscentSpeed = 2f; // Vitesse � laquelle la bulle monte apr�s capture
    public float oscillationSpeed = 5f;    // Vitesse de l'oscillation (plus grand = plus rapide)
    public float oscillationAmplitude = 0.1f; // Amplitude de l'oscillation (plus grand = plus de mouvement lat�ral)

    // ====== Variables internes ======
    private float currentDirection; // Direction horizontale (1 pour droite, -1 pour gauche)
    private bool captured = false; // Indique si un ennemi a �t� captur�
    private float initialXPosition; // Position X de la bulle au moment de la capture, pour l'oscillation

    // ====== R�f�rences aux composants externes ======
    public VisualEffect visualEffect; // Le VFX � jouer (par exemple, un effet de 'pop')
    public AudioSource PlayerAudioSource; // La source audio du joueur pour jouer les SFX

    [Header("Effets Sonores")]
    public List<Audios> SFXs; // Liste des clips audio

    // ==== M�thodes Unity ====

    void Start()
    {
        // D�finir la direction initiale de la bulle en fonction de la direction du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Supposons que l'�chelle X du joueur indique sa direction (positif pour droite, n�gatif pour gauche)
            currentDirection = player.transform.localScale.x > 0 ? 1f : -1f;

            // D�caler la bulle l�g�rement de la position du joueur au d�part
            // Important : Utilisez transform.position directement pour d�finir la position
            transform.position = new Vector2(player.transform.position.x + (currentDirection * 0.5f), player.transform.position.y);
        }
        else
        {
            Debug.LogWarning("Joueur non trouv� avec le tag 'Player'. La bulle d�marrera sans direction sp�cifique.");
            currentDirection = 1f; // Direction par d�faut
        }
    }

    void Update()
    {
        if (!captured)
        {
            // Mouvement horizontal de la bulle avant capture
            transform.Translate(Vector2.right * currentDirection * initialSpeed * Time.deltaTime);

            // G�rer la dur�e de vie de la bulle avant capture
            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
            {
                OnBubbleHit(); // La bulle �clate si elle n'a rien captur� � temps
            }
        }
        else // Si un ennemi est captur�
        {
            // 1. Mouvement vertical (mont�e)
            float newY = transform.position.y + verticalAscentSpeed * Time.deltaTime;

            // 2. Oscillation horizontale autour de la position X initiale au moment de la capture
            // Mathf.Sin(Time.time * oscillationSpeed) cr�e une valeur entre -1 et 1 qui change avec le temps
            float oscillationOffset = Mathf.Sin(Time.time * oscillationSpeed) * oscillationAmplitude;
            float oscillationX = initialXPosition + oscillationOffset;

            // 3. Appliquer la nouvelle position combin�e
            transform.position = new Vector2(oscillationX, newY);

            // S'assure que l'ennemi captur� reste bien au centre de la bulle
            UpdateCapturedEnemyPosition();
        }
    }

    // Cette m�thode peut �tre appel�e si vous voulez changer la direction de la bulle apr�s la cr�ation
    public void SetDirection(float dir)
    {
        currentDirection = dir;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision avec : " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.transform.SetParent(this.transform);
            collision.transform.localPosition = Vector3.zero;
            captured = true;
            initialXPosition = this.transform.position.x;
            var enemyCollider = collision.gameObject.GetComponent<Collider2D>();
            if (enemyCollider != null) enemyCollider.enabled = false;

            GameManager.instance.AddScore(100);
            OnBubbleHit(); 
        }
        else if (!collision.gameObject.CompareTag("Player")) 
        {
            OnBubbleHit(); 
        }
        else 
        {
            FindInAudiosAndPlay("Bounce"); 
        }
    }

    // ==== Fonctions utilitaires ====
    private void FindInAudiosAndPlay(string name)
    {
        var audio = SFXs.Find(sfx => sfx.name == name);
        if (audio.clip != null)
        {
            PlayerAudioSource.clip = audio.clip;
            PlayerAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Clip audio '{name}' non trouv� ou nul dans la liste SFXs.");
        }
    }

    private void OnBubbleHit()
    {
        if (visualEffect != null)
        {
            visualEffect.Play();
        }
        FindInAudiosAndPlay("Pop"); 
        Destroy(gameObject, 0.25f);
    }

    [System.Serializable]
    public struct Audios
    {
        public string name;
        public AudioClip clip;
    }

    private void UpdateCapturedEnemyPosition()
    {
        if (captured && transform.childCount > 0)
        {
            Transform capturedEnemy = transform.GetChild(0);
            capturedEnemy.localPosition = Vector3.zero;
        }
    }

}