using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class TealZeal : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Teal-Zeal Flatline");
			Tooltip.SetDefault(
				"60% chance not to consume ammo.\n"+
				"Hand crafted by the mythical God of Youtube Zzolven.");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}

		public override void SetDefaults() 
		{
			item.damage = 62;
			item.ranged = true;
			item.melee = false;
			item.magic = false;
			item.scale = 1f;
			item.width = 40;
			item.height = 20;
			item.useTime = 7;
			item.useAnimation = 7;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 6;
			item.value = Item.buyPrice(0, 10, 12, 0);
			item.rare = 7;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
			item.shoot = 10;
			item.shootSpeed = 16f;
			item.useAmmo = AmmoID.Bullet;
			item.crit = 26;
			item.glowMask = customGlowMask;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-17, 6);
		}

		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .60f; 
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int numberProjectiles = 1 + Main.rand.Next(1);
				for (int i = 0; i < numberProjectiles; i++)
				{
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(2));
					Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
				}
			return false;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "MechSubmachineGun", 1);
			recipe.AddIngredient(ItemID.Megashark, 1);
			recipe.AddIngredient(ItemID.HallowedBar, 40);
			recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}