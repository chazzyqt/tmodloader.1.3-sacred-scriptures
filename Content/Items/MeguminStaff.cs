﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
    public class MeguminStaff : ModItem
    {
        public const int baseDamage = 40;
        public override void SetStaticDefaults()
        {
            //Came from WeaponOut Mod, be sure to check them out!
            DisplayName.SetDefault("Megumin's Staff");
            Tooltip.SetDefault(
                "Create a powerful explosion at a location\n" +
                "Increase channel speed by standing still\n" +
                "Enemies are more likely to target you while casting");
        }

        public override void SetDefaults()
        {
            item.width = 52;
            item.height = 14;
            item.scale = 1f;

            item.magic = true;
            item.channel = true;
            item.mana = 10;
            item.damage = baseDamage; //damage * (charge ^ 2) *1(0) - *25(8) - *160(11) - *1000(15)
            item.knockBack = 3f; //up to x2.18
            item.autoReuse = true;

            item.noMelee = true;
            Item.staff[item.type] = true; //rotate weapon, as it is a staff
            item.shoot = ModContent.ProjectileType<Projectiles.Explosion>();
            item.shootSpeed = 1;

            item.useStyle = 5;
            item.UseSound = SoundID.Item8;
            item.useTime = 60;
            item.useAnimation = 60;

            item.rare = 8;
            item.value = Item.buyPrice(0, 5, 50, 0); //10000;
        }

        public override void UseStyle(Player player)
        {
            SacredScriptures.modifyPlayerItemLocation(player, -4, -5);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            return true;
        }

        public override void AddRecipes() {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WandofSparking, 1); // Surface chest
            recipe.AddIngredient(ItemID.RubyStaff, 1); // Extractinator, or Gold ore world
            recipe.AddIngredient(ItemID.MeteorStaff, 1); // Meteorite
            recipe.AddIngredient(ItemID.InfernoFork, 1); // Inferno caster
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}