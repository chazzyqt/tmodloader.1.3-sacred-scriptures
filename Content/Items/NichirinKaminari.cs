using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
    public class NichirinKaminari : ModItem
    {
        public override void SetStaticDefaults(){
            DisplayName.SetDefault("Nichirin Kaminari");
            Tooltip.SetDefault(
                "Weilder must hone the first form into perfection.\n" +
                "Dash through multiple enemies dealing damage.\n" +
                "Press or hold right click to perform multiple slash in seconds.\n" +
                "Beware you will be vulnerable for a few seconds after the ability ends.\n" +
                "Great for travelling use! -Sweii 2022.");
        }
        public override void SetDefaults()
        {
            item.damage = 355;
			item.ranged = false;
			item.melee = true;
			item.magic = false;
			item.scale = 1f;
			item.width = 40;
			item.height = 40;
			item.useTime = 5 / 4; //DashCD / SwingCD
			item.useAnimation = 15;
			item.useStyle = 1;
			item.noMelee = false;
			item.knockBack = 1;
			item.value = Item.buyPrice(0, 23, 25, 0);
			item.rare = 8;
            item.UseSound = SoundID.Item1;
			item.autoReuse = true;
            item.mana = 10;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Muramasa, 1);
            recipe.AddIngredient(ItemID.Katana, 1);
            recipe.AddIngredient(ItemID.BrokenHeroSword, 1);
            recipe.AddIngredient(ItemID.SpectreBar, 12);
            recipe.AddIngredient(ItemID.SoulofLight, 26);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool AltFunctionUse(Player player) {
			return true;
		}

		public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2) {//right click
				item.useTime = 4 / 4;
			    item.useAnimation = 1;
                item.mana = 35;
			}
			else {//left click
				item.useTime = 5 / 4;
			    item.useAnimation = 15;
                item.mana = 10;
			}
			return base.CanUseItem(player);
		}

        public override void HoldItem(Player player)
        {
            SacredScriptures.HoldItemManager(player, item, ModContent.ProjectileType<NichirinSlash>(),
                Color.Yellow, 0.9f, player.itemTime == 0 ? 0f : 1f);//Color.HotPink
        }

        public override bool UseItemFrame(Player player)
        {
            SacredScriptures.UseItemFrame(player, 0.9f, item.isBeingGrabbed);
            return true;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            int height = 98;
            int length = 100;
            SacredScriptures.UseItemHitboxCalculate(player, item, ref hitbox, ref noHitbox, 0.9f, height, length);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Color colour = new Color(255, 180, 210, 119);
            SacredScriptures.OnHitFX(player, target, crit, colour);
        }
    }

    public class NichirinSlash : ModProjectile
    {
        public const int specialProjFrames = 5;
        bool sndOnce = true;
        int chargeSlashDirection = 1;
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = specialProjFrames;
        }
        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.aiStyle = -1;
            projectile.timeLeft = 60;

            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.penetrate = 6;
        }

        public override bool? CanCutTiles() { return SlashLogic == 0; }
        public float FrameCheck
        {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }
        public int SlashLogic
        {
            get { return (int)projectile.ai[1]; }
            set { projectile.ai[1] = value; }
        }

        Vector2? preDashVelocity;
        bool firstFrame = true;
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (SacredScriptures.AINormalSlash(projectile, SlashLogic)) { }
            else
            {
                // Charged attack
                SacredScriptures.AISetChargeSlashVariables(player, chargeSlashDirection);
                SacredScriptures.NormalSlash(projectile, player);

                // Play charged sound
                if (sndOnce)
                {
                    Main.PlaySound(SoundID.Item71, projectile.Center); sndOnce = false;     
                    preDashVelocity = null; // Save velocity before dash
                }
            }

            if (SlashLogic == 0)
            {
                if (player.altFunctionUse == 2)
                {
                    player.AddBuff(BuffID.Slow, 900); //300 = 10 seconds
                    player.AddBuff(BuffID.Weak, 900);
                    //player.AddBuff(BuffID.BrokenArmor, 900);

                    float dashFrameDuration = 3; //6 //duration of dash
                    float dashSpeed = 64f; //128 //dash speed to destination
                    int freezeFrame = 2;
                    bool dashing = SacredScriptures.AIDashSlash(player, projectile, dashFrameDuration, dashSpeed, freezeFrame, ref preDashVelocity);

                    if (dashing)
                    {
                        Dust.NewDust(player.Center - new Vector2(6, 6), 4, 4, 20);

                        // Coloured line trail
                        Vector2 dashStep = player.position - player.oldPosition;
                        for (int i = 0; i < 8; i++)
                        {
                            Dust d = Main.dust[Dust.NewDust(player.Center - (dashStep / 8) * i, 
                                0, 0, 181, dashStep.X / 32, dashStep.Y / 32, 0, default(Color), 1.3f)];
                            d.noGravity = true;
                            d.velocity *= 0.1f;
                        }
                    }

                    // Calculate ending position dust
                    if (firstFrame)
                    {
                        firstFrame = false;

                        Vector2 endPosition = player.position;
                        Vector2 dashVector = projectile.velocity * dashSpeed;
                        for (int i = 0; i < dashFrameDuration * 2; i++)
                        {
                            Vector2 move = Collision.TileCollision(
                                endPosition, dashVector / 2,
                                player.width, player.height,
                                false, false, (int)player.gravDir);
                            if (move == Vector2.Zero) break;
                            endPosition += move;
                        }

                        // dash dust from the total distance over the duration
                        Vector2 totalDistanceStep = 
                            (endPosition + new Vector2(player.width / 2, player.height / 2)
                            - player.Center) / dashFrameDuration;
                        for (int i = 0; i < dashFrameDuration; i++)
                        {
                            Vector2 pos = player.Center + (totalDistanceStep * i) - new Vector2(4, 4);
                            for (int j = 0; j < 5; j++)
                            {
                                pos += totalDistanceStep * (j / 5f);
                                Dust d = Main.dust[Dust.NewDust(pos, 0, 0,
                                    175, projectile.velocity.X, projectile.velocity.Y,
                                    0, Color.White, 3f)]; //1f
                                d.noGravity = true;
                                d.velocity *= 0.05f;
                            }
                        }
                    }
                }
                else
                {
                    float dashFrameDuration = 3; //6 //duration of dash
                    float dashSpeed = 64f; //128 //dash speed to destination
                    int freezeFrame = 2;
                    bool dashing = SacredScriptures.AIDashSlash(player, projectile, dashFrameDuration, dashSpeed, freezeFrame, ref preDashVelocity);

                    if (dashing)
                    {
                        Dust.NewDust(player.Center - new Vector2(6, 6), 4, 4, 20);

                        // Coloured line trail
                        Vector2 dashStep = player.position - player.oldPosition;
                        for (int i = 0; i < 8; i++)
                        {
                            Dust d = Main.dust[Dust.NewDust(player.Center - (dashStep / 8) * i, 
                                0, 0, 181, dashStep.X / 32, dashStep.Y / 32, 0, default(Color), 1.3f)];
                            d.noGravity = true;
                            d.velocity *= 0.1f;
                        }
                    }

                    // Calculate ending position dust
                    if (firstFrame)
                    {
                        firstFrame = false;

                        Vector2 endPosition = player.position;
                        Vector2 dashVector = projectile.velocity * dashSpeed;
                        for (int i = 0; i < dashFrameDuration * 2; i++)
                        {
                            Vector2 move = Collision.TileCollision(
                                endPosition, dashVector / 2,
                                player.width, player.height,
                                false, false, (int)player.gravDir);
                            if (move == Vector2.Zero) break;
                            endPosition += move;
                        }

                        // dash dust from the total distance over the duration
                        Vector2 totalDistanceStep = 
                            (endPosition + new Vector2(player.width / 2, player.height / 2)
                            - player.Center) / dashFrameDuration;
                        for (int i = 0; i < dashFrameDuration; i++)
                        {
                            Vector2 pos = player.Center + (totalDistanceStep * i) - new Vector2(4, 4);
                            for (int j = 0; j < 5; j++)
                            {
                                pos += totalDistanceStep * (j / 5f);
                                Dust d = Main.dust[Dust.NewDust(pos, 0, 0,
                                    175, projectile.velocity.X, projectile.velocity.Y,
                                    0, Color.White, 3f)]; //1f
                                d.noGravity = true;
                                d.velocity *= 0.05f;
                            }
                        }
                    }
                }
            }

            projectile.damage = 0;
            FrameCheck += 1f; // Framerate
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            int weaponItemID = ModContent.ItemType<NichirinKaminari>();
            Color lighting = Lighting.GetColor((int)(player.MountedCenter.X / 16), (int)(player.MountedCenter.Y / 16));
            return SacredScriptures.PreDrawSlashAndWeapon(spriteBatch, projectile, weaponItemID, lighting,
                null,//SlashLogic == 0f ? specialSlash : null,
                lighting,
                specialProjFrames,
                SlashLogic == 0f ? chargeSlashDirection : SlashLogic,
                SlashLogic == 0f);
        }

    }
}