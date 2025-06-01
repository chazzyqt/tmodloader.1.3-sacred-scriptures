using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics;
using Terraria.Localization;
using Terraria.DataStructures;

namespace SacredScriptures
{
    //Load Mod
    /*--------------------------------------------------------------------------------------------*/
	public class SacredScriptures : Mod
	{
		internal static SacredScriptures mod;
        public static int shakeIntensity = 0;
        internal List<Func<Player, Item, DrawData, bool>> sacredScripturesCustomPreDrawMethods;
        internal List<Func<Player, Item, int, int>> sacredScripturesCustomHoldMethods;

        public SacredScriptures()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
            sacredScripturesCustomPreDrawMethods = new List<Func<Player, Item, DrawData, bool>>();
            sacredScripturesCustomHoldMethods = new List<Func<Player, Item, int, int>>();
        }
        
        //Megumin Staff
        /*--------------------------------------------------------------------------------------------*/
        #region Utils
        public static void drawMagicCast(Player player, SpriteBatch spriteBatch, Color colour, int frame = -1)
        {
            if (frame < 0) frame = (int)Main.time % 48 / 12;

            Texture2D textureCasting = Main.extraTexture[51];
            Vector2 origin = player.Bottom + new Vector2(0f, player.gfxOffY + 4f);
            if (player.gravDir < 0) origin.Y -= player.height + 8f;
            Rectangle rectangle = textureCasting.Frame(1, 4, 0, Math.Max(0, Math.Min(3, frame)));
            Vector2 origin2 = rectangle.Size() * new Vector2(0.5f, 1f);
            if (player.gravDir < 0) origin2.Y = 0f;
            spriteBatch.Draw(
                textureCasting, new Vector2((float)((int)(origin.X - Main.screenPosition.X)), (float)((int)(origin.Y - Main.screenPosition.Y))),
                new Rectangle?(rectangle), colour, 0f, origin2, 1f,
                player.gravDir >= 0f ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
        }

        public static void modifyPlayerItemLocation(Player player, float X, float Y)
        {
            float cosRot = (float)Math.Cos(player.itemRotation);
            float sinRot = (float)Math.Sin(player.itemRotation);
            player.itemLocation.X = player.itemLocation.X + (X * cosRot * player.direction) + (Y * sinRot * player.gravDir);
            player.itemLocation.Y = player.itemLocation.Y + (X * sinRot * player.direction) - (Y * cosRot * player.gravDir);
        }

        public static bool SameTeam(Player player1, Player player2)
        {
            if (player1.whoAmI == player2.whoAmI) return true;
            if (player1.team > 0 && player1.team != player2.team) return false;
            if (player1.hostile && player2.hostile && (player1.team == 0 || player2.team == 0)) return false;
            return true;
        }

        public struct SoundData
        {
            public int Type;
            public int x;
            public int y;
            public int Style;
            public float volumeScale;
            public float pitchOffset;
            public SoundData(int Type)
            { this.Type = Type; x = -1; y = -1; Style = 1; volumeScale = 1f; pitchOffset = 0f; }
        }
        public static void ItemFlashFX(Player player, int dustType = 45, SoundData sDat = default(SoundData))
        {
            if (sDat.Type == 0) { sDat = new SoundData(25); }
            if (player.whoAmI == Main.myPlayer)
            { Main.PlaySound(sDat.Type, sDat.x, sDat.y, sDat.Style, sDat.volumeScale, sDat.pitchOffset); }
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(
                    player.position, player.width, player.height, dustType, 0f, 0f, 255,
                    default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
                Main.dust[d].noLight = true;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }
        }
        #endregion

        //Item GlowMask
        /*--------------------------------------------------------------------------------------------*/
        public override void Load()
        {
            mod = this;
        }

        public override void Unload()
        {
            mod = null;
        }

        public static short SetStaticDefaultsGlowMask(ModItem modItem)
        {
            if (!Main.dedServ)
            {
                Texture2D[] glowMasks = new Texture2D[Main.glowMaskTexture.Length + 1];
                for (int i = 0; i < Main.glowMaskTexture.Length; i++)
                {
                    glowMasks[i] = Main.glowMaskTexture[i];
                }
                glowMasks[glowMasks.Length - 1] = mod.GetTexture("Content/Glow/" + modItem.GetType().Name + "_Glow");
                Main.glowMaskTexture = glowMasks;
                return (short)(glowMasks.Length - 1);
            }
            else return 0;
        }

        //Zenitsu
        /*--------------------------------------------------------------------------------------------*/
        private static byte internalChargeTicker;
        public static int DustIDSlashFX;
        const float HALFPI = (float)(Math.PI / 2);

        public override void PostSetupContent() {
            DustIDSlashFX = ModContent.GetInstance<Content.Dusts.SlashDust>().Type;
        }

        private static void SetAttackRotation(Player player, bool quiet = false)
        {
            // Get rotation at use
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                Vector2 value = Vector2.UnitX.RotatedBy((double)player.fullRotation, default(Vector2));
                float num79 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                float num80 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (player.gravDir == -1f)
                {
                    num80 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                }
                player.itemRotation = (float)Math.Atan2((double)(num80 * (float)player.direction), (double)(num79 * (float)player.direction)) - player.fullRotation;
            }

            if (Math.Abs(player.itemRotation) > Math.PI / 2) //swapping then doing it again because lazy and can't be bothered to find in code
            {
                player.direction *= -1;

                if (Main.myPlayer == player.whoAmI)
                {
                    Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                    Vector2 value = Vector2.UnitX.RotatedBy((double)player.fullRotation, default(Vector2));
                    float num79 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                    float num80 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                    if (player.gravDir == -1f)
                    {
                        num80 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                    }
                    player.itemRotation = (float)Math.Atan2((double)(num80 * (float)player.direction), (double)(num79 * (float)player.direction)) - player.fullRotation;
                }
            }

            if (!quiet && Main.netMode == 1 && Main.myPlayer == player.whoAmI)
            {
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                NetMessage.SendData(MessageID.ItemAnimation, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }
        }

        public static void RecentreSlash(Projectile projectile, Player player)
        {
            if (player.direction < 0) projectile.position.X += projectile.width;

            float dist = Math.Max(0, projectile.width - projectile.height); // total distance covered by the moving hitbox

            Vector2 direction = new Vector2(
                (float)Math.Cos(projectile.rotation),
                (float)Math.Sin(projectile.rotation));
            direction.Y *= player.gravDir;
            Vector2 centre = player.MountedCenter;
            Vector2 playerOffset = (player.Size.X * projectile.scale * direction);

            projectile.Center = (centre
                + direction * (dist + projectile.height) / 2
                - playerOffset);
        }

		public static bool HoldItemManager(Player player, Item item, int slashProjectileID, Color chargeColour = default(Color), float slashDelay = 0.9f, float ai1 = 1f, Action<Player, bool> customCharge = null, int delaySpeed = 4)
        {
            bool charged = false;
            // Attacking
            if (player.itemAnimation > 0)
            {
                // JUST attacked
                bool onAttackFrame = player.itemAnimation == player.itemAnimationMax - 1; 
                /*if(Zenitsu.modOverhaul != null)
                { onAttackFrame = player.itemAnimation == player.itemAnimationMax - 2; }*/

                if (onAttackFrame)
                {
                    if (ai1 == 1f || ai1 == -1f)
                    {
                        // Use isBeingGrabbed for alternating swings
                        ai1 = item.isBeingGrabbed ? 1f : -1f;
                        item.isBeingGrabbed = !item.isBeingGrabbed;
                    }
                    else if (ai1 == 0)
                    {
                        // Used to identify a charged attack
                        item.beingGrabbed = true;
                        charged = true;
                    }

                    if (Main.myPlayer == player.whoAmI)
                    {
                        // First frame of attack
                        Vector2 mouse = new Vector2(Main.screenPosition.X + Main.mouseX, Main.screenPosition.Y + Main.mouseY);
                        SetAttackRotation(player);
                        Vector2 velocity = (mouse - player.MountedCenter).SafeNormalize(new Vector2(player.direction, 0));
                        int p = Projectile.NewProjectile(
                            player.MountedCenter,
                            velocity,
                            slashProjectileID,
                            (int)(item.damage * player.meleeDamage),
                            item.scale,
                            player.whoAmI,
                            (int)(player.itemAnimationMax * slashDelay - player.itemAnimationMax), ai1);
                    }

                    // Set item time anyway, if not shoot, also make next slash upwards
                    if (item.shoot <= 0 && player.itemTime == 0)
                    { player.itemTime = item.useTime; item.isBeingGrabbed = false; }
                }

                item.useStyle = 0;
            }
            else
            {
                item.useStyle = 1;
                item.beingGrabbed = false;
            }

            // when counting down
            if (player.itemTime > 0)
            {
                // internalChargeTicker hangs the item time until past the delaySpeed
                // in which case it allows the itemTime to be reduced on that frame.

                // If not moving much, boost item charge speed
                //int delaySpeed = 4; // default, item charged 1 1 / 3 speed

                // If grounded, half the dleay speed
                if (player.velocity.Y == 0)
                { delaySpeed /= 2; }

                // cap
                delaySpeed = Math.Max(delaySpeed, 1);

                // Reset if swinging
                if (player.itemAnimation > 0) { player.itemTime = Math.Max(player.itemTime, item.useTime); }
                else if (Main.myPlayer == player.whoAmI)
                {
                    if (customCharge != null)
                    {
                        customCharge(player, false);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            // Charging dust
                            Vector2 vector = new Vector2(
                                Main.rand.Next(-2048, 2048) * (0.003f * player.itemTime) - 4,
                                Main.rand.Next(-2048, 2048) * (0.003f * player.itemTime) - 4);
                            Dust d = Main.dust[Dust.NewDust(
                                player.MountedCenter + vector, 1, 1,
                                45, 0, 0, 255,
                                chargeColour, 1.5f)];
                            d.velocity = -vector / 16;
                            d.velocity -= player.velocity / 8;
                            d.noLight = true;
                            d.noGravity = true;
                        }
                    }
                }

                // allow item time when past "limit"

                if (internalChargeTicker >= delaySpeed)
                { internalChargeTicker = 0; }

                // delay item time unless at 0
                if (internalChargeTicker > 0)
                { player.itemTime++; }
                internalChargeTicker++;

                // flash and correct
                if (player.itemTime <= 1)
                {
                    player.itemTime = 1;
                    if (customCharge != null)
                    { customCharge(player, true); }
                    else
                    { SacredScriptures.ItemFlashFX(player, 45, new SacredScriptures.SoundData(25) { volumeScale = 0.5f }); }
                }
            }

            // HACK: allows the player to swing the sword when held on the mouse
            if (Main.mouseItem.type == item.type)
            {
                if (player.controlUseItem && player.itemAnimation == 0 && player.itemTime == 0 && player.releaseUseItem)
                { player.itemAnimation = 1; }
            }

            return charged;
        }

        public static void  UseItemFrame(Player player, float delayStart = 0.9f, bool flip = false)
        {
            //counts down from 1 to 0
            float anim = player.itemAnimation / (float)(player.itemAnimationMax);
            int frames = player.itemAnimationMax - player.itemAnimation;

            // animation frames;
            int start, swing, swing2, end;

            if (flip)
            {
                start = 4 * player.bodyFrame.Height;
                swing = 3 * player.bodyFrame.Height;
                swing2 = 2 * player.bodyFrame.Height;
                end = 1 * player.bodyFrame.Height;
            }
            else
            {
                start = 1 * player.bodyFrame.Height;
                swing = 2 * player.bodyFrame.Height;
                swing2 = 3 * player.bodyFrame.Height;
                end = 4 * player.bodyFrame.Height;
            }

            // Actual animation
            if (delayStart < 0.4f) delayStart = 0.4f;
            if (anim > delayStart)
            {
                player.bodyFrame.Y = start;
            }
            else if (anim > delayStart - 0.1f)
            {
                player.bodyFrame.Y = swing;
            }
            else if (anim > delayStart - 0.2f)
            {
                player.bodyFrame.Y = swing2;
            }
            else
            {
                player.bodyFrame.Y = end;
            }
        }

        public static void UseItemHitboxCalculate(Player player, Item item, ref Rectangle hitbox, ref bool noHitbox, float delayStart, int height, int length, float hitboxDuration = 3)
        {
            // Lengthen the hitbox duration the longer it is
            // 1 Frame per 4 tiles after 12
            hitboxDuration += Math.Max(0, (length - 96) / 32);

            height = (int)(height * item.scale);
            length = (int)(length * item.scale);

            // Define when after first swinging the the hitbox becomes active
            int startFrame = (int)(player.itemAnimationMax * delayStart);

            // For faster attacks, the start frame must be at least the magic number
            if (startFrame < hitboxDuration) startFrame = (int)hitboxDuration;

            int activeFrame = startFrame - player.itemAnimation;
            if (activeFrame >= 0 && activeFrame < hitboxDuration + 1)
            {
                hitbox.Height = height;
                hitbox.Width = height;

                float invert = 0f;
                if (player.direction < 0) invert = MathHelper.Pi;
                float dist = Math.Max(0, length - height); // total distance covered by the moving hitbox

                Vector2 direction = new Vector2(
                    (float)Math.Cos(player.itemRotation + invert),
                    (float)Math.Sin(player.itemRotation + invert));
                Vector2 centre = player.MountedCenter - (hitbox.Size() / 2);
                Vector2 playerOffset = (player.Size.X * item.scale * direction);
                hitbox.Location = (centre
                    + direction * hitbox.Height / 2
                    - playerOffset
                    + (dist * direction / hitboxDuration * activeFrame)
                    ).ToPoint();

                player.attackCD = 0;

                // DEBUG hitbox
                //for (int i = 0; i < 256; i++)
                //{ Dust d = Dust.NewDustDirect(hitbox.Location.ToVector2() - new Vector2(2, 2), hitbox.Width, hitbox.Height, 60, 0, 0, 0, default(Color), 0.75f); d.velocity = Vector2.Zero; d.noGravity = true; }
            }
            else
            {
                hitbox = player.Hitbox;
                noHitbox = true;
            }
        }
        
        public static void OnHitFX(Player player, Entity target, bool crit, Color colour, bool glow = false)
        {
            Vector2 source = player.MountedCenter + new Vector2(
                Main.rand.NextFloatDirection() * 16f,
                Main.rand.NextFloatDirection() * 16f
                );
            Vector2 dir = (target.Center - source).SafeNormalize(Vector2.Zero);
            Dust d = Dust.NewDustPerfect(target.Center - dir * 30f,
                SacredScriptures.DustIDSlashFX, dir * 15f, 0, colour, (crit ? 1.5f : 1f));
            d.noLight = glow;
        }

        public static bool SabreIsChargedStriking(Player player, Item sabre)
        {
            return player == Main.LocalPlayer && sabre.beingGrabbed;
        }

        public static bool AINormalSlash(Projectile projectile, float slashDirection)
        {
            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                projectile.timeLeft = 0;
                return false;
            }

            if (slashDirection == 1 || slashDirection == -1)
            {
                player.HeldItem.noGrabDelay = 0;
                NormalSlash(projectile, player);
                return true;
            }
            return false;
        }

        public static bool AIDashSlash(Player player, Projectile projectile, float dashFrameDuration, float dashSpeed, int freezeFrame, ref Vector2? dashEndVelocity)
        {
            if (player.dead || !player.active)
            {
                projectile.timeLeft = 0;
                return false;
            }
            if (freezeFrame < 1) freezeFrame = 1;

            bool dashing = false;
            if ((int)projectile.ai[0] < dashFrameDuration)
            {
                // Fine-tuned tilecollision
                player.armorEffectDrawShadow = true;
                Vector2 projVel = projectile.velocity;
                if (player.gravDir < 0) projVel.Y = -projVel.Y;
                for (int i = 0; i < 4; i++) //dash distance
                {
                    player.position += Collision.TileCollision(player.position, projVel * dashSpeed / 4,
                        player.width, player.height, false, false, (int)player.gravDir);
                }

                if (player.velocity.Y == 0)
                { player.velocity = new Vector2(0, (projectile.velocity * dashSpeed).Y); }
                else
                { player.velocity = new Vector2(0, player.gravDir * player.gravity); }

                // Prolong mid-slash player animation
                RecentreSlash(projectile, player);
                if (player.itemAnimation <= player.itemAnimationMax - freezeFrame)
                { player.itemAnimation = player.itemAnimationMax - freezeFrame; }

                // Set immunities
                player.immune = true;
                player.immuneTime = Math.Max(player.immuneTime, 6);
                player.immuneNoBlink = true;

                dashing = true;
            }
            else if ((int)projectile.ai[0] >= dashFrameDuration && dashEndVelocity != new Vector2(float.MinValue, float.MinValue))
            {
                if (dashEndVelocity == null)
                {
                    Vector2 projVel = projectile.velocity.SafeNormalize(Vector2.Zero);
                    if (player.gravDir < 0) projVel.Y = -projVel.Y;
                    float speed = dashSpeed / 4f;
                    if (speed < player.maxFallSpeed)
                    { player.velocity = projVel * speed; }
                    else
                    { player.velocity = projVel * player.maxFallSpeed; }

                    // Reset fall damage
                    player.fallStart = (int)(player.position.Y / 16f);
                    player.fallStart2 = player.fallStart;
                }
                else
                {
                    player.velocity = (Vector2)dashEndVelocity;
                }

                // Set the vector to a "reset" state
                dashEndVelocity = new Vector2(float.MinValue, float.MinValue);
            }

            // Trigger lerp by offsetting camera
            if (projectile.timeLeft == 60)
            {
                Main.SetCameraLerp(0.1f, 10);
                Main.screenPosition -= projectile.velocity * 2;
            }

            // Set new projectile frame
            projectile.frame = (int)Math.Max(0, projectile.ai[0] - dashFrameDuration);

            return dashing;
        }

        public static void AISetChargeSlashVariables(Player player, int chargeSlashDirection)
        {
            player.HeldItem.noGrabDelay = player.itemAnimation;
            player.HeldItem.isBeingGrabbed = chargeSlashDirection < 0;
        }

        public static void NormalSlash(Projectile projectile, Player player)
        {
            if (projectile.ai[0] <= 0f)
            {
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
                if (Math.Abs(projectile.rotation) == HALFPI)
                { projectile.spriteDirection = player.direction; }
                else
                { projectile.spriteDirection = Math.Abs(projectile.rotation) <= HALFPI ? 1 : -1; }

                RecentreSlash(projectile, player);
            }
            else
            {
                projectile.position -= projectile.velocity * 2;
            }

            projectile.frame = (int)projectile.ai[0];
            if (projectile.frame >= Main.projFrames[projectile.type] && player.itemAnimation < 2)
            {
                projectile.timeLeft = 0;
            }

            projectile.scale = projectile.knockBack;
        }

        public static bool PreDrawSlashAndWeapon(SpriteBatch spriteBatch, Projectile projectile, int weaponItemID, Color weaponColor, Texture2D slashTexture, Color slashColor, int slashFramecount, float slashDirection = 1f, bool shadow = false)
        {
            Player player = Main.player[projectile.owner];
            Texture2D weapon = Main.itemTexture[weaponItemID];
            if (slashTexture == null)
            {
                slashTexture = Main.projectileTexture[projectile.type];
                slashFramecount = Main.projFrames[projectile.type];
            }

            float slashNormal = MathHelper.Clamp(slashDirection, -1, 1);

            // Flip Horziontally
            SpriteEffects spriteEffect = SpriteEffects.None;
            bool spriteFlipH = false;
            bool spriteFlipV = false;
            if (projectile.spriteDirection < 0)
            {
                spriteFlipH = true;
            }

            // Flip Vertically : Weapon spriteEffect
            float vDir = slashNormal * player.gravDir;
            Vector2 weaponOrigin = weapon.Bounds.BottomLeft();
            if ( vDir < 0)
            {
                spriteFlipV = true;
            }

            if (spriteFlipH)
            {
                spriteEffect = spriteEffect | SpriteEffects.FlipHorizontally;
                weaponOrigin.X = weapon.Bounds.Right;
            }
            if (spriteFlipV)
            {
                spriteEffect = spriteEffect | SpriteEffects.FlipVertically;
                weaponOrigin.Y = weapon.Bounds.Top;
            }

            // Draw weapon if at the start or end animation
            if (projectile.frame > 0 && (
                player.bodyFrame.Y == 1 * player.bodyFrame.Height ||
                player.bodyFrame.Y == 4 * player.bodyFrame.Height))
            {
                spriteBatch.Draw(weapon,
                    player.MountedCenter - Main.screenPosition + new Vector2(0f, 8f * player.gravDir * slashNormal),
                    weapon.Frame(1, 1, 0, 0),
                    weaponColor,
                    player.itemRotation + 3.26f * slashNormal * projectile.spriteDirection,
                    weaponOrigin,
                    projectile.scale,
                    spriteEffect,
                    1f);
            }

            // projectile drawing already mirrors horizontally when needed, just remove reverse flip from earlier
            if(projectile.spriteDirection < 0) { vDir *= -1f; }
            spriteEffect = vDir < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            if (projectile.frame >= 0 &&
                projectile.frame < slashFramecount)
            {
                if(shadow)
                {
                    Vector2 dist = player.position - player.oldPosition;
                    dist = new Vector2(
                        MathHelper.Clamp(dist.X, -32, 32),
                        MathHelper.Clamp(dist.Y, -32, 32));

                    // Draw slashes
                    for (int i = 1; i <= 6; i++)
                    {
                        int iter = i / 2;
                        float itef = i / 2f;

                        if (projectile.frame + iter >= slashFramecount) break;
                        spriteBatch.Draw(slashTexture,
                            projectile.Center - (dist) * itef - Main.screenPosition,
                            slashTexture.Frame(1, slashFramecount, 0, projectile.frame + iter),
                            slashColor * (0.5f - 0.1f * itef),
                            projectile.rotation,
                            new Vector2(slashTexture.Width / 2, slashTexture.Height / (2 * slashFramecount)),
                            projectile.scale,
                            spriteEffect,
                            1f);
                    }
                }

                // Draw slash
                spriteBatch.Draw(slashTexture,
                    projectile.Center - Main.screenPosition,
                    slashTexture.Frame(1, slashFramecount, 0, projectile.frame),
                    slashColor,
                    projectile.rotation,
                    new Vector2(slashTexture.Width / 2, slashTexture.Height / (2 * slashFramecount)),
                    projectile.scale,
                    spriteEffect,
                    1f);
            }
            return false;
        }
        /*--------------------------------------------------------------------------------------------*/
	}

    //NPC Loot
    /*--------------------------------------------------------------------------------------------*/
    public class SacredLoot : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            if (Main.rand.Next(15) == 0) //Higher = lower drop chance
            {
                if (npc.type == NPCID.PirateShip)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("JarofDirt"));
                }
            }
        }
    }

    //Glowmask Config
    /*--------------------------------------------------------------------------------------------*/
	public class ScripturesGlowMask : ModPlayer
	{
		private static readonly Dictionary<int, Texture2D> ItemGlowMask = new Dictionary<int, Texture2D>();

		internal static void Unload()
		{
			ItemGlowMask.Clear();
		}

		public static void AddGlowMask(int itemType, string texturePath)
		{
			ItemGlowMask[itemType] = ModContent.GetTexture(texturePath);
		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			Texture2D textureLegs;
			if (!player.armor[12].IsAir) {
				if (player.armor[12].type >= ItemID.Count && ItemGlowMask.TryGetValue(player.armor[12].type, out textureLegs))//Vanity Legs
				{
					InsertAfterVanillaLayer(layers, "Legs", new PlayerLayer(mod.Name, "GlowLegs", delegate (PlayerDrawInfo info) {
						SacredGlowUtils.DrawArmorGlowMask(EquipType.Legs, textureLegs, info);
					}));
				}
			}
			Texture2D textureBody;
			if (!player.armor[11].IsAir) {
				if (player.armor[11].type >= ItemID.Count && ItemGlowMask.TryGetValue(player.armor[11].type, out textureBody))//Vanity Body
				{
					InsertAfterVanillaLayer(layers, "Body", new PlayerLayer(mod.Name, "GlowBody", delegate (PlayerDrawInfo info) {
						SacredGlowUtils.DrawArmorGlowMask(EquipType.Body, textureBody, info);
					}));
				}
			}
			Texture2D textureHead;
			if (!player.armor[10].IsAir) {
				if (player.armor[10].type >= ItemID.Count && ItemGlowMask.TryGetValue(player.armor[10].type, out textureHead))//Vanity Head
				{
					InsertAfterVanillaLayer(layers, "Head", new PlayerLayer(mod.Name, "GlowHead", delegate (PlayerDrawInfo info) {
						SacredGlowUtils.DrawArmorGlowMask(EquipType.Head, textureHead, info);
					}));
				}
			}
		}

		public static void InsertAfterVanillaLayer(List<PlayerLayer> layers, string vanillaLayerName, PlayerLayer newPlayerLayer)
		{
			for (int i = 0; i < layers.Count; i++) {
				if (layers[i].Name == vanillaLayerName && layers[i].mod == "Terraria") {
					layers.Insert(i + 1, newPlayerLayer);
					return;
				}
			}
			layers.Add(newPlayerLayer);
		}
	}

    //Glowmask armor
    /*--------------------------------------------------------------------------------------------*/
    public static class SacredGlowUtils
	{
		public static void DrawArmorGlowMask(EquipType type, Texture2D texture, PlayerDrawInfo info)
		{
			switch (type) {
				case EquipType.Head: {
					DrawData drawData = new DrawData(texture, new Vector2((int)(info.position.X - Main.screenPosition.X) + ((info.drawPlayer.width - info.drawPlayer.bodyFrame.Width) / 2), (int)(info.position.Y - Main.screenPosition.Y) + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4) + info.drawPlayer.headPosition + info.headOrigin, info.drawPlayer.bodyFrame, info.headGlowMaskColor, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) {
						shader = info.headArmorShader
					};
				Main.playerDrawData.Add(drawData);
				}
				return;

				case EquipType.Body: {
					Rectangle bodyFrame = info.drawPlayer.bodyFrame;
					int num123 = 0;

					bodyFrame.X += num123;
					bodyFrame.Width -= num123;

					if (info.drawPlayer.direction == -1) {
						num123 = 0;
					}

					if (!info.drawPlayer.invis) {
						DrawData drawData = new DrawData(texture, new Vector2((int)(info.position.X - Main.screenPosition.X - (info.drawPlayer.bodyFrame.Width / 2) + (info.drawPlayer.width / 2) + num123), ((int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4))) + info.drawPlayer.bodyPosition + new Vector2(info.drawPlayer.bodyFrame.Width / 2, info.drawPlayer.bodyFrame.Height / 2), bodyFrame, info.bodyGlowMaskColor, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) {
							shader = info.bodyArmorShader
						};
					Main.playerDrawData.Add(drawData);
					}
				}
				return;

				case EquipType.Legs: {
					if (info.drawPlayer.shoe != 15 || info.drawPlayer.wearsRobe) {
						if (!info.drawPlayer.invis) {
							DrawData drawData = new DrawData(texture, new Vector2((int)(info.position.X - Main.screenPosition.X - (info.drawPlayer.legFrame.Width / 2) + (info.drawPlayer.width / 2)), (int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.legFrame.Height + 4)) + info.drawPlayer.legPosition + info.legOrigin, info.drawPlayer.legFrame, info.legGlowMaskColor, info.drawPlayer.legRotation, info.legOrigin, 1f, info.spriteEffects, 0) {
								shader = info.legArmorShader
							};
						Main.playerDrawData.Add(drawData);
						}
					}
				}
				return;
			}
		}
	}
}