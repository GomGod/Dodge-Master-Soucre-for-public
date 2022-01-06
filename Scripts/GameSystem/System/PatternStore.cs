
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PatternStore
{
    public readonly List<Pattern> ListPatterns = new();

    public void InitPatternStorage()
    {
        LoadXML();
    }

    private void LoadXML()
    {
        var patternData = (TextAsset)Resources.Load("PatternData");

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(patternData.text);
        var nodes = xmlDoc.SelectNodes("root/Patterns/Pattern");

        if (nodes is null)
            return;
        
        foreach(XmlNode node in nodes)
        {
            Pattern tempPattern = new()
            {
                Difficulty = float.Parse(node.SelectSingleNode("difficulty")?.InnerText ?? string.Empty)
            };
            
            var projectiles = node.SelectNodes("Projectiles/ProjectileInfo");
            if (projectiles == null) continue;
            
            foreach (XmlNode pNode in projectiles)
            {
                var genNumber = int.Parse(pNode.SelectSingleNode("GeneratePos")?.InnerText ?? string.Empty);
                var dir = genNumber < 5 ? new Vector2(0, 1) : new Vector2(1, 0);
                var genPos = genNumber < 5 ? new Vector2(genNumber + 1, 0) : new Vector2(0, genNumber - 4);

                var tempPrj = new ProjectileInfo()
                {
                    GenPosVector = genPos,
                    Dir = dir,
                    WarningDelay = float.Parse(pNode.SelectSingleNode("WarningDelay")?.InnerText ?? string.Empty),
                    AfterDelay = float.Parse(pNode.SelectSingleNode("Delay")?.InnerText ?? string.Empty),
                };

                tempPattern.ProjectileQueue.Enqueue(tempPrj);
            }
            
            ListPatterns.Add(tempPattern);
        }
    }
}