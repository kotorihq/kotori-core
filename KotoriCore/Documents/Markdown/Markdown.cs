using System.IO;
using System.Text;
using System.Threading.Tasks;
using KotoriCore.Documents.Deserializers;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Newtonsoft.Json;
using Sushi2;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.TypeResolvers;

namespace KotoriCore.Documents
{
    public class Markdown : IDocument
    {
        readonly string _content;

        public string Identifier { get; }

        public Markdown(string identifier, string content)
        {
            Identifier = identifier;
            _content = content;
        }

        public IDocumentResult Process()
        {
            return AsyncTools.RunSync(ProcessAsync);
        }

        public async Task<IDocumentResult> ProcessAsync()
        {
            var markDownResult = new MarkdownResult(Identifier);

            var tr = new StringReader(_content);

            int? frontMatterStart = null;
            int? frontMatterEnd = null;
            string line;
            var counter = -1;

            var meta = new StringBuilder();
            var body = new StringBuilder();

            while ((line = await tr.ReadLineAsync()) != null)
            {
                counter++;

                if (line.Equals("---"))
                {
                    if (!frontMatterStart.HasValue)
                    {
                        frontMatterStart = counter;
                        continue;
                    }

                    if (!frontMatterEnd.HasValue)
                    {
                        frontMatterEnd = counter;
                        continue;
                    }
                }

                // push to meta
                if (frontMatterStart.HasValue &&
                   !frontMatterEnd.HasValue)
                {
                    meta.AppendLine(line);
                }

                // push to body
                else if (frontMatterStart.HasValue &&
                        frontMatterEnd.HasValue)
                {
                    body.AppendLine(line);
                }
            }

            // check structure
            if (frontMatterStart.HasValue &&
               !frontMatterEnd.HasValue)
            {
                throw new KotoriDocumentException(Identifier, "Invalid front matter. There is a starting tag --- that is not closed.");
            }

            // no front matter
            if (!frontMatterStart.HasValue &&
               !frontMatterEnd.HasValue)
            {
                body = new StringBuilder(_content);
            }

            markDownResult.FrontMatterType = meta.ToString().ToFrontMatterType();
            markDownResult.Content = body.ToString();

            IDeserializer des = null;

            if (markDownResult.FrontMatterType == Enums.FrontMatterType.Yaml)

                des = new Yaml();

            if (markDownResult.FrontMatterType == Enums.FrontMatterType.Json)
                des = new Json();

            if (des != null)
                markDownResult.Meta = des.Deserialize(meta.ToString());

            return markDownResult;
        }
    }
}
