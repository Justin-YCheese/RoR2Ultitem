﻿using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public abstract class BuffBase : MonoBehaviour
    {
        public abstract void Init();

        //protected abstract void Hooks();

        protected static BuffDef DefineBuff(string name, bool canStack, bool isDebuff, Color color, Sprite icon, bool isCooldown, bool isHidden)
        {
            BuffDef definition = ScriptableObject.CreateInstance<BuffDef>();
            definition.name = name;
            definition.canStack = canStack;
            definition.isDebuff = isDebuff;
            definition.buffColor = color;
            definition.iconSprite = icon;
            definition.eliteDef = null;
            definition.isCooldown = isCooldown;
            definition.isHidden = isHidden;

            if (ContentAddition.AddBuffDef(definition))
            {
                //Log.Warning(definition.name + " Buff Initialized");
            };
            return definition;
        }
    }
}
