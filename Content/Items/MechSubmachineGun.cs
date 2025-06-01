using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class MechSubmachineGun : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Mechanized Vanguard");
			Tooltip.SetDefault(
				"40% chance not to consume ammo\n"+
				"Imbued with the souls of the destroyers, the object's rage could not be held within.");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}

		public override void SetDefaults()
		{
			item.damage = 46;
			item.ranged = true;
			item.melee = false;
			item.magic = false;
			item.scale = 1.3f;
			item.width = 40;
			item.height = 20;
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3;
			item.value = Item.buyPrice(0, 22, 50, 0);
			item.rare = 5;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
			item.shoot = 10;
			item.shootSpeed = 12f;
			item.useAmmo = AmmoID.Bullet;
			item.glowMask = customGlowMask;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-17, -2);
		}

		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .40f; 
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
			recipe.AddIngredient(null, "SubmachineGun", 1);
			recipe.AddIngredient(ItemID.Uzi, 1);
			recipe.AddIngredient(ItemID.Megashark, 1);
            recipe.AddIngredient(ItemID.HallowedBar, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 22);
            recipe.AddIngredient(ItemID.SoulofMight, 22);
			
			
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}