using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SacredScriptures.Content.Items
{
	public class Javeline : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Javeline");
			Tooltip.SetDefault("Fire a salvos homing rockets, but is it worth the ammunation?");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}
		 
		public override void SetDefaults()
		{
			item.damage = 480;
			item.ranged = true;
			item.melee = false;
			item.magic = false;
			item.scale = 1.3f;
			item.width = 40;
			item.height = 20;
			item.useTime = 17;
			item.useAnimation = 17;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 56, 0, 0);
			item.rare = 9;
			item.UseSound = SoundID.Item34;
			item.shoot = ModContent.ProjectileType<Ammo.SpearWarHead>();
			item.shootSpeed = 14f;
			item.useAmmo = ModContent.ItemType<Ammo.SpearHead>();
			item.crit = 26;
			item.glowMask = customGlowMask;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int numberProjectiles = 1 + Main.rand.Next(3);
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return false;
			

			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 50f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			return true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-27, -5);
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RocketLauncher, 1);
			recipe.AddIngredient(ItemID.ChainGun, 1);
			recipe.AddIngredient(ItemID.ShroomiteBar, 15);
			recipe.AddIngredient(ItemID.SoulofSight, 20);
			recipe.AddIngredient(ItemID.FragmentVortex, 16);
			recipe.AddIngredient(ItemID.FragmentSolar, 12);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
	
	
}