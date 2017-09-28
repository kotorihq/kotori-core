using System.IO;
using System.Text;
using System.Threading.Tasks;
using KotoriCore.Exceptions;
using KotoriCore.Helpers;
using Sushi2;

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

            while((line = await tr.ReadLineAsync()) != null)
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
                    body.AppendLine(line);;
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
                body.Append(_content);
            }

            markDownResult.FrontMatterType = meta.ToString().ToFrontMatterType();
            markDownResult.Content = body.ToString();

            return markDownResult;
        }
    }
}
