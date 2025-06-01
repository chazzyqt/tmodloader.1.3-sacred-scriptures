using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Projectiles
{
    //Projectile for the CrimsonQueenStaff
    public class CrimsonExplosion : ModProjectile
    {
        public override bool Autoload(ref string name) { return true; }
        #region Textures
        public static Texture2D textureTargetS;
        public static Texture2D textureTargetM;
        public static Texture2D textureTargetL;
        public static Texture2D textureTargetXL;
        public static Texture2D textureLaser;
        
        public float[] textureAlphas = new float[maxCharge];
        #endregion

        public const int maxCharge = 15;

        #region Tweaking Values
        public const int explosionSize = 100;

        public const int chargeTicksIdle = 6;
        public const int chargeTicksMax = 24 * (1 + chargeTicksIdle); //lower faster charge
        public const float chargeTickGameMax = (chargeTicksMax / chargeTicksIdle);
        public const int castTicksTime = 5;
        public const int fireTicksTime = 3;
        public const float manaIncrease = 0.5f;
        public const int manaMaintainCost = 35;
        public const float explosionScale = 1.15f;
        public const float farDistance = 2000;
        public const float maxDistance = 2500;
        #endregion

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Clan's Explosion");
            if(!Main.dedServ)
            {
                textureTargetS = mod.GetTexture("Content/Projectiles/Explosion_Targetsm");
                textureTargetM = mod.GetTexture("Content/Projectiles/Explosion_Targetmd");
                textureTargetL = mod.GetTexture("Content/Projectiles/Explosion_Targetlg");
                textureTargetXL = mod.GetTexture("Content/Projectiles/Explosion_Targetxl");
                textureLaser = mod.GetTexture("Content/Projectiles/Explosion_Laser");
            }
        }
        public override void SetDefaults()
        {
            projectile.width = explosionSize;
            projectile.height = explosionSize;

            projectile.penetrate = -1;

            //projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
        }
        public override bool? CanCutTiles() { return false; }

        private int ChargeLevel { get { return (int)projectile.ai[0]; } set { projectile.ai[0] = value; } }
        private int ChargeTicks { get { return (int)projectile.ai[1]; } set { projectile.ai[1] = value; } }
        private int Damage { get { return (int)projectile.localAI[1]; } set { projectile.localAI[1] = value; } }
        private bool Casting { get { return ChargeTicks < 0; } }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.velocity = Vector2.Zero;

            if (player.dead || !player.active)
            {
                projectile.timeLeft = 0;
                projectile.netUpdate = true;
                SpawnExplosionAndFade(true);
            }
            else { projectile.timeLeft++; }

            player.heldProj = projectile.whoAmI;

            if (projectile.damage > 0)
            {
                Damage = (projectile.damage + Content.Items.CrimsonQueenStaff.baseDamage * 9) / 10;
                projectile.damage = 0;
            }

            if (Main.myPlayer == projectile.owner)
            {
                if (!player.channel && !Casting)
                {
                    ChangeToCastState();
                }
            }

            if (!Casting)
            {
                if (ChargeLevel < maxCharge)
                {
                    if (PlayerCanChannel(player))
                    {
                        float rise = 4.5f;
                        if (PlayerStandingStillChannel(player))
                        {
                            ChargeTicks += chargeTicksIdle;
                            rise += rise * chargeTicksIdle;
                        }
                        projectile.position.Y -= (float)projectile.height / (chargeTickGameMax * 25f);
                    }

                    if (ChargeTicks >= chargeTicksMax)
                    {
                        ChargeTicks = 0;
                        LevelUpCharge(player);
                    }
                }
                else
                {
                    ConsumeMana(player, manaMaintainCost);
                }

                for (int i = 0; i < Math.Min(textureAlphas.Length, ChargeLevel + 1); i++)
                {
                    if (i == ChargeLevel)
                    {
                        if (textureAlphas[i] < 1f) { textureAlphas[i] = ChargeTicks / (chargeTicksMax - 1f); }
                        if (textureAlphas[i] > 1f) { textureAlphas[i] = 1f; }
                    }
                    else
                    {
                        textureAlphas[i] = 1f;
                    }
                }
                PlayerCharging(player);
            }
            else
            {
                if (-ChargeTicks < (2 + ChargeLevel) * castTicksTime)
                {
                    ChargeTicks--;
                    PlayerCasting(player);
                }
                else
                {
                    SpawnExplosionAndFade(false);
                }
            }
        }

        private void SpawnExplosionAndFade(bool weaken)
        {
            if (projectile.alpha == 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.velocity,
                        ModContent.ProjectileType<Explooosion>(),
                        CalculateDamage(weaken),
                        projectile.knockBack,
                        projectile.owner,
                        ChargeLevel,
                        projectile.width);
                }
            }
            projectile.alpha += 25;
            projectile.scale *= 0.95f;
            if (projectile.alpha > 255)
            {
                projectile.timeLeft = 0;
                projectile.netUpdate = true;
            }
        }

        private bool PlayerStandingStillChannel(Player player)
        {
            return player.velocity.X == 0 && player.velocity.Y == 0 &&
                              Vector2.Distance(player.Center, projectile.Center) <= farDistance;
        }
        private bool PlayerCanChannel(Player player) { return !player.dead && !player.frozen && !player.stoned && !player.webbed && !player.tongued && !player.silence; }
        private void LevelUpCharge(Player player)
        {
            if (!ConsumeMana(player, CalculateManaCost())) return;

            ChargeLevel++;

            Vector2 centre = new Vector2(projectile.Center.X, projectile.Center.Y);
            projectile.Size = new Vector2(explosionSize, explosionSize);
            for (int i = 0; i < ChargeLevel; i++)
            {
                projectile.Size *= explosionScale;
            }
            projectile.Center = centre;

            if (Main.myPlayer == projectile.owner)
            {
                Main.PlaySound(25, player.position);
            }

            projectile.netUpdate = true;
        }
        private void ChangeToCastState()
        {
            ChargeTicks = -1;
            projectile.netUpdate = true;
        }

        private int CalculateManaCost()
        {
            return (int)(Main.player[projectile.owner].HeldItem.mana * (1f + (ChargeLevel + 1) / 2f));
        }
        private bool ConsumeMana(Player player, int manaCost)
        {
            if (player.whoAmI != Main.myPlayer) return true;

            if (player.statMana < manaCost && player.manaFlower)
            { player.QuickMana(); }

            if (player.statMana < manaCost)
            { ChangeToCastState(); return false; }

            player.statMana -= manaCost;
            return true;
        }

        private int CalculateDamage(bool weaken)
        {
            int level = ChargeLevel;
            if (weaken) { level /= 3; }
            if (level > 0)
            {
                Damage = Damage + (int)(Damage * Math.Pow(2, level * 10f / Explosion.maxCharge));
                projectile.knockBack *= 1 + (int)Math.Log10(level * 10);
            }

            return Damage / (1 + (Explosion.fireTicksTime * (1 + ChargeLevel) / 10));
        }

        private void PlayerCharging(Player player)
        {
            player.aggro += 1800; //900 higher value = higher aggro

            Vector2 vectorDiff = player.Center - projectile.Center;
            if (vectorDiff.X < 0f)
            {
                player.ChangeDir(1);
                projectile.direction = 1;
            }
            else
            {
                player.ChangeDir(-1);
                projectile.direction = -1;
            }

            player.itemAnimation = player.itemAnimationMax;
            player.itemTime = player.itemAnimationMax;

            player.itemRotation = (vectorDiff * -1f * (float)projectile.direction).ToRotation();
            projectile.spriteDirection = ((vectorDiff.X > 0f) ? -1 : 1);

            projectile.frameCounter++;
            if (projectile.frameCounter > (12 + maxCharge - ChargeLevel / 2))
            {
                projectile.frameCounter = 0;
                Main.PlaySound(SoundID.Item34.WithVolume(0.4f + 0.05f * ChargeLevel), player.Center);
            }

            Vector2 staffTip = player.MountedCenter + new Vector2(
                player.inventory[player.selectedItem].width * 1.1f * (float)Math.Cos(player.itemRotation),
                player.inventory[player.selectedItem].width * 1.1f * (float)Math.Sin(player.itemRotation)
            ) * player.direction;
            Dust d = Dust.NewDustDirect(staffTip - new Vector2(3 - player.direction * 2 + ChargeLevel * 0.05f, 3 + ChargeLevel * 0.05f),
                0, 0, 174, 0, 0, 0, Color.White, 0.3f + (0.1f * ChargeLevel));
            d.noGravity = true;
            d.velocity *= 0.3f;
        }
        private bool playerStartCast = false;
        private void PlayerCasting(Player player)
        {
            if(!playerStartCast)
            {
                Main.PlaySound(2, player.position, 72);
                playerStartCast = true;
            }
            if (!PlayerCanChannel(player)) return;

            player.itemRotation = -1.5708f * player.direction;
            player.itemAnimation = player.itemAnimationMax / 2;
            player.itemTime = player.itemAnimationMax / 2;
            player.noKnockback = true;
            player.velocity /= 2f;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            drawChargeCircles(spriteBatch);

            drawCastingCircles(spriteBatch);

            Player player = Main.player[projectile.owner];
            if (!Casting)
            {
                if(PlayerStandingStillChannel(player))
                { SacredScriptures.drawMagicCast(player, spriteBatch, Color.OrangeRed); }
            }
            else
            {
                if (projectile.alpha == 0)
                { SacredScriptures.drawMagicCast(player, spriteBatch, Color.OrangeRed); }
            }
            return false;
        }

        private void drawChargeCircles(SpriteBatch spriteBatch)
        {
            Vector2 circleVector;

            for (int i = 0; i < 2; i++)
            {
                float randAng = Main.rand.Next(-31416, 31417) * 0.0001f;
                circleVector = new Vector2(
                    50 * (float)Math.Cos(randAng) - 4,
                    50 * (float)Math.Sin(randAng) - 4
                    );
                int d = Dust.NewDust(projectile.Center + circleVector, 0, 0, 174, 0, 0, 0, Color.White, 0.1f);
                Main.dust[d].fadeIn = 0.4f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.25f;
                d = Dust.NewDust(projectile.Center, 0, 0, 183, circleVector.X / 30, circleVector.Y / 30, 0, Color.White, 0.1f);
                Main.dust[d].fadeIn = 0.8f;
                Main.dust[d].noGravity = true;
            }

            float size;
            float alpha;
            Vector2 castCentre;
            float sizeCircle;
            float sizeOffset;
            float angle;
            for (int i = 0; i < Math.Min(textureAlphas.Length, ChargeLevel + 1); i++)
            {
                size = (float)(Explosion.explosionSize * Math.Pow(explosionScale, i));
                alpha = textureAlphas[i] * projectile.Opacity;
                switch (i)
                {
                    case 0: ///////////////////////////////////////////////////////////////////// initial circle
                        castCentre = projectile.Center - Main.screenPosition;
                        spriteBatch.Draw(textureTargetM,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            (float)(Main.time * 0.05f),
                            textureTargetM.Bounds.Center.ToVector2(),
                            size * projectile.scale / textureTargetM.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 1: ///////////////////////////////////////////////////////////////////// flat centre
                        castCentre = projectile.Center - Main.screenPosition;

                        spriteBatch.Draw(textureTargetS,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetS.Bounds.Center.ToVector2(),
                            new Vector2(size, size / 3) * projectile.scale / textureTargetS.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 2: ///////////////////////////////////////////////////////////////////// initial circle inner
                        castCentre = projectile.Center - Main.screenPosition;

                        spriteBatch.Draw(textureTargetS,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            (float)(Main.time * 0.05f),
                            textureTargetS.Bounds.Center.ToVector2(),
                            size * projectile.scale / textureTargetS.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 3: ///////////////////////////////////////////////////////////////////// backspin circle
                        castCentre = projectile.Center - Main.screenPosition;

                        spriteBatch.Draw(textureTargetS,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            (float)(Main.time * 0.08f),
                            textureTargetS.Bounds.Center.ToVector2(),
                            new Vector2(size, size / 3) * projectile.scale / textureTargetS.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 4: ///////////////////////////////////////////////////////////////////// orbiter circle 1
                        sizeCircle = size * (2 / 5f);
                        sizeOffset = size * (3 / 5f);
                        angle = (float)(Main.time % 62831) * -0.02f;
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            sizeOffset * 0.5f * (float)Math.Cos(angle),
                            sizeOffset * 0.5f * (float)Math.Sin(angle)
                        );

                        spriteBatch.Draw(textureTargetM,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            (float)(Main.time * 0.03f),
                            textureTargetM.Bounds.Center.ToVector2(),
                            new Vector2(sizeCircle, sizeCircle) * projectile.scale / textureTargetM.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 5: ///////////////////////////////////////////////////////////////////// orbiter circle 2
                        sizeCircle = size * (2 / 5f);
                        sizeOffset = size * (3 / 5f);
                        angle = (float)((Main.time + 20943) % 62831) * -0.02f;
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            sizeOffset * 0.5f * (float)Math.Cos(angle),
                            sizeOffset * 0.5f * (float)Math.Sin(angle)
                        );

                        spriteBatch.Draw(textureTargetM,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            (float)(Main.time * 0.02f),
                            textureTargetM.Bounds.Center.ToVector2(),
                            new Vector2(sizeCircle, sizeCircle) * projectile.scale / textureTargetM.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 6: ///////////////////////////////////////////////////////////////////// orbiter circle 3
                        sizeCircle = size * (2 / 5f);
                        sizeOffset = size * (3 / 5f);
                        angle = (float)((Main.time - 20943) % 62831) * -0.02f;
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            sizeOffset * 0.5f * (float)Math.Cos(angle),
                            sizeOffset * 0.5f * (float)Math.Sin(angle)
                        );

                        spriteBatch.Draw(textureTargetM,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            (float)(Main.time * 0.015f),
                            textureTargetM.Bounds.Center.ToVector2(),
                            new Vector2(sizeCircle, sizeCircle) * projectile.scale / textureTargetM.Width,
                            SpriteEffects.None,
                            0f);
                        break;

                    case 7: ///////////////////////////////////////////////////////////////////// larger circle
                        if (alpha > 0.3f) { alpha = 0.3f; }
                        castCentre = projectile.Center - Main.screenPosition;

                        spriteBatch.Draw(textureTargetL,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            (float)(Main.time * 0.01f),
                            textureTargetL.Bounds.Center.ToVector2(),
                            size * projectile.scale / textureTargetL.Width,
                            SpriteEffects.None,
                            0f);

                        for (int j = 0; j < 2; j++)
                        {
                            float randAng = Main.rand.Next(-31416, 31417) * 0.0001f;
                            circleVector = new Vector2(
                                textureTargetL.Width * 0.25f * (float)Math.Cos(randAng) - 20,
                                textureTargetL.Height * 0.25f * (float)Math.Sin(randAng) - 24
                                );
                            int d = Dust.NewDust(projectile.Center + circleVector,
                                32, 32, 174, 0, 0, 0, Color.White, 0.1f);
                            Main.dust[d].fadeIn = 0.6f;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 0.25f;
                        }
                        break;
                    case 8: ///////////////////////////////////////////////////////////////////// top circle
                        if (alpha > 0.5f) { alpha = 0.5f; }
                        sizeCircle = size * (1 / 7f);
                        sizeOffset = size * (6 / 7f);
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            0,
                            -sizeOffset * 0.5f
                        );

                        spriteBatch.Draw(textureTargetXL,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetXL.Bounds.Center.ToVector2(),
                            new Vector2(size, sizeCircle) * projectile.scale / textureTargetXL.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 9: ///////////////////////////////////////////////////////////////////// bottom circle
                        if (alpha > 0.5f) { alpha = 0.5f; }
                        sizeCircle = size * (1 / 6f);
                        sizeOffset = size * (5 / 6f);
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            0,
                            sizeOffset * 0.5f
                        );

                        spriteBatch.Draw(textureTargetXL,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetXL.Bounds.Center.ToVector2(),
                            new Vector2(size, sizeCircle) * projectile.scale / textureTargetXL.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 10: ///////////////////////////////////////////////////////////////////// mid circle
                        if (alpha > 0.5f) { alpha = 0.5f; }
                        sizeCircle = size * (1 / 7f);
                        castCentre = projectile.Center - Main.screenPosition;

                        spriteBatch.Draw(textureTargetL,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetL.Bounds.Center.ToVector2(),
                            new Vector2(size, sizeCircle) * projectile.scale / textureTargetL.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 11: ///////////////////////////////////////////////////////////////////// guide topper 1
                        if (alpha > 0.5f) { alpha = 0.5f; }
                        sizeCircle = size * (1 / 9f);
                        sizeOffset = size * (8 / 9f);
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            0,
                            -sizeOffset * 0.5f
                        );

                        spriteBatch.Draw(textureTargetS,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetS.Bounds.Center.ToVector2(),
                            new Vector2(size * 0.7f, sizeCircle) * projectile.scale / textureTargetS.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 12: ///////////////////////////////////////////////////////////////////// guide topper 2
                        if (alpha > 0.5f) { alpha = 0.5f; }
                        sizeCircle = size * (1 / 9f);
                        sizeOffset = size * (8 / 9f);
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            0,
                            -sizeOffset * 0.5f
                        );

                        spriteBatch.Draw(textureTargetXL,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetXL.Bounds.Center.ToVector2(),
                            new Vector2(size * 0.8f, sizeCircle) * projectile.scale / textureTargetXL.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 13: ///////////////////////////////////////////////////////////////////// guide topper 3
                        if (alpha > 0.5f) { alpha = 0.5f; }
                        sizeCircle = size * (1 / 9f);
                        sizeOffset = size * (8 / 9f);
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            0,
                            -sizeOffset * 0.5f
                        );

                        spriteBatch.Draw(textureTargetXL,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetXL.Bounds.Center.ToVector2(),
                            new Vector2(size * 0.85f, sizeCircle) * projectile.scale / textureTargetXL.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                    case 14: ///////////////////////////////////////////////////////////////////// guide topper 4
                        if (alpha > 0.5f) { alpha = 0.5f; }
                        sizeCircle = size * (1 / 9f);
                        sizeOffset = size * (8 / 9f);
                        castCentre = projectile.Center - Main.screenPosition
                            + new Vector2(
                            0,
                            -sizeOffset * 0.5f
                        );

                        spriteBatch.Draw(textureTargetXL,
                            castCentre,
                            null,
                            new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                            0,
                            textureTargetXL.Bounds.Center.ToVector2(),
                            new Vector2(size * 0.6f, sizeCircle) * projectile.scale / textureTargetXL.Width,
                            SpriteEffects.None,
                            0f);
                        break;
                }
            }
        }
        private void drawCastingCircles(SpriteBatch spriteBatch)
        {
            Player player = Main.player[projectile.owner];
            
            float size;
            float alpha;
            Vector2 castCentre;
            float sizeCircle;
            if (Casting)
            {
                int distance = 120 + ChargeLevel * 3;

                alpha = projectile.Opacity;

                size = projectile.width;
                sizeCircle = size * (1 / 9f);
                castCentre = projectile.Center
                    + new Vector2(
                    0,
                    -size * 0.75f
                );

                spriteBatch.Draw(textureTargetL,
                    castCentre - Main.screenPosition,
                    null,
                    new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                    0,
                    textureTargetL.Bounds.Center.ToVector2(),
                    new Vector2(size, sizeCircle) * projectile.scale / textureTargetL.Width,
                    SpriteEffects.None,
                    0f);

                int d = Dust.NewDust(castCentre - new Vector2(2 + ChargeLevel * 0.2f, 0), 8, 0, 162,
                    0, 2, 0, Color.White, 1 + ChargeLevel * 0.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X *= ChargeLevel;

                if (projectile.alpha == 0)
                {
                    drawLaser(spriteBatch, player.Top + new Vector2(0, -34), player.Top + new Vector2(0, -distance));
                    drawLaser(spriteBatch, castCentre, projectile.Center);
                }

                Vector2 circleVector;
                float randAng = Main.rand.Next(-31416, 31417) * 0.0001f;
                circleVector = new Vector2(
                    58 * (float)Math.Cos(randAng),
                    10 * (float)Math.Sin(randAng)
                    );
                d = Dust.NewDust(player.Top + new Vector2(-4, -distance - 4) + circleVector, 0, 0,
                    170, circleVector.X, circleVector.Y, 0, Color.White, 0.1f);
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= -0.07f;

                spriteBatch.Draw(textureTargetS,
                    player.Top + new Vector2(0, -distance) - Main.screenPosition,
                    null,
                    new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                    0,
                    textureTargetS.Bounds.Center.ToVector2(),
                    new Vector2(player.width * 8, player.width * 1.6f) * projectile.scale / textureTargetS.Width,
                    SpriteEffects.None,
                    0f);
                if (ChargeLevel < 8) return;
                spriteBatch.Draw(textureTargetM,
                    player.Top + new Vector2(0, -distance * 0.6f) - Main.screenPosition,
                    null,
                    new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                    0,
                    textureTargetM.Bounds.Center.ToVector2(),
                    new Vector2(player.width * 5, player.width) * projectile.scale / textureTargetM.Width,
                    SpriteEffects.None,
                    0f);
                if (ChargeLevel < 12) return;
                spriteBatch.Draw(textureTargetL,
                    player.Top + new Vector2(0, -distance * 0.3f) - Main.screenPosition,
                    null,
                    new Color(1f * alpha, 1f * alpha, 1f * alpha, alpha),
                    0,
                    textureTargetL.Bounds.Center.ToVector2(),
                    new Vector2(player.width * 4, player.width * 0.8f) * projectile.scale / textureTargetL.Width,
                    SpriteEffects.None,
                    0f);
            }
        }

        private void drawLaser(SpriteBatch spritebatch, Vector2 start, Vector2 end)
        {
            try
            {
                float size = 5f;
                Utils.DrawLaser(
                    spritebatch,
                    textureLaser,
                    start - Main.screenPosition,
                    end - Main.screenPosition,
                    new Vector2(Math.Max(0.1f, size / (4f + Math.Abs( + ChargeTicks)))),
                    new Utils.LaserLineFraming(ExplosionLaser));
            }
            catch { }
        }
        private void ExplosionLaser(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
        {
            color = Color.White;
            if (stage == 0)
            {
                distCovered = 33f;
                frame = new Rectangle(0, 0, 22, 22);
                origin = frame.Size() / 2f;
                return;
            }
            if (stage == 1)
            {
                frame = new Rectangle(0, 22, 22, 22);
                distCovered = (float)frame.Height;
                origin = new Vector2((float)(frame.Width / 2), 0f);
                return;
            }
            if (stage == 2)
            {
                distCovered = 22f;
                frame = new Rectangle(0, 44, 22, 22);
                origin = new Vector2((float)(frame.Width / 2), 1f);
                return;
            }
            distCovered = 9999f;
            frame = Rectangle.Empty;
            origin = Vector2.Zero;
            color = Color.Transparent;
        }
    }
}