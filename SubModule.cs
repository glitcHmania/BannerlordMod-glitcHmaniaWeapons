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


namespace glitcHmaniaBL
{
    public class SubModule : MBSubModuleBase
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002; // Left button down
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;   // Left button up

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
            private float nextFireTime = 0f;
            private const float fireCooldown = 0.03f;

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
                        }
                        else
                        {
                            weaponName = "None";
                        }

                        if (weaponName == "Auto Rifle")
                        {
                            if (TaleWorlds.InputSystem.Input.IsKeyDown(TaleWorlds.InputSystem.InputKey.LeftShift) && agent.IsRangedCached)
                            {
                                if (Mission.CurrentTime > nextFireTime)
                                {
                                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

                                    nextFireTime = Mission.CurrentTime + fireCooldown;
                                }
                            }
                            
                        }
                    }
                }

            }

        }

    }
}