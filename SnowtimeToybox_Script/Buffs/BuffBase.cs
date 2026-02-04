using R2API;
using RoR2;
using System;
using RoR2.ContentManagement;

namespace SnowtimeToybox.Buffs
{
    public abstract class BuffBase<T> : BuffBase where T : BuffBase<T>
    {
        public static T Instance { get; private set; }
        public static BuffIndex BuffIndex => Instance.Buff.buffIndex;

        public BuffBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBase was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class BuffBase
    {
        public abstract BuffDef Buff { get; }

        public abstract void PostCreation();
        
        public void Create() {
            ContentAddition.AddBuffDef(Buff);
            PostCreation();
        }
    }
}