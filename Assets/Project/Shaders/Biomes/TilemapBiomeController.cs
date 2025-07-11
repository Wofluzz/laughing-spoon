// Fichier: Assets/Scripts/TilemapBiomeController.cs

using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System; // Nécessaire pour [Serializable]

[ExecuteInEditMode] // Permet au script de s'exécuter dans l'éditeur Unity
public class TilemapBiomeController : MonoBehaviour
{
    // --- Définition de l'énumération des biomes ---
    public enum BiomeType
    {
        Plains = 0,    // Plaines (ex: vert clair)
        Forest = 1,    // Forêt (ex: vert foncé)
        Desert = 2,    // Désert (ex: orange/jaune)
        Snow = 3,      // Neige (ex: blanc/bleu clair)
        // Ajoutez d'autres biomes ici si nécessaire
    }

    // --- Classe pour associer un type de biome à une couleur ---
    [Serializable]
    public class BiomeColorSetting
    {
        public BiomeType biomeType;
        public Color biomeColor;
    }

    [Header("Configuration Générale")]
    [Tooltip("Référence au TilemapRenderer qui utilise le shader de biome.")]
    public Renderer targetRenderer;

    [Tooltip("Le matériau qui utilise le shader de biome. Doit être une instance de matériau, pas l'asset.")]
    public Material biomeMaterialInstance; // IMPORTANT: Doit être une instance de matériau pour modifier ses propriétés

    [Tooltip("La texture de masque (noir et blanc) qui définit la partie modifiable des tuiles d'herbe. Doit être de la même taille que l'atlas de tuiles principal.")]
    public Texture2D grassMaskTexture;

    [Tooltip("Liste des sprites de tuiles d'herbe que le shader doit modifier. Faites glisser vos assets Sprite ici.")]
    public List<Sprite> grassSpritesToModify;

    [Header("Paramètres des Biomes")]
    [Tooltip("Le type de biome actuellement actif. Changez-le pour voir l'effet.")]
    public BiomeType currentActiveBiome = BiomeType.Plains;

    [Tooltip("Liste des couleurs pour chaque type de biome.")]
    public List<BiomeColorSetting> biomeColorSettings;

    // Constantes pour les noms des propriétés du shader
    private const int MAX_TARGET_SPRITES = 20; // Doit correspondre à la valeur dans le shader !
    private const string SHADER_CURRENT_BIOME_COLOR_NAME = "_CurrentBiomeColor";
    private const string SHADER_GRASS_MASK_NAME = "_GrassMask";
    private const string SHADER_TARGET_UVS_ARRAY_NAME = "_TargetSpriteUVs";
    private const string SHADER_NUM_TARGET_SPRITES_NAME = "_NumTargetSprites";

    void Start()
    {
        // Vérifications initiales
        if (targetRenderer == null)
        {
            Debug.LogError("TilemapRenderer non assigné ! Veuillez assigner le TilemapRenderer dans l'inspecteur.");
            return;
        }
        if (biomeMaterialInstance == null)
        {
            biomeMaterialInstance = targetRenderer.material;
            if (biomeMaterialInstance == null)
            {
                Debug.LogError("Matériau du biome non assigné et non trouvé sur le TilemapRenderer !");
                return;
            }
        }
        if (grassMaskTexture == null)
        {
            Debug.LogWarning("Grass Mask Texture non assignée. Le shader pourrait ne pas fonctionner comme prévu pour les tuiles d'herbe.");
        }

        // Appelle la fonction pour mettre à jour le shader au démarrage
        UpdateShaderProperties();
    }

    // Appelé à chaque frame en mode édition et en jeu si [ExecuteInEditMode]
    void Update()
    {
        // Met à jour les propriétés du shader si quelque chose a changé
        // (Peut être optimisé pour ne se déclencher qu'en cas de changement réel)
        UpdateShaderProperties();
    }

    // Cette fonction met à jour toutes les propriétés du shader
    public void UpdateShaderProperties()
    {
        if (biomeMaterialInstance == null) return;

        // 1. Mettre à jour la texture de masque d'herbe
        if (grassMaskTexture != null)
        {
            biomeMaterialInstance.SetTexture(SHADER_GRASS_MASK_NAME, grassMaskTexture);
        }
        else
        {
            biomeMaterialInstance.SetTexture(SHADER_GRASS_MASK_NAME, Texture2D.whiteTexture);
        }

        // 2. Mettre à jour la couleur du biome actuelle
        Color colorToApply = Color.white; // Couleur par défaut si non trouvée
        foreach (var setting in biomeColorSettings)
        {
            if (setting.biomeType == currentActiveBiome)
            {
                colorToApply = setting.biomeColor;
                break;
            }
        }
        biomeMaterialInstance.SetColor(SHADER_CURRENT_BIOME_COLOR_NAME, colorToApply);

        // 3. Récupérer et passer les coordonnées UV des sprites d'herbe modifiables
        List<Vector4> targetUVs = new List<Vector4>();
        foreach (Sprite sprite in grassSpritesToModify)
        {
            if (sprite != null)
            {
                Texture2D spriteAtlasTexture = sprite.texture;
                if (spriteAtlasTexture == null)
                {
                    Debug.LogWarning($"Sprite '{sprite.name}' n'a pas de texture valide. Ignoré.");
                    continue;
                }

                Rect pixelRect = sprite.textureRect;
                float minX = pixelRect.x / spriteAtlasTexture.width;
                float minY = pixelRect.y / spriteAtlasTexture.height;
                float maxX = (pixelRect.x + pixelRect.width) / spriteAtlasTexture.width;
                float maxY = (pixelRect.y + pixelRect.height) / spriteAtlasTexture.height;

                Vector4 uvBounds = new Vector4(minX, minY, maxX, maxY);
                targetUVs.Add(uvBounds);
            }
        }

        if (targetUVs.Count > MAX_TARGET_SPRITES)
        {
            Debug.LogWarning($"Trop de sprites cibles ({targetUVs.Count}) pour le shader (max: {MAX_TARGET_SPRITES}). Seuls les premiers {MAX_TARGET_SPRITES} seront utilisés.");
            targetUVs.RemoveRange(MAX_TARGET_SPRITES, targetUVs.Count - MAX_TARGET_SPRITES);
        }

        biomeMaterialInstance.SetVectorArray(SHADER_TARGET_UVS_ARRAY_NAME, targetUVs.ToArray());
        biomeMaterialInstance.SetInt(SHADER_NUM_TARGET_SPRITES_NAME, targetUVs.Count);

        // Debug.Log($"Shader mis à jour pour le biome : {currentActiveBiome} avec {targetUVs.Count} sprites d'herbe cibles.");
    }

    // Méthode pour mettre à jour les propriétés directement depuis l'éditeur via le menu contextuel
    [ContextMenu("Mettre à jour les propriétés du shader maintenant")]
    void EditorUpdateShaderProperties()
    {
        UpdateShaderProperties();
    }
}
