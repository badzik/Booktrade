using Booktrade.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Booktrade
{
    public static class LuceneSearchIndexer
    {
        static Lucene.Net.Store.Directory directory;
        static Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

        public static void RunIndex(IList<Book> entities)
        {
            directory = FSDirectory.Open(new DirectoryInfo(System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + @".\LuceneIndex"));
            using (var writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (Book book in entities)
                {
                    var document = new Document();
                    document.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Author", book.Author, Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("BookId", book.BookId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    writer.AddDocument(document);
                }
                writer.Optimize();
            }
        }

        public static void UpdateBooksIndex()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (System.IO.Directory.Exists(System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + @".\LuceneIndex")) System.IO.Directory.Delete(System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + @".\LuceneIndex", true);
            var context = new AppDbContext();
            List<Book> books = context.Books.Where(x => x.isSold == false && x.isChanged == false).ToList();
            LuceneSearchIndexer.RunIndex(books);
        }
    }
}