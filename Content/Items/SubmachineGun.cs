using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class SubmachineGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Submachine Gun");
			Tooltip.SetDefault(
				"33% chance not to consume ammo.\n"+
				"This gun costs 12,000$ to fire for 12 seconds.");
		}

		public override void SetDefaults()
		{
			item.damage = 28;
			item.ranged = true;
			item.melee = false;
			item.magic = false;
			item.scale = 1.3f;
			item.width = 40;
			item.height = 20;
			item.useTime = 8;
			item.useAnimation = 8;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3;
			item.value = Item.buyPrice(0, 0, 12, 0);
			item.rare = 3;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
			item.shoot = 10;
			item.shootSpeed = 8f;
			item.useAmmo = AmmoID.Bullet;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-12, 1);
		}

		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .33f; 
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int numberProjectiles = 1 + Main.rand.Next(0);//2
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(2));
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return false;

			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			return true;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Minishark, 1);
			recipe.AddIngredient(ItemID.Musket, 1);
            recipe.AddIngredient(ItemID.HellstoneBar, 22);
            recipe.AddIngredient(ItemID.ShadowScale, 24);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 32);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();

			recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Minishark, 1);
			recipe.AddIngredient(ItemID.TheUndertaker, 1);
            recipe.AddIngredient(ItemID.HellstoneBar, 22);
            recipe.AddIngredient(ItemID.TissueSample, 24);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 32);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}