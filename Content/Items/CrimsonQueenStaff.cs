using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
    public class CrimsonQueenStaff : ModItem
    {
		public static short customGlowMask = 0;
        public const int baseDamage = 155;
        public override void SetStaticDefaults()
        {
            //Came from WeaponOut Mod, be sure to check them out!
            //Upgraded version of Megumin's Staff or Staff of explosion
            DisplayName.SetDefault("Crimson Queen's Staff");
            Tooltip.SetDefault(
                "Obliterate everything within the range of the cursor.\n" +
                "Increase channel speed by standing still.\n" +
                "Enemies are will target you while casting.");
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
            item.mana = 30;
			item.scale = 1f;
			item.width = 52;
			item.height = 14;
			item.useTime = 60;
			item.useAnimation = 60;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3f;
			item.value = Item.buyPrice(1, 17, 50, 0);
			item.rare = 10;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<Projectiles.CrimsonExplosion>();
			item.shootSpeed = 1;
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

        public override void AddRecipes() {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeguminStaff", 1);
            recipe.AddIngredient(ItemID.NebulaArcanum, 1);
            recipe.AddIngredient(ItemID.NebulaBlaze, 1);
            recipe.AddIngredient(ItemID.LastPrism, 1);
			recipe.AddIngredient(ItemID.FragmentSolar, 46);
			recipe.AddIngredient(ItemID.FragmentNebula, 34);
            recipe.AddIngredient(ItemID.LunarBar, 52);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}