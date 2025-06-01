using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class CarlFrog : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Carl the Frog");
			Tooltip.SetDefault(
				"Uses gel for ammo.\n"+
				"Who ate too many Tacobell!?\nor flies...");
		}

		public override void SetDefaults()
		{
			item.damage = 8;
			item.ranged = true;
			item.melee = false;
			item.magic = false;
			item.scale = 1.4f;
			item.width = 40;
			item.height = 20;
			item.useTime = 4;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1;
			item.value = Item.buyPrice(0, 0, 5, 50);
			item.rare = 3;
			item.UseSound = SoundID.Item34;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<Projectiles.FrogFlame>();
			item.shootSpeed = 9f;
			item.useAmmo = AmmoID.Gel;
		}

		public override bool ConsumeAmmo(Player player)
		{
			return player.itemAnimation >= player.itemAnimationMax - 4;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 30f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			return true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, 2);
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Firefly, 24);
			recipe.AddIngredient(ItemID.Gel, 75);
			recipe.AddIngredient(ItemID.Frog, 36);
			recipe.AddIngredient(ItemID.SoulofLight, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();

			recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Firefly, 38);
			recipe.AddIngredient(ItemID.Gel, 90);
			recipe.AddIngredient(ItemID.Frog, 50);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}