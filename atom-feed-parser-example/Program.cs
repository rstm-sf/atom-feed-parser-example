using System;
using System.Linq;

namespace atom_feed_parser_example
{
    class Program
    {
        private static void Main()
        {
            var url = "https://www.myget.org/F/avalonia-ci/api/v2/Packages";
            var packages = FeedParser.Parse(url).ToList();

            Console.WriteLine($"Packages count: {packages.Count}");

            var lastResult = packages.Last();
            Console.WriteLine("Last package info");
            Console.WriteLine("Id:                " + lastResult.Package.Id);
            Console.WriteLine("Version:           " + lastResult.Package.Version);
            Console.WriteLine("Description:       " + lastResult.Description);
            Console.WriteLine("LastEdited:        " + lastResult.LastEdited);
            Console.WriteLine("GalleryDetailsUrl: " + lastResult.GalleryDetailsUrl);
            Console.WriteLine("Dependencies:");
            foreach (var dependency in lastResult.Dependencies)
            {
                Console.WriteLine("  Framework: " + dependency.Framework);
                foreach (var package in dependency.Packages)
                    Console.WriteLine("    " + package);
            }
        }
    }
}
