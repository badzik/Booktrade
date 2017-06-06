using Booktrade.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Booktrade
{
    public static class LuceneSearch
    {
        static Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName+@".\LuceneIndex"));

        public static List<Book> SearchBooks(string propertyValue)
        {
            List<Book> books = new List<Book>();
            IndexReader indexReader = IndexReader.Open(directory, true);

            using (var reader = IndexReader.Open(directory, true))
            using (var searcher = new IndexSearcher(reader))
            {
                using (Analyzer myAnalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                {
                    var queryParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, "Author|Title".Split('|'), myAnalyzer);
                    queryParser.AllowLeadingWildcard = true;
                    var query = queryParser.Parse("*" + propertyValue + "*");
                    var collector = TopScoreDocCollector.Create(2, true);
                    searcher.Search(query, collector);
                    var matches = collector.TopDocs().ScoreDocs;
                    foreach (var hit in matches)
                    {
                        var id = hit.Doc;
                        var documentFromSearch = searcher.Doc(id);
                        books.Add(new Models.Book()
                        {
                            BookId= Convert.ToInt32(documentFromSearch.Get("BookId")),
                            Author= documentFromSearch.Get("Author"),
                            Title= documentFromSearch.Get("Title")
                        });

                    }
                }
            }

            return books;
        }
    }
}