using System.Threading.Tasks;
using fafikspace.client;

namespace fafikspace
{
    class Program
    {
        static async Task Main(string[] args)
            => await new StreamMusicBotClient().InitializeAsync();
    }
}