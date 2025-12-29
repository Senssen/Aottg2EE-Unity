using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    class PhysicsLayer
    {
        public static int Water = 4; //added by Sysyfus Oct 6 2025
        public static int UI = 5;
        public static int NoCollision = 8;
        public static int Hitbox = 9;
        public static int Human = 10;
        public static int TitanMovebox = 11;
        public static int TitanPushbox = 12;
        public static int Hurtbox = 13;
        public static int Projectile = 14;
        public static int ProjectileDetection = 15;
        public static int CharacterDetection = 16;
        public static int NPC = 17;
        public static int MapObjectIgnoreCamera = 19;
        public static int MapObjectMapObjects = 20;
        public static int MapObjectProjectiles = 21;
        public static int MapObjectCharacters = 22;
        public static int MapObjectEntities = 23;
        public static int MapObjectAll = 24;
        public static int MapEditorObject = 25;
        public static int MapEditorGizmo = 26;
        public static int MinimapIcon = 27;
        public static int Background = 28;
        public static int MapObjectTitans = 29;
        public static int MapObjectHumans = 30;
        public static int WagonHurtbox = 31;
        private static Dictionary<int, LayerMask> _masks = new Dictionary<int, LayerMask>();

        public static void Init()
        {
            SetLayerCollisions(NoCollision, new int[0]);
            SetLayerCollisions(Hitbox, new int[] { Human, TitanPushbox, Hurtbox, WagonHurtbox });
            SetLayerCollisions(Human, new int[] { Hitbox, TitanPushbox, Projectile, CharacterDetection, 
                MapObjectAll, MapObjectEntities, MapObjectCharacters, MapObjectHumans, MapObjectIgnoreCamera});
            SetLayerCollisions(TitanMovebox, new int[] { TitanMovebox, CharacterDetection, MapObjectAll, MapObjectEntities, MapObjectCharacters, MapObjectTitans, WagonHurtbox, MapObjectIgnoreCamera });
            SetLayerCollisions(TitanPushbox, new int[] { Hitbox, Human, Projectile, NPC, WagonHurtbox });
            SetLayerCollisions(Projectile, new int[] { Human, TitanPushbox, ProjectileDetection, MapObjectEntities, MapObjectAll, MapObjectProjectiles, MapObjectIgnoreCamera });
            SetLayerCollisions(ProjectileDetection, new int[] { Projectile });
            SetLayerCollisions(CharacterDetection, new int[] { Human, TitanMovebox });
            SetLayerCollisions(NPC, new int[] {TitanPushbox, MapObjectCharacters, MapObjectEntities, MapObjectAll, MapObjectIgnoreCamera});
            SetLayerCollisions(Hurtbox, new int[] { Hitbox });
            SetLayerCollisions(MapObjectMapObjects, new int[] { MapObjectAll, MapObjectMapObjects, MapObjectProjectiles, MapObjectTitans, MapObjectHumans, MapObjectCharacters, MapObjectEntities, MapObjectIgnoreCamera });
            SetLayerCollisions(MapObjectProjectiles, new int[] { MapObjectAll, MapObjectMapObjects, Projectile, MapObjectIgnoreCamera });
            SetLayerCollisions(MapObjectTitans, new int[] { MapObjectAll, MapObjectMapObjects, TitanMovebox, MapObjectIgnoreCamera });
            SetLayerCollisions(MapObjectHumans, new int[] { MapObjectAll, MapObjectMapObjects, Human, MapObjectIgnoreCamera });
            SetLayerCollisions(MapObjectCharacters, new int[] { MapObjectAll, MapObjectMapObjects, Human, TitanMovebox, NPC, MapObjectIgnoreCamera });
            SetLayerCollisions(MapObjectEntities, new int[] { MapObjectAll, MapObjectMapObjects, TitanMovebox, Human, Projectile, NPC, MapObjectIgnoreCamera });
            SetLayerCollisions(MapObjectAll, new int[] { Human, TitanMovebox, Projectile, MapObjectAll, MapObjectTitans, MapObjectHumans, MapObjectMapObjects, MapObjectEntities, 
                MapObjectCharacters, MapObjectProjectiles, NPC, MapObjectIgnoreCamera});
            SetLayerCollisions(MapEditorObject, new int[0]);
            SetLayerCollisions(MapEditorGizmo, new int[0]);
            SetLayerCollisions(MinimapIcon, new int[0]);
            SetLayerCollisions(Background, new int[0]);
            SetLayerCollisions(Water, new int[] { Human, TitanPushbox, Projectile, NPC, WagonHurtbox, MapObjectCharacters }); //added by Sysyfus Oct 6 2025
        }

        public static LayerMask GetMask(params int[] layers)
        {
            if (layers.Length == 0)
                return 0;
            LayerMask layerMask = 1 << layers[0];
            for (int i = 1; i < layers.Length; i++)
                layerMask = layerMask | (1 <<layers[i]);
            return layerMask;
        }

        public static LayerMask CopyMask(int originLayer)
        {
            return _masks[originLayer];
        }

        // This method sets the layer on "layer" parameter to ignore collisiosn with every available layer expect for the layers inside the "others" array
        private static void SetLayerCollisions(int layer, int[] others)
        {
            for (int i = 0; i < 32; i++)
                Physics.IgnoreLayerCollision(layer, i, true);
            foreach (int i in others)
                Physics.IgnoreLayerCollision(layer, i, false);
            _masks.Add(layer, GetMask(others));
        }
    }
}
