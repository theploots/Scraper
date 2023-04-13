using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Web;


namespace Scraper
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            string url = "https://books.toscrape.com/catalogue/category/books/travel_2/index.html";
            var doc = GetDocument(url);
            var links = GetBookLinks(url);// here we get the links to the books by calling the method GetBookLinks

            List<Book> books = GetBooks(links);
           
            DisplayBooks(books);
        }


        //here we display the books that have been scraped in the console
        //To display the apostrophes correctly, you can use the HttpUtility.HtmlDecode() method from the System.Web namespace to decode the HTML entities back into their original characters.
        static void DisplayBooks(List<Book> books)
        {
            foreach (var book in books)
            {
                Console.WriteLine($"{HttpUtility.HtmlDecode(book.Title)} : {book.Price}$");
                Console.ForegroundColor= ConsoleColor.Green;
                Console.WriteLine("------------------------------------------------------------------------------------------------------------");
                Console.ResetColor();


            }
        }



        // Here i use methods form HTMLAgilityPack to select singleNodes while using the XPATH(XML Path Language) to identify what to get.
        // Here we go one by one the links to the books and get the title and the price of each book
        private static List<Book> GetBooks(List<string> links)
        {
            var books = new List<Book>();
            foreach (var link in links)//this link represents the book link
            {
                var doc = GetDocument(link);//here we get the document using the link with the method GetDocument
                var book = new Book();
                book.Title=doc.DocumentNode.SelectSingleNode("//h1").InnerText; // here we assign the title to the book in this list of books by using the XPATH to identify the title
                var xpath = "//*[@class=\"col-sm-6 product_main\"]/*[@class=\"price_color\"]"; // here is the xpath to the price
                var price_raw= doc.DocumentNode.SelectSingleNode(xpath).InnerText; // here we assign the string associated to the xpath to the variable price_raw                
                book.Price = ExtractPrice(price_raw);//uses the method ExtractPrice to extract the price from the string
                books.Add(book);//adds the book to the list of books in the List of Type Book
            }
            return books;
        }
        
      

        // here we use the Regex to extract the price from the string

        static double ExtractPrice(string raw_string)
        {
            var reg = new Regex(@"[\'\d\.,]+",RegexOptions.Compiled);
            var m = reg.Match(raw_string);
            if(!m.Success)//.Success returns a true or false value. If the match is successful, the value is true; otherwise, the value is false.
            {
                return 0;
            }

            return Convert.ToDouble(m.Value);//returns the captured substring by the match
        }


        //here we select all nodes(SelectNodes) from the root document (DocumentNode)
        //here is specifically for link nodes
        static List<string> GetBookLinks(string url)
        {
            var doc = GetDocument(url);
            var linkNodes=   doc.DocumentNode.SelectNodes("//h3/a");// returns an HtmlNodeCollection of all the nodes that match the specified XPath query.

            var baseUri= new Uri(url);// initializes a new instance of the Uri class with the specified Uri string.
            var links = new List<string>();// initializes a new instance of the List class that is empty and has the default initial capacity.
            foreach (var linkNode in linkNodes)
            {
                 var link = linkNode.Attributes["href"].Value;// check node attribute "href"
                link = new Uri(baseUri, link).AbsoluteUri;// convert it to the to the absolute Uniform Resource Identifier(Uri) using baseUri that is the url + link that is a value of linkNodes
                links.Add(link);// add link to the List<string>();
            }
            return links;
        }


        
        // Here i create the document using HtmlWeb which exposes one method called load which takes one url and returns HtmlDocument,
        // which connects to the site but also converts it and parses it into HtmlDocument object that is ready to be querried.
        static  HtmlDocument GetDocument(string url)
        {
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);//gets the document from the url
            return doc;
            
        }
    }
}