using Discord.Commands;

namespace ArcaNews_V2.Modules
{
    // public class Commands : InteractionModuleBase<SocketInteractionContext>
    public class Commands : ModuleBase
    {
        [Command("hello")]
        public async Task HelloCommand()
        {
            await ReplyAsync("World!");
        }
    }
}