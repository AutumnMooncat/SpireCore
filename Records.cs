using System;
using System.Collections;
using System.Collections.Generic;
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

    public abstract record Requirement
    {
        public bool not;
        public abstract bool Test(State s, StoryVars vars, StorySearch ctx);
    }

    public record OrPayload : Requirement
    {
        public required List<Requirement> reqs;
        
        public override bool Test(State s, StoryVars vars, StorySearch ctx)
        {
            return reqs.Any(r => r.Test(s, vars, ctx)) != not;
        }
    }
    
    public record AndPayload : Requirement
    {
        public required List<Requirement> reqs;
        
        public override bool Test(State s, StoryVars vars, StorySearch ctx)
        {
            return reqs.All(r => r.Test(s, vars, ctx)) != not;
        }
    }

    public record AtLeastOnePresentPayload : Requirement
    {
        public required HashSet<string> chars;

        public override bool Test(State s, StoryVars vars, StorySearch ctx)
        {
            return chars.Fast_Overlap(ctx.charactersConsideredPresent) != not;
        }
    }

    public record StoryDataPayload : Requirement
    {
        public required string key;
        public required object value;
        public bool atLeast;
        public bool atMost;
        public bool contains;

        public override bool Test(State s, StoryVars vars, StorySearch ctx)
        {
            return DataLookup(vars) != not;
        }

        protected bool DataLookup(StoryVars vars)
        {
            if (!vars.GetData(key, out object data)) return value == null;
            if (value == null) return false;
            if (data is IComparable a && value is IComparable b)
            {
                if (atLeast)
                {
                    return a.CompareTo(b) >= 0;
                }

                if (atMost)
                {
                    return a.CompareTo(b) <= 0;
                }
            }

            if (contains && data is ICollection col && col.Cast<object>().Contains(value))
            {
                return true;
            }

            return data.Equals(value);
        }
    }
}