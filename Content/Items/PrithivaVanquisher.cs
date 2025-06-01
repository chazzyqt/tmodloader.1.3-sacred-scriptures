using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class PrithivaVanquisher : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prithiva Vanquisher");
			Tooltip.SetDefault(
				"To cleanse the land and defend our safe harbor, that was the first contract. And now, the final contract too, has been set in stone.\n" + 
				"Throws a exploding spear that deals massive damage in a certain area.\n" +
				"Grants immense defense when held.\n" + 
				"Enemies are will target you.");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}

		public override void SetDefaults() 
		{
			item.damage = 909;
			item.ranged = false;
			item.melee = true;
			item.magic = false;
			item.width = 20;
			item.height = 20;
			item.useTime = 12;
			item.useAnimation = 12;
			item.knockBack = 1;
			item.value = Item.buyPrice(0, 98, 0, 0);
			item.rare = 11;
			item.UseSound = SoundID.Item1;
			item.shoot = ModContent.ProjectileType<Projectiles.PrithivaVanquisherSpear>();
			item.shootSpeed = 20f;
			item.autoReuse = true;
			item.crit = 6;
			item.useStyle = 1;
			item.noUseGraphic = true;
			item.noMelee = true;
			item.glowMask = customGlowMask;
		}

		public override void HoldItem(Player player)
        {
			player.aggro += 1800; //higher value = higher aggro
            player.statDefense += 55; //bonus defence when held
        }
		
		/*public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X - 5, position.Y + 0, speedX + ((float)Main.rand.Next(-230, 230) / 300), speedY + ((float)Main.rand.Next(-230, 230) / 300), ModContent.ProjectileType<Projectiles.PrithivaVanquisherProj>(), 50, knockBack, player.whoAmI, 0f, 0f);
			return true;
		}
		
		public override bool CanUseItem(Player player) //initially spear
		{
			if (player.ownedProjectileCounts[item.shoot] > 0) //only one projectile spawns, if projectile exist then spawn false
				return false;
			return true;
		}*/

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DayBreak, 1);
			recipe.AddIngredient(ItemID.Topaz, 25);
			recipe.AddIngredient(ItemID.LunarBar, 12);
			recipe.AddIngredient(ItemID.FragmentSolar, 16);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}