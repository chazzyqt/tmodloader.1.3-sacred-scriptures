using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
    public class DevStaff : ModItem
    {
		public static short customGlowMask = 0;
        public const int baseDamage = 25000;
        public override void SetStaticDefaults()
        {
            //Came from WeaponOut Mod, be sure to check them out!
            //Upgraded version of Megumin's Staff or Staff of Explosion
            DisplayName.SetDefault("Absolute Developer Staff");
            Tooltip.SetDefault(
                "Why do you have this item?\n" +
                "Unobtainable in normal playthrough.\n" + 
                "Developer Item.");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
        }

        public override void SetDefaults()
        {
            item.damage = baseDamage;
			item.ranged = false;
			item.melee = false;
			item.magic = true;
            item.channel = true;
            Item.staff[item.type] = true;
            item.mana = 1;
			item.scale = 1f;
			item.width = 52;
			item.height = 14;
			item.useTime = 60;
			item.useAnimation = 60;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3f;
			item.value = Item.buyPrice(0, 0, 0, 10);
			item.rare = -12;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<Projectiles.AbsoluteExplosion>();
			item.shootSpeed = 1;
            item.crit = 44;
			item.glowMask = customGlowMask;
        }

        public override void UseStyle(Player player)
        {
            SacredScriptures.modifyPlayerItemLocation(player, -4, -5);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            return true;
        }
    }
}