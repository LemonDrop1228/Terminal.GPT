using OpenAI;
using Microsoft.Extensions.Options;
using TerminalGPT.Options;

namespace TerminalGPT.Services
{
    public interface IOpenAIClientFactory
    {
        OpenAIClient Client { get; }
    }

    public class OpenAIClientFactory : IOpenAIClientFactory
    {
        private readonly TerminalGptOptions _options;
        private OpenAIClient _client;

        public OpenAIClientFactory(IOptions<TerminalGptOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public OpenAIClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new OpenAIClient(new OpenAIAuthentication(_options.ApiKey, _options.OrgId));
                }

                return _client;
            }
        }
    }
}
