using NetworkMessages.FromServer;
using System;
using System.Net.Mail;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using System.Runtime.InteropServices;
using static TaleWorlds.MountAndBlade.Agent;
using System.Diagnostics.Tracing;
using TaleWorlds.Engine;


namespace glitcHmaniaBL
{
    public class SubModule : MBSubModuleBase
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002; // Left button down
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;   // Left button up
        int ammoCounter = 0;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new glitcHmaniaMissionLogic());
        }

        public class glitcHmaniaMissionLogic : MissionLogic
        {

            private int rifleSoundIndex = SoundEvent.GetEventIdFromString("glitchmania/riflesound");//to avoid string operations in runtime soundIndex can be cached.
            private float nextFireTime = 0f;
            private const float fireCooldown = 0.05f;
            private float nextFireSoundTime = 0f;
            private const float fireSoundCooldown = 0.15f;
            private bool soundMuted = false;
            private float muteEndTime = 0f;
            private bool isAuto = false;
            private short weaponAmmo;


            public override void OnMissionTick(float dt)
            {
                base.OnMissionTick(dt);

                foreach (Agent agent in Mission.Current.Agents)
                {
                    if (agent.IsHuman && agent.IsPlayerControlled)
                    {
                        EquipmentIndex equipmentIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
                        MissionWeapon weapon;
                        string weaponName;
                        if( equipmentIndex != EquipmentIndex.None)
                        {
                            weapon = agent.Equipment[equipmentIndex];
                            weaponName = weapon.Item.Name.ToString();
                            weaponAmmo = weapon.Ammo;
                        }
                        else
                        {
                            weaponName = "None";
                        }

                        if ( weaponName == "Auto Rifle")
                        {

                            if (TaleWorlds.InputSystem.Input.IsKeyDown(TaleWorlds.InputSystem.InputKey.LeftShift))
                            {
                                isAuto = true;

                                if (Mission.CurrentTime > nextFireTime)
                                {
                                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

                                    nextFireTime = Mission.CurrentTime + fireCooldown;
                                }

                                if (Mission.CurrentTime > nextFireSoundTime)
                                {
                                    if (weaponAmmo != 0)
                                    {
                                        Mission.Current.MakeSound(rifleSoundIndex, agent.Position, false, false, -1, -1);
                                        nextFireSoundTime = Mission.CurrentTime + fireSoundCooldown;
                                    }
                                    else if (!soundMuted)
                                    {
                                        soundMuted = true;
                                        muteEndTime = Mission.CurrentTime + 3.5f;
                                    }

                                    if (soundMuted && Mission.CurrentTime > muteEndTime)
                                    {
                                        soundMuted = false;
                                    }

                                }
                                isAuto = false;
                            }

                            else if (TaleWorlds.InputSystem.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.LeftMouseButton) && !soundMuted && !isAuto && weaponAmmo != 0)
                            {
                                Mission.Current.MakeSound(rifleSoundIndex, agent.Position, false, false, -1, -1);
                                nextFireSoundTime = Mission.CurrentTime + fireSoundCooldown;
                            }
                            

                        }
                    }
                }

            }

        }

    }
}