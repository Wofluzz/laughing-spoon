using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX; // N'oubliez pas d'importer Visual Effect Graph

public class BubbleController : MonoBehaviour
{
    // ====== Paramètres de mouvement et d'animation ======
    public float initialSpeed = 3f; // Vitesse de déplacement initiale de la bulle (quand elle n'a pas capturé)
    public float lifetime = 5f; // Durée de vie de la bulle avant qu'elle n'éclate (si rien n'est capturé)

    // Paramètres pour l'animation post-capture
    public float verticalAscentSpeed = 2f; // Vitesse à laquelle la bulle monte après capture
    public float oscillationSpeed = 5f;    // Vitesse de l'oscillation (plus grand = plus rapide)
    public float oscillationAmplitude = 0.1f; // Amplitude de l'oscillation (plus grand = plus de mouvement latéral)

    // ====== Variables internes ======
    private float currentDirection; // Direction horizontale (1 pour droite, -1 pour gauche)
    private bool captured = false; // Indique si un ennemi a été capturé
    private float initialXPosition; // Position X de la bulle au moment de la capture, pour l'oscillation
    [SerializeField]
    private GameObject capturedObject;

    // ====== Références aux composants externes ======
    public VisualEffect visualEffect; // Le VFX à jouer (par exemple, un effet de 'pop')
    public AudioSource PlayerAudioSource; // La source audio du joueur pour jouer les SFX

    [Header("Effets Sonores")]
    public List<Audios> SFXs; // Liste des clips audio

    // ==== Méthodes Unity ====

    void Start()
    {
        // Définir la direction initiale de la bulle en fonction de la direction du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Supposons que l'échelle X du joueur indique sa direction (positif pour droite, négatif pour gauche)
            currentDirection = player.transform.localScale.x > 0 ? 1f : -1f;

            // Décaler la bulle légèrement de la position du joueur au départ
            // Important : Utilisez transform.position directement pour définir la position
            
            if (player.GetComponent<PlayerMovements>().isJumping)
            {
                transform.position = new Vector2(player.transform.position.x + (currentDirection * 0.5f), player.transform.position.y - 1);
            } else
            {
                transform.position = new Vector2(player.transform.position.x + (currentDirection * 0.5f), player.transform.position.y);
            }
        }
        else
        {
            Debug.LogWarning("Joueur non trouvé avec le tag 'Player'. La bulle démarrera sans direction spécifique.");
            currentDirection = 1f; // Direction par défaut
        }
    }

    void Update()
    {
        if (!captured)
        {
            transform.Translate(Vector2.right * currentDirection * initialSpeed * Time.deltaTime);

            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
            {
                OnBubbleHit(); 
            }
        }
        else 
        {
            
            float newY = transform.position.y + verticalAscentSpeed * Time.deltaTime;
            float oscillationOffset = Mathf.Sin(Time.time * oscillationSpeed) * oscillationAmplitude;
            float oscillationX = initialXPosition + oscillationOffset;

            transform.position = new Vector2(oscillationX, newY);

            UpdateCapturedEnemyPosition();
        }

        if (capturedObject)
        {
            if (!capturedObject.activeInHierarchy)
            {
                capturedObject.SetActive(true);
                Debug.Log("L'objet capturé n'était pas actif dans le monde, il a été réactivé.");
            }

            if (capturedObject.CompareTag("Objects"))
                CaptureObjectOrEnemies(CapturedType.Object);
            else if (capturedObject.CompareTag("Enemy"))
                CaptureObjectOrEnemies(CapturedType.Enemy);
            else
                Debug.LogWarning("Pas de type associé à cet objet !");
        }
    }

    public void SetDirection(float dir)
    {
        currentDirection = dir;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision avec : " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            capturedObject = collision.gameObject;
            CaptureObjectOrEnemies(CapturedType.Enemy);
        }
        else if (collision.gameObject.CompareTag("Objects"))
        {
            capturedObject = collision.gameObject;
            CaptureObjectOrEnemies(CapturedType.Enemy);
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

    private void CaptureObjectOrEnemies(CapturedType type)
    {
        switch (type)
        {
            case CapturedType.Object:
                capturedObject.transform.SetParent(this.transform);
                capturedObject.transform.localPosition = Vector3.zero;
                captured = true;
                initialXPosition = this.transform.position.x;
                verticalAscentSpeed = 2f;

                //OnBubbleHit();
                break;
            case CapturedType.Enemy:
                capturedObject.transform.SetParent(this.transform);
                capturedObject.transform.localPosition = Vector3.zero;
                captured = true;
                initialXPosition = this.transform.position.x;
                var enemyCollider = capturedObject.GetComponent<Collider2D>();
                if (enemyCollider != null) enemyCollider.enabled = false;
                verticalAscentSpeed = 2f;

                GameManager.instance.AddScore(100);
                //OnBubbleHit();
                break;
            default:
                break;
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
            Debug.LogWarning($"Clip audio '{name}' non trouvé ou nul dans la liste SFXs.");
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

    private enum CapturedType
    {
        Object,
        Enemy,
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