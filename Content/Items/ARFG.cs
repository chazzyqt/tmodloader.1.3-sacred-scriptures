using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class ARFG : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Rapid Fire Gun");
			Tooltip.SetDefault(
				"66% chance not to consume ammo.\n"+
				"The material of cosmos, the material of all living and non-living crafted into a weapon.\n"+
				"The silent killer of Gods.");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}

		public override void SetDefaults() 
		{
			item.damage = 110;
			item.ranged = true;
			item.melee = false;
			item.magic = false;
			item.scale = 1f;
			item.width = 40;
			item.height = 20;
			item.useTime = 2;
			item.useAnimation = 2;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 0;
			item.value = Item.buyPrice(0, 72, 25, 0);
			item.rare = 9;
			item.UseSound = SoundID.Item15;
			item.autoReuse = true;
			item.shoot = 10;
			item.shootSpeed = 16f;
			item.useAmmo = AmmoID.Bullet;
			item.glowMask = customGlowMask;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, -1);
		}

		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .66f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int numberProjectiles = 1 + Main.rand.Next(0);//3
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(1));
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DawnGun", 1);
            recipe.AddIngredient(ItemID.SDMG, 1);
			recipe.AddIngredient(ItemID.ChainGun, 1);
			recipe.AddIngredient(ItemID.VortexBeater, 1);
            recipe.AddIngredient(ItemID.LunarBar, 26);
			recipe.AddIngredient(ItemID.FragmentVortex, 16);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 25);
			recipe.AddIngredient(ItemID.ShroomiteBar, 22);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DawnGun", 1);
            recipe.AddIngredient(ItemID.SDMG, 1);
			recipe.AddIngredient(ItemID.ChainGun, 1);
			recipe.AddIngredient(ItemID.VortexBeater, 1);
            recipe.AddIngredient(ItemID.LunarBar, 26);
			recipe.AddIngredient(ItemID.FragmentVortex, 10);
			recipe.AddIngredient(ItemID.FragmentSolar, 10);
			recipe.AddIngredient(ItemID.FragmentNebula, 10);
			recipe.AddIngredient(ItemID.FragmentStardust, 10);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 25);
			recipe.AddIngredient(ItemID.ShroomiteBar, 22);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}