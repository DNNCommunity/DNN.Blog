using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Modules.Blog.Core.Entities.Terms;
using DotNetNuke.Modules.Blog.Models;

namespace DotNetNuke.Modules.Blog.Common
{
    public static class CategoryTreeHelper
    {
        public static List<PostCategoryNode> BuildCategoryTree(IEnumerable<TermInfo> terms, IEnumerable<int> selectedIds)
        {
            var termList = terms?.ToList() ?? new List<TermInfo>();
            var selected = selectedIds != null
                ? new HashSet<int>(selectedIds)
                : new HashSet<int>();

            var roots = termList
                .Where(t => t.ParentTermId == null || t.ParentTermId == 0)
                .OrderBy(t => t.Name)
                .ToList();

            var result = new List<PostCategoryNode>();
            foreach (var root in roots)
                result.Add(BuildCategoryNode(root, termList, selected, 0));

            var included = new HashSet<int>(FlattenIds(result));
            foreach (var orphan in termList.Where(t => !included.Contains(t.TermId)).OrderBy(t => t.Name))
                result.Add(BuildCategoryNode(orphan, termList, selected, 0));

            return result;
        }

        public static List<int> ParseCategoryIds(string categories)
        {
            var ids = new List<int>();
            if (string.IsNullOrWhiteSpace(categories))
                return ids;

            foreach (var part in categories.Split(','))
            {
                if (int.TryParse(part.Trim(), out int id) && id > 0)
                    ids.Add(id);
            }
            return ids;
        }

        public static string JoinCategoryIds(IEnumerable<int> ids)
        {
            if (ids == null)
                return "";
            return string.Join(",", ids.Where(id => id > 0).Distinct());
        }

        private static PostCategoryNode BuildCategoryNode(
            TermInfo term,
            List<TermInfo> all,
            HashSet<int> selectedIds,
            int depth)
        {
            var node = new PostCategoryNode
            {
                TermId = term.TermId,
                Name = term.Name,
                Depth = depth,
                Selected = selectedIds.Contains(term.TermId)
            };

            foreach (var child in all
                .Where(t => t.ParentTermId.HasValue && t.ParentTermId.Value == term.TermId)
                .OrderBy(t => t.Name))
            {
                node.Children.Add(BuildCategoryNode(child, all, selectedIds, depth + 1));
            }

            return node;
        }

        private static IEnumerable<int> FlattenIds(IEnumerable<PostCategoryNode> nodes)
        {
            foreach (var n in nodes)
            {
                yield return n.TermId;
                foreach (var c in FlattenIds(n.Children))
                    yield return c;
            }
        }
    }
}
