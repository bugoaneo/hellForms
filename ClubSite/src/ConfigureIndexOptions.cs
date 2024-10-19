using Examine;
using Examine.Lucene;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.Examine;

namespace ClubSite.src
{
    public class IndexComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.ConfigureOptions<ConfigureIndexOptions>();
        }
    }

    public sealed class ConfigureIndexOptions : IConfigureNamedOptions<LuceneDirectoryIndexOptions>
    {
        private readonly IUmbracoIndexConfig _umbracoIndexConfig;
        private readonly IOptions<IndexCreatorSettings> _settings;

        public ConfigureIndexOptions(
            IUmbracoIndexConfig umbracoIndexConfig,
            IOptions<IndexCreatorSettings> settings)
        {
            _umbracoIndexConfig = umbracoIndexConfig;
            _settings = settings;
        }

        public void Configure(string name, LuceneDirectoryIndexOptions options)
        {
            switch (name)
            {
                case Constants.UmbracoIndexes.ExternalIndexName:
                    options.FieldDefinitions.TryAdd(new FieldDefinition("newsDate", FieldDefinitionTypes.DateTime));
                    break;
            }
        }

        public void Configure(LuceneDirectoryIndexOptions options)
            => Configure(string.Empty, options);
    }
}
