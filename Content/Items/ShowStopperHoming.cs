using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class ShowStopperHoming : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Modified Showstopper");
			Tooltip.SetDefault(
				"You suck at aiming the original launcher now you seek the easier way to hit the enemies.\n" +
				"You now use magic to guide your horrendous potato aim.");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}
		 
		public override void SetDefaults()
		{
			item.damage = 280;
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
			item.value = Item.buyPrice(0, 0, 69, 0);
			item.rare = 6;
			item.UseSound = SoundID.Item34;
			item.shoot = 340; // 134=rocket, 167-170=fireworks, 338-341=rocketsnowman
			item.shootSpeed = 14f;
			//item.useAmmo = AmmoID.Rocket;
			item.crit = 4;
			item.glowMask = customGlowMask;
			item.mana = 3;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;
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
			recipe.AddIngredient(null, "ShowStopper", 1);
			recipe.AddIngredient(ItemID.SoulofSight, 24);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}