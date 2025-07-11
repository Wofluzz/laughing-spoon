Shader "Custom/BiomeTilemapShader"  
{  
    Properties  
    {  
        _MainTex ("Texture (Tilemap Atlas)", 2D) = "white" {}  
        _GrassMask ("Grass Mask (Atlas)", 2D) = "white" {} // Masque pour la partie herbe (blanc = affecté, noir = non)
        _CurrentBiomeColor ("Current Biome Color", Color) = (0, 1, 0, 1) // Couleur du biome actuelle, passée par C#
    }  
    SubShader  
    {  
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }  
        Blend SrcAlpha OneMinusSrcAlpha  
        ZWrite Off  
        LOD 100  

        Pass  
        {  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #include "UnityCG.cginc"  

            // Définir le nombre maximal de sprites que vous pouvez cibler.
            // Assurez-vous que cette valeur correspond à celle du script C#.
            #define MAX_TARGET_SPRITES 20 

            struct appdata  
            {  
                float4 vertex : POSITION;  
                float2 uv : TEXCOORD0;  
            };  

            struct v2f  
            {  
                float2 uv : TEXCOORD0;       // UV pour la texture principale
                float4 vertex : SV_POSITION;
                float2 grassMaskUV : TEXCOORD1; // UV pour la texture de masque de l'herbe
            };  

            sampler2D _MainTex;  
            float4 _MainTex_ST;  
            sampler2D _GrassMask; 
            float4 _GrassMask_ST; // Propriétés de tiling/offset pour le masque
            fixed4 _CurrentBiomeColor; // Couleur du biome actuelle

            // Tableau des coordonnées UV (minX, minY, maxX, maxY) pour chaque sprite cible (herbe)
            // Ces valeurs sont passées par le script C#.
            float4 _TargetSpriteUVs[MAX_TARGET_SPRITES]; 
            int _NumTargetSprites; // Nombre réel de sprites cibles dans le tableau

            v2f vert (appdata v)  
            {
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);      // Applique le tiling/offset pour la texture principale
                o.grassMaskUV = TRANSFORM_TEX(v.uv, _GrassMask); // Applique le tiling/offset pour le masque
                return o;  
            }  

            half4 frag (v2f i) : SV_Target  
            {  
                half4 originalCol = tex2D(_MainTex, i.uv);  

                // Si le pixel de la texture principale est transparent, ne rien rendre  
                if (originalCol.a == 0)  
                {  
                    return half4(0, 0, 0, 0);  
                }  

                bool isGrassTile = false;  

                // Vérifie si le UV actuel du fragment se trouve dans les limites  
                // d'un des sprites d'herbe cibles.  
                for (int j = 0; j < _NumTargetSprites; ++j)  
                {  
                    float4 targetUVBounds = _TargetSpriteUVs[j]; // (minX, minY, maxX, maxY)  

                    // Vérifie si le UV actuel est dans les limites de ce sprite cible  
                    if (i.uv.x >= targetUVBounds.x && i.uv.x <= targetUVBounds.z &&   
                        i.uv.y >= targetUVBounds.y && i.uv.y <= targetUVBounds.w)     
                    {  
                        isGrassTile = true;  
                        break; // Une fois qu'une correspondance est trouvée, on sort de la boucle  
                    }  
                }  

                if (isGrassTile)  
                {  
                    // Obtenir la valeur du masque pour le pixel courant  
                    // Cette valeur est entre 0 (noir) et 1 (blanc)
                    half maskFactor = tex2D(_GrassMask, i.grassMaskUV).r; 

                    // Convertir la couleur originale du sprite cible en niveaux de gris (pour toute la tuile d'herbe)
                    float grayOriginal = dot(originalCol.rgb, float3(0.299, 0.587, 0.114));  
                    half4 grayscaleOriginalCol = half4(grayOriginal, grayOriginal, grayOriginal, originalCol.a);  

                    // Calculer la couleur du biome appliquée sur la version en niveaux de gris
                    // C'est la couleur que l'on veut voir apparaître sur la partie supérieure de l'herbe.
                    half4 biomeColoredGrayscale = half4(_CurrentBiomeColor.rgb * grayscaleOriginalCol.rgb, grayscaleOriginalCol.a);  

                    // Mélanger la couleur originale en niveaux de gris avec la couleur du biome appliquée.
                    // Le maskFactor contrôle ce mélange:
                    // - Si maskFactor est 0 (noir dans le masque, ex: la terre), le résultat est grayscaleOriginalCol.
                    // - Si maskFactor est 1 (blanc dans le masque, ex: le haut de l'herbe), le résultat est biomeColoredGrayscale.
                    // - Si maskFactor est entre 0 et 1 (nuances de gris), il y a un mélange progressif.
                    half4 finalColor = lerp(grayscaleOriginalCol, biomeColoredGrayscale, maskFactor);
                    
                    return finalColor;  
                }  
                else  
                {  
                    // Si ce n'est PAS une tuile d'herbe, retourne la couleur originale sans modification  
                    return originalCol;  
                }  
            }  
            ENDCG  
        }  
    }  
}
