using System;
using CommandSystem.Commands.Console;
using Exiled.API.Features;


namespace DCReplace
{
    public class DCReplace : Plugin<Config>
    {
        private EventHandlers ev;

        public override void OnEnabled()
        {
            base.OnEnabled();

            if (!Config.IsEnabled) return;
            if(Config.ForceDisconnectDropFalse)
                CustomNetworkManager.TypedSingleton._disconnectDrop = false;

            ev = new EventHandlers();

            ev.RegisterEvents();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            ev.UnregisterEvents();

            ev = null;
        }

        public override string Name => "DcReplace";
        public override string Author => "TayTay (Original by Cyanox62)";
        public override Version Version => new Version(1, 2, 0);
    }
}