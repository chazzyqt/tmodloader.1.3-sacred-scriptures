using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class ShowStopper : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Showstopper");
			Tooltip.SetDefault(
				"Here comes the party!\n" + 
				"-Some guy named Roza");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}
		 
		public override void SetDefaults()
		{
			item.damage = 310;
			item.ranged = true;
			item.melee = false;
			item.magic = false;
			item.scale = 1f;
			item.width = 40;
			item.height = 20;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 35, 0, 0);
			item.rare = 8;
			item.UseSound = SoundID.Item34;
			item.shoot = 134;
			item.shootSpeed = 14f;
			item.useAmmo = AmmoID.Rocket;
			item.crit = 14;
			item.glowMask = customGlowMask;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 50f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			return true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-42, -5);
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RocketLauncher, 1);
			recipe.AddIngredient(ItemID.SoulofMight, 20);
			recipe.AddIngredient(ItemID.ShroomiteBar, 36);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ShowStopperHoming", 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}