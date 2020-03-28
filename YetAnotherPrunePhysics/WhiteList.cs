using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YetAnotherPrunePhysics
{
    public static class WhiteList
    {
        private static List<Regex> whiteList;
        private static Dictionary<string, bool> partWhiteList = new Dictionary<string, bool>();

        public static bool IsInWhiteList(AvailablePart part)
        {
            if (!partWhiteList.TryGetValue(part.name, out var result))
            {
                result = Identify(part);
            }
            return result;
        }

        private static bool Identify(AvailablePart part)
        {
            bool result = default;
            try
            {
                result = CanPrunePhysics(part);
                partWhiteList.Add(part.name, result);
                Debug.Log(part.name);
            }
            catch (Exception e)
            {
                Debug.LogError($"YAPP: error identifying part {part.name}");
                Debug.LogException(e);
                partWhiteList.Add(part.name, false);
            }

            return result;
        }

        public static void ReadWhiteList()
        {
            Debug.Log($"YAPP: loading whitelist");
            whiteList = new List<Regex>();
            foreach (UrlDir.UrlFile url in GameDatabase.Instance.root.AllFiles)
            {
                if (!url.fileExtension.Equals("yappwl", StringComparison.OrdinalIgnoreCase))
                    continue;
                Debug.Log($"YAPP: loading whitelist {url.fullPath}");

                var index = 0;
                foreach (var line in File.ReadLines(url.fullPath))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.TrimStart(' ').StartsWith("//"))
                        {
                            whiteList.Add(new Regex(line));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"YAPP: error in { url.name} line {index}\n{e.Message}");
                    }
                }
            }
        }

        private static bool CanPrunePhysics(AvailablePart part)
        {
            foreach (var item in part.partPrefab.Modules)
                if (!CheckName(item.GetType().Name))
                    return false;
            return true;
        }

        private static bool CheckName(string name)
        {
            if (whiteList == null)
                ReadWhiteList();
            for (int i = 0; i < whiteList.Count; i++)
            {
                Regex re = whiteList[i];
                if (re.IsMatch(name))
                    return true;
            }
            return false;
        }
    }
}