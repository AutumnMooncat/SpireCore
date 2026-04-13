using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Artifacts.Ship;
using Nickel;

namespace AutumnMooncat.SpireCore.Ships;

[IRegisterable.Ignore]
public class DemoShip : IRShip
{
    public static string ID => nameof(DemoShip);
    public static IShipEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        var demoShipPartWing = helper.Content.Ships.RegisterPart("DemoPart.Wing", new PartConfiguration()
        {
            Sprite = MainModFile.MakeSprite("assets/ships/demowing.png").Sprite
        });
        var demoShipPartCannon = helper.Content.Ships.RegisterPart("DemoPart.Cannon", new PartConfiguration()
        {
            Sprite = MainModFile.MakeSprite("assets/ships/democannon.png").Sprite
        });
        var demoShipPartMissiles = helper.Content.Ships.RegisterPart("DemoPart.Missiles", new PartConfiguration()
        {
            Sprite = MainModFile.MakeSprite("assets/ships/demomissiles.png").Sprite
        });
        var demoShipPartCockpit = helper.Content.Ships.RegisterPart("DemoPart.Cockpit", new PartConfiguration()
        {
            Sprite = MainModFile.MakeSprite("assets/ships/democockpit.png").Sprite
        });
        var demoShipSpriteChassis = MainModFile.MakeSprite("assets/ships/demochassis.png").Sprite;
        
        Entry = helper.Content.Ships.RegisterShip(ID, new ShipConfiguration()
        {
            Ship = new StarterShip()
            {
                ship = new Ship()
                {
                    /* This is how much hull the ship will start a run with. We recommend matching hullMax */
                    hull = 12,
                    hullMax = 12,
                    shieldMaxBase = 4,
                    parts =
                    {
                        /* This is the order in which the ship parts will be arranged in-game, from left to right. Part1 -> Part2 -> Part3 */
                        new Part
                        {
                            type = PType.wing,
                            skin = demoShipPartWing.UniqueName
                        },
                        new Part
                        {
                            type = PType.cannon,
                            skin = demoShipPartCannon.UniqueName,
                            damageModifier = PDamMod.armor
                        },
                        new Part
                        {
                            type = PType.missiles,
                            skin = demoShipPartMissiles.UniqueName,
                            damageModifier = PDamMod.weak
                        },
                        new Part
                        {
                            type = PType.cockpit,
                            skin = demoShipPartCockpit.UniqueName
                        },
                        new Part
                        {
                            type = PType.wing,
                            skin = demoShipPartWing.UniqueName,
                            flip = true
                        }
                    }
                },

                /* These are cards and artifacts the ship will start a run with. The recommended card amount is 4, and the recommended artifact amount is 2 to 3 */
                cards =
                {
                    new CannonColorless(),
                    new DodgeColorless()
                    {
                        upgrade = Upgrade.A,
                    },
                    new DodgeColorless()
                    {
                        upgrade = Upgrade.B,
                    },
                    new BasicShieldColorless(),
                },
                artifacts =
                {
                    new ShieldPrep(),
                    new DemoArtifactCounting()
                }
            },
            ExclusiveArtifactTypes = new HashSet<Type>()
            {
                /* If you make some artifacts that you want only this ship to encounter in a run, here is where you place them */
                typeof(DemoArtifactCounting)
            },

            UnderChassisSprite = demoShipSpriteChassis,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["ship", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["ship", ID, "description"]).Localize
        });
    }
}