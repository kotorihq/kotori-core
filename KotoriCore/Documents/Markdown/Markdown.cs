using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KotoriCore.Documents
{
    public class Markdown : IDocument
    {
        readonly string _content;

        public Markdown(string content)
        {
            _content = content;
        }

        public async Task<IDocumentResult> ProcessAsync()
        {
            var markDownResult = new MarkdownResult();

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

            // no front matter
            if (!frontMatterStart.HasValue &&
               !frontMatterEnd.HasValue)
            {
                body.Append(_content);
            }

            return markDownResult;
        }
    }
}
