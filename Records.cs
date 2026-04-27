using System;
using System.Collections;
using System.Linq;
using AutumnMooncat.SpireCore.Features.Dialogue;

namespace AutumnMooncat.SpireCore;

public class Records
{
    public record RenderPayload
    {
        public Spr? spr;
        public int? amount;
        public int? xHint;
        public Color? color;
        public int width = 8;
        public int dx;
        public int dy;
        public bool flipX;
        public bool flipY;
    }

    public record TexturePayload
    {
        public Spr spr;
        public int x;
        public int y;
        public Color? color;
        public bool flipX;
        public bool flipY;
    }

    public record OnExecutePayload
    {
        public enum Lifetime
        {
            Persistent,
            Run,
            Combat,
            Turn
        }
        
        public required string key;
        public required object value;
        public required Lifetime lifetime;
        public bool add;

        public void Apply(StoryVars vars)
        {
            var toSet = value;
            if (add && vars.GetData(key, out object data) && data.TryAdd(value, out var result))
            {
                toSet = result;
            }
            switch (lifetime)
            {
                case Lifetime.Persistent:
                    vars.AddPersistentData(key, toSet);
                    break;
                case Lifetime.Run:
                    vars.AddRunData(key, toSet);
                    break;
                case Lifetime.Combat:
                    vars.AddCombatData(key, toSet);
                    break;
                case Lifetime.Turn:
                    vars.AddTurnData(key, toSet);
                    break;
            }
        }
    }

    public record RequirementPayload
    {
        public required string key;
        public required object value;
        public bool? atLeast;
        public bool? atMost;
        public bool? contains;

        public bool Test(object o)
        {
            if (!o.GetData(key, out object data)) return false;
            if (data is IComparable a && value is IComparable b)
            {
                if (atLeast is true)
                {
                    return a.CompareTo(b) >= 0;
                }

                if (atMost is true)
                {
                    return a.CompareTo(b) <= 0;
                }
            }

            if (contains is true && data is ICollection col && col.Cast<object>().Contains(value))
            {
                return true;
            }

            return data.Equals(value);
        }
    }
}