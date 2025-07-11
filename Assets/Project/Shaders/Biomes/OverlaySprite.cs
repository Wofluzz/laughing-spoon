using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapOverlayGenerator : MonoBehaviour
{
    // --- Nouvelle structure pour définir une règle de superposition ---
    [System.Serializable] // Rend le struct visible dans l'Inspector d'Unity
    public struct OverlayRule
    {
        public TileBase targetedTile; // La tuile de la Tilemap que l'on veut superposer
        public Sprite overlaySprite; // Le sprite à afficher par-dessus
        public Material overlayMaterial; // Le matériau à appliquer au sprite de superposition (optionnel)

        [Tooltip("L'ordre de tri pour ce sprite de superposition. Doit être supérieur à celui de votre TilemapRenderer.")]
        public int overlaySortingOrder; // L'ordre de tri spécifique pour cette règle

        [Tooltip("Le Sorting Layer pour ce sprite de superposition. Laissez vide pour utiliser celui par défaut de la Tilemap.")]
        public string overlaySortingLayerName; // Le Sorting Layer spécifique (optionnel)
    }

    public Tilemap targetTilemap; // Référence à votre Tilemap

    [Tooltip("Définissez ici toutes les règles de superposition (quelle tuile cible, quel sprite, etc.).")]
    public OverlayRule[] overlayRules; // Tableau de règles de superposition

    [Tooltip("Désactiver ce booléen après l'exécution en mode édition pour éviter la génération répétée.")]
    public bool generateOnStart = true;

    [Tooltip("Le nom du GameObject parent où les overlays seront regroupés.")]
    public string parentContainerName = "GeneratedTileOverlays"; // Nom du conteneur par défaut

    private GameObject parentContainer; // Référence au GameObject parent qui contiendra les overlays

    void Start()
    {
        if (generateOnStart)
        {
            GenerateOverlays();
        }
    }

    // Cette fonction peut être appelée manuellement ou au démarrage
    public void GenerateOverlays()
    {
        // Vérifications initiales
        if (targetTilemap == null)
        {
            Debug.LogError("Erreur : La référence à la Tilemap n'est pas définie. Veuillez assigner votre Tilemap dans l'inspecteur.");
            return;
        }

        if (overlayRules == null || overlayRules.Length == 0)
        {
            Debug.LogWarning("Attention : Aucune règle de superposition n'est définie. Veuillez en ajouter dans l'inspecteur.");
            return;
        }

        // --- Gérer le GameObject parent pour les overlays ---
        // Chercher un conteneur existant comme enfant de ce GameObject
        parentContainer = transform.Find(parentContainerName)?.gameObject;

        if (parentContainer == null)
        {
            // Si le conteneur n'existe pas, le créer
            parentContainer = new GameObject(parentContainerName);
            parentContainer.transform.parent = this.transform; // Rendre le conteneur enfant de ce script
            parentContainer.transform.localPosition = Vector3.zero; // Le placer à la même position locale
            Debug.Log($"Création du conteneur parent pour les overlays : '{parentContainerName}'.");
        }
        else
        {
            // Si le conteneur existe, supprimer tous ses enfants (les anciens overlays)
            // Utilise une boucle inversée pour éviter les problèmes d'itération lors de la suppression
            for (int i = parentContainer.transform.childCount - 1; i >= 0; i--)
            {
                // DestroyImmediate est nécessaire si appelé en mode édition via [ContextMenu]
                DestroyImmediate(parentContainer.transform.GetChild(i).gameObject);
            }
            Debug.Log($"Anciens overlays supprimés du conteneur '{parentContainerName}'.");
        }

        // Parcourir toutes les positions de tuiles dans les limites de la Tilemap
        foreach (Vector3Int position in targetTilemap.cellBounds.allPositionsWithin)
        {
            TileBase currentTile = targetTilemap.GetTile(position);

            // Parcourir toutes les règles de superposition définies
            foreach (OverlayRule rule in overlayRules)
            {
                // Si la tuile actuelle correspond à la tuile ciblée par cette règle
                if (currentTile == rule.targetedTile)
                {
                    // Vérifier si le sprite de superposition est bien assigné pour cette règle
                    if (rule.overlaySprite == null)
                    {
                        Debug.LogWarning($"Avertissement : Le 'Overlay Sprite' est manquant pour la tuile cible '{rule.targetedTile?.name}' à la position {position}. Skipping.");
                        continue; // Passer à la règle suivante ou à la tuile suivante
                    }

                    // Créer un nouveau GameObject pour le sprite de superposition
                    GameObject overlayGameObject = new GameObject($"Overlay_{rule.targetedTile?.name}_{position.x}_{position.y}");

                    // Positionner le GameObject au centre de la cellule de la tuile dans le monde
                    overlayGameObject.transform.position = targetTilemap.GetCellCenterWorld(position);

                    // Rendre ce GameObject enfant du conteneur parent
                    overlayGameObject.transform.parent = parentContainer.transform;

                    // Ajouter et configurer le SpriteRenderer
                    SpriteRenderer spriteRenderer = overlayGameObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = rule.overlaySprite; // Assigner le sprite de la règle

                    // Appliquer l'ordre de tri et le matériau de la règle
                    spriteRenderer.sortingOrder = rule.overlaySortingOrder;

                    if (rule.overlayMaterial != null)
                    {
                        spriteRenderer.material = rule.overlayMaterial;
                    }

                    // Appliquer le Sorting Layer si spécifié dans la règle
                    if (!string.IsNullOrEmpty(rule.overlaySortingLayerName))
                    {
                        spriteRenderer.sortingLayerName = rule.overlaySortingLayerName;
                    }

                    // Une fois qu'une règle est appliquée à une tuile, on peut passer à la tuile suivante
                    // si on ne veut qu'une seule superposition par tuile.
                    // Si vous voulez que plusieurs règles puissent s'appliquer à la même tuile, retirez le 'break;'.
                    break;
                }
            }
        }

        if (Application.isEditor)
        {
            Debug.Log("Génération des overlays terminée. Vous pouvez décocher 'Generate On Start' si vous avez généré en mode édition.");
            // generateOnStart = false; // Optionnel : désactiver automatiquement après la génération en édition
        }
    }

    // Un bouton dans l'inspecteur pour déclencher la génération manuellement
    [ContextMenu("Generate Tile Overlays")]
    void GenerateTileOverlaysFromMenu()
    {
        GenerateOverlays();
    }
}
