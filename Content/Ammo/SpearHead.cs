using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SacredScriptures.Content.Ammo
{
	public class SpearHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Looks fragile doesn't it?");
		}

		public override void SetDefaults()
		{
			item.damage = 20;
			item.ranged = true;
			item.width = 14;
			item.height = 14;
			item.knockBack = 2f;
			item.value = Item.buyPrice(0, 0, 2, 0);
			item.rare = 9;
			item.shoot = ModContent.ProjectileType<SpearWarHead>();
			item.maxStack = 999;
			item.consumable = true;
			item.ammo = item.type;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RocketI, 50);
			recipe.AddIngredient(ItemID.FragmentNebula, 18);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();
			
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RocketII, 50);
			recipe.AddIngredient(ItemID.FragmentNebula, 18);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RocketIII, 50);
			recipe.AddIngredient(ItemID.FragmentNebula, 18);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RocketIV, 50);
			recipe.AddIngredient(ItemID.FragmentNebula, 18);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();
        }
	}

	public class SpearWarHead : ModProjectile
	{

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Javeline's Warhead");
		}

		public override void SetDefaults() {
			projectile.CloneDefaults(ProjectileID.RocketSnowmanI);
			aiType = ProjectileID.RocketSnowmanI;
			projectile.tileCollide = true;
			projectile.penetrate = 1;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.width = 15;
			projectile.height = 15;
			projectile.timeLeft = 300;
            projectile.damage = 480;
		}

		public override bool PreKill(int timeLeft)
		{
			projectile.type = ProjectileID.RocketSnowmanI;
			return true;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.position, 0.6f, 0.6f, 0.6f);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
        {
			projectile.Kill();
			return false;
        }
		
		public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}