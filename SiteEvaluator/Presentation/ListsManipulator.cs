using System;
using System.Collections.Generic;
using System.Linq;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.Presentation
{
    public static class ListsManipulator
    {
        /**
     * Subtract list2 from list1
     */
        public static IEnumerable<T> SubtractLists<T>(IEnumerable<T> list1, IEnumerable<T> list2)
            where T : IEquatable<T>
        {
            return list1.Where(item => !list2.Contains(item));
        }

        public static void DifferenceReport(IList<ContentLoadResult> crawlerResults,
            IList<ContentLoadResult> siteMapUrls)
        {
            var subtractResultForSiteMap = SubtractLists(siteMapUrls, crawlerResults).ToList();
            var subtractResultForCrawler = SubtractLists(crawlerResults, siteMapUrls).ToList();

            ConsoleMessage.WriteLineSuccess("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");
            subtractResultForSiteMap.ForEach(item => Console.WriteLine(item.PageUrl));
            Console.WriteLine($"Total: {subtractResultForSiteMap.Count}\n");

            ConsoleMessage.WriteLineSuccess("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
            subtractResultForCrawler.ForEach(item => Console.WriteLine(item.PageUrl));
            Console.WriteLine($"Total: {subtractResultForCrawler.Count}");
        }
        
        public static IEnumerable<T> MultiSort<T>(params IEnumerable<T>[] lists) where T : IComparable<T>
        {
            return MultiSort(Comparer<T>.Default, lists);
        }

        public static IEnumerable<T> MultiSort<T>(IComparer<T>? comparer, params IEnumerable<T>[] lists)
        {
            var result = new List<T>();
            foreach (var list in lists)
            {
                result.AddRange(list);
            }
            
            result.Sort(comparer);

            return result;
        }
    }
}