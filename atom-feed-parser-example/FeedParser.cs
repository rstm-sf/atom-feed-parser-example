using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace atom_feed_parser_example
{
    internal static class FeedParser
    {
        public static IEnumerable<FeedPackage> Parse(string url)
        {
            var doc = XDocument.Load(url);
            if (doc.Root == null)
                yield break;

            foreach (var item in doc.Root.Elements().Where(x => x.Name.LocalName == "entry"))
            {
                var properties = item.Elements()
                    .Single(x => x.Name.LocalName == "properties")
                    .Elements()
                    .ToList();

                yield return new FeedPackage
                {
                    Package = new Package
                    {
                        Id = properties.ParseValue("Id"),
                        Version = properties.ParseValue("Version"),
                    },
                    Description = properties.ParseValue("Description"),
                    GalleryDetailsUrl = properties.ParseValue("GalleryDetailsUrl"),
                    LastEdited = ParseDate(properties.ParseValue("LastEdited")),
                    Dependencies = DependenciesParser.Parse(properties),
                };
            }
        }

        private static DateTime? ParseDate(string date) =>
            DateTime.TryParse(date, out var result)
                ? result
                : (DateTime?)null;

        private static string ParseValue(this IReadOnlyList<XElement> properties, string localName) =>
            properties.SingleOrDefault(x => x.Name.LocalName == localName)?.Value;

        private static class DependenciesParser
        {
            private const string GroupId = "Id";
            private const string GroupVersion = "Version";
            private const string GroupFramework = "Framework";

            private static readonly string s_pattern =
                $@"(?<{GroupId}>.*):(?<{GroupVersion}>.*):(?<{GroupFramework}>.*)";

            private static readonly Regex s_regex = new Regex(s_pattern, RegexOptions.Compiled);

            public static List<Dependency> Parse(IReadOnlyList<XElement> properties)
            {
                var dependencies = new List<Dependency>();
                var dependenciesRaw = properties.ParseValue("Dependencies");
                if (dependenciesRaw == null)
                    return dependencies;

                var currentDependency = new Dependency { Framework = string.Empty };
                foreach (var element in dependenciesRaw.Split("|"))
                {
                    var match = s_regex.Match(element);
                    if (match.Success)
                    {
                        if (currentDependency.Framework != match.Groups[GroupFramework].Value)
                        {
                            currentDependency = new Dependency
                            {
                                Framework = match.Groups[GroupFramework].Value,
                                Packages = new List<Package>(),
                            };
                            dependencies.Add(currentDependency);
                        }

                        currentDependency.Packages.Add(new Package
                        {
                            Id = match.Groups[GroupId].Value,
                            Version = match.Groups[GroupVersion].Value,
                        });
                    }
                }

                return dependencies;
            }
        }
    }
}
